namespace Taskly.Services.Identity.Application.Commands;

public sealed class RevokeRefreshToken : ICommand<Empty>
{
    public string RefreshToken { get; }

    public RevokeRefreshToken(string refreshToken)
    {
        RefreshToken = refreshToken;
    }
}
