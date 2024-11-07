namespace Jalpan.HTTP;

public sealed class HttpClientOptions
{
    public string Name { get; init; } = string.Empty;
    public CertificateOptions? Certificate { get; init; }
    public ResiliencyOptions Resiliency { get; init; } = new();
    public RequestMaskingOptions RequestMasking { get; init; } = new();
    
    public Dictionary<string, string> Services { get; init; } = [];

    public sealed class CertificateOptions
    {
        public string Location { get; init; } = string.Empty;
        public string? Password { get; init; }
    }

    public sealed class ResiliencyOptions
    {
        public int Retries { get; init; } = 3;
        public TimeSpan? RetryInterval { get; init; }
        public bool Exponential { get; init; }
    }

    public class RequestMaskingOptions
    {
        public bool Enabled { get; init; }
        public List<string> UrlParts { get; init; } = [];
        public string? MaskTemplate { get; init; }
    }
}