using Jalpan.Auth.Jwt.Managers;

namespace Taskly.Services.Identity.Application.Commands.Handlers;

[UsedImplicitly]
internal sealed class RevokeAccessTokenHandler(IAccessTokenManager accessTokenManager)
    : ICommandHandler<RevokeAccessToken, Empty>
{
    public async Task<Empty> HandleAsync(RevokeAccessToken command, CancellationToken cancellationToken = default)
        => await Empty.ExecuteAsync(async () => await accessTokenManager.DeactivateAsync(command.AccessToken));
}