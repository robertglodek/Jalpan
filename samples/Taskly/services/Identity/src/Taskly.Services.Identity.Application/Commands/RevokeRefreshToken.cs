namespace Taskly.Services.Identity.Application.Commands;

[UsedImplicitly]
public sealed class RevokeRefreshToken(string refreshToken) : ICommand<Empty>
{
    public string RefreshToken { get; } = refreshToken;
}
