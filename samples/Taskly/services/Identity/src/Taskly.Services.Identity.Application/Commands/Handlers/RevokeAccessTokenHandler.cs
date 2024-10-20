using Jalpan.Auth.Managers;

namespace Taskly.Services.Identity.Application.Commands.Handlers;

internal sealed class RevokeAccessTokenHandler(IAccessTokenManager accessTokenManager) : ICommandHandler<RevokeAccessToken, Empty>
{
    private readonly IAccessTokenManager _accessTokenManager = accessTokenManager;
    public async Task<Empty> HandleAsync(RevokeAccessToken command, CancellationToken cancellationToken = default)
        => await Empty.ExecuteAsync(async () => await _accessTokenManager.DeactivateAsync(command.AccessToken));
}