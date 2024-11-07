using Microsoft.AspNetCore.Http;
using System.Reflection;
using Jalpan.Helpers;
using System.Net.Mime;
using Jalpan.Types;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Jalpan.WebApi.Contracts;

public sealed class PublicContractsMiddleware(
    RequestDelegate next,
    string endpoint,
    Type attributeType,
    bool attributeRequired)
{
    private readonly string _endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
    private readonly RequestDelegate _next = next ?? throw new ArgumentNullException(nameof(next));
    private readonly Lazy<string> _serializedContracts = new(() => LoadContracts(attributeType, attributeRequired), isThreadSafe: true);
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };
    
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path == _endpoint)
        {
            context.Response.ContentType = MediaTypeNames.Application.Json;
            await context.Response.WriteAsync(_serializedContracts.Value);
            return;
        }

        await _next(context);
    }

    private static string LoadContracts(Type attributeType, bool attributeRequired)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var types = assemblies.SelectMany(a => a.GetTypes())
            .Where(t => (!attributeRequired || t.GetCustomAttribute(attributeType) is not null) && !t.IsInterface)
            .ToArray();

        var contracts = new ContractTypes();

        foreach (var @event in types.Where(t => typeof(IEvent).IsAssignableFrom(t) && t != typeof(RejectedEvent)))
        {
            var instance = @event.GetDefaultInstance();
            var name = instance!.GetType().Name;

            if (!contracts.Events.TryAdd(name, instance))
            {
                throw new InvalidOperationException($"Event: '{name}' already exists.");
            }
        }

        return JsonSerializer.Serialize(contracts, Options);
    }

    private class ContractTypes
    {
        public Dictionary<string, object?> Events { get; } = [];
    }
}