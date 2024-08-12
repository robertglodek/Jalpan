namespace Jalpan.Time;

public class UtcDateTime : IDateTime
{
    public DateTime Now => DateTime.UtcNow;
}
