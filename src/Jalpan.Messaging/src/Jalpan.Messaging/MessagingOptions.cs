namespace Jalpan.Messaging;

public sealed class MessagingOptions
{
    public ResiliencyOptions Resiliency { get; init; } = new();

    public sealed class ResiliencyOptions
    {
        public int Retries { get; init; } = 3;
        public TimeSpan? RetryInterval { get; init; }
        public bool Exponential { get; init; }
    }
}