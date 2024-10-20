namespace Taskly.Services.Identity.Application.Commands;

public sealed class RevokeAccessToken : ICommand<Empty>
{
    public string AccessToken { get; }

    public RevokeAccessToken(string accessToken)
    {
        AccessToken = accessToken;
    }
}
