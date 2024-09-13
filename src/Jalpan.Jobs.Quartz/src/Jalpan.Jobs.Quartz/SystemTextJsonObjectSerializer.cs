using Microsoft.Extensions.Logging;
using Quartz.Spi;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Jalpan.Jobs.Quartz;

public class SystemTextJsonObjectSerializer : IObjectSerializer
{
    private readonly ILogger<SystemTextJsonObjectSerializer> _logger;

    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    public SystemTextJsonObjectSerializer(ILogger<SystemTextJsonObjectSerializer> logger)
    {
        _logger = logger;
    }

    public void Initialize()
    {
        _logger.LogInformation("Quartz SystemTextJsonObjectSerializer initialized with options: {Options}", _options);
    }

    public T? DeSerialize<T>(byte[] data) where T : class => JsonSerializer.Deserialize<T>(data, _options);


    public byte[] Serialize<T>(T obj) where T : class => JsonSerializer.SerializeToUtf8Bytes(obj, _options);

}