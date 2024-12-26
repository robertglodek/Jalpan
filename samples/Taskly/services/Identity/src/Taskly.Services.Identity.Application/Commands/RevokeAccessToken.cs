namespace Taskly.Services.Identity.Application.Commands;

[UsedImplicitly]
public sealed class RevokeAccessToken(string accessToken) : ICommand<Empty>
{
    public string AccessToken { get; } = accessToken;
}
