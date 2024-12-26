namespace Jalpan.Time;

internal sealed class UtcDateTime : IDateTime
{
    public DateTime Now => DateTime.UtcNow;

    public DateTime UnixMsToDateTime(long unixMilliseconds)
        => DateTimeOffset.FromUnixTimeMilliseconds(unixMilliseconds).UtcDateTime;
}