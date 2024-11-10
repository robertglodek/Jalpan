using System.Text.Json;
using System.Text.Json.Serialization;

namespace Jalpan.HTTP.Serialization;

public sealed class SystemTextJsonHttpClientSerializer : IHttpClientSerializer
{
    private readonly JsonSerializerOptions? _options = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    public string Serialize<T>(T? value) => JsonSerializer.Serialize(value, _options);

    public ValueTask<T?> DeserializeAsync<T>(Stream stream) => JsonSerializer.DeserializeAsync<T>(stream, _options);
}