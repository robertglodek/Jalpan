namespace Jalpan.Time;

public interface IDateTime
{
    DateTime Now { get; }
    DateTime UnixMsToDateTime(long unixMilliseconds);
}
