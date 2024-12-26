namespace Taskly.Services.Identity.Application;

public sealed class RefreshTokenOptions
{
    public TimeSpan Expiry { get; init; } = TimeSpan.FromHours(720);
}