using Taskly.Services.Identity.Domain.Exceptions;
using Taskly.Services.Identity.Domain.Repositories;

namespace Taskly.Services.Identity.Application.Commands.Handlers;

[UsedImplicitly]
internal sealed class RevokeRefreshTokenHandler(IRefreshTokenRepository refreshTokenRepository, IDateTime dateTime)
    : ICommandHandler<RevokeRefreshToken, Empty>
{
    public async Task<Empty> HandleAsync(RevokeRefreshToken command, CancellationToken cancellationToken = default)
        => await Empty.ExecuteAsync(async () =>
        {
            var token = await refreshTokenRepository.GetAsync(command.RefreshToken) ??
                        throw new InvalidRefreshTokenException();
            token.Revoke(dateTime.Now);
            await refreshTokenRepository.UpdateAsync(token);
        });
}