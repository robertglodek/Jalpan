using Jalpan.CQRS.Events;
using Microsoft.AspNetCore.Http;
using System.Reflection;
using Jalpan.CQRS.Commands;
using Jalpan.Helpers;
using System.Net.Mime;
using Jalpan.Serialization;

namespace Jalpan.WebApi.CQRS.Middlewares;

public class PublicContractsMiddleware
{
    private static int _initialized;
    private readonly string _endpoint;
    private readonly RequestDelegate _next;
    private readonly bool _attributeRequired;
    private readonly IJsonSerializer _jsonSerializer;
    private static string _serializedContracts = null!;

    public PublicContractsMiddleware(
        IJsonSerializer jsonSerializer,
        RequestDelegate next,
        string endpoint,
        Type attributeType,
        bool attributeRequired)
    {
        _jsonSerializer = jsonSerializer;
        _next = next;
        _endpoint = endpoint;
        _attributeRequired = attributeRequired;

        Load(attributeType);
    }

    public Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path != _endpoint)
        {
            return _next(context);
        }

        context.Response.ContentType = MediaTypeNames.Application.Json;
        context.Response.WriteAsync(_serializedContracts);

        return Task.CompletedTask;
    }

    private void Load(Type attributeType)
    {
        if (Interlocked.Exchange(ref _initialized, 1) == 1)
        {
            return;
        }

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var types = assemblies.SelectMany(a => a.GetTypes())
            .Where(t => (!_attributeRequired || t.GetCustomAttribute(attributeType) is not null) && !t.IsInterface)
            .ToArray();

        var contracts = new ContractTypes();

        foreach (var command in types.Where(t => typeof(ICommand).IsAssignableFrom(t)))
        {
            var instance = command.GetDefaultInstance();
            var name = instance!.GetType().Name;

            if (contracts.Commands.ContainsKey(name))
            {
                throw new InvalidOperationException($"Command: '{name}' already exists.");
            }

            contracts.Commands[name] = instance;
        }

        foreach (var @event in types.Where(t => typeof(IEvent).IsAssignableFrom(t) &&
                                                    t != typeof(RejectedEvent)))
        {
            var instance = @event.GetDefaultInstance();
            var name = instance!.GetType().Name;

            if (contracts.Events.ContainsKey(name))
            {
                throw new InvalidOperationException($"Event: '{name}' already exists.");
            }

            contracts.Events[name] = instance;
        }

        _serializedContracts = _jsonSerializer.Serialize(contracts);
    }

    private class ContractTypes
    {
        public Dictionary<string, object?> Commands { get; } = [];
        public Dictionary<string, object?> Events { get; } = [];
    }
}