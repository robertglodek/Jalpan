using Jalpan.Time;
using Taskly.Services.Identity.Core.Exceptions;

namespace Taskly.Services.Identity.Application.Commands.Handlers;

internal sealed class RevokeRefreshTokenHandler(IRefreshTokenRepository refreshTokenRepository, IDateTime dateTime) : ICommandHandler<RevokeRefreshToken, Empty>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
    private readonly IDateTime _dateTime = dateTime;
    public async Task<Empty> HandleAsync(RevokeRefreshToken command, CancellationToken cancellationToken = default)
        => await Empty.ExecuteAsync(async () =>
        {
            var token = await _refreshTokenRepository.GetAsync(command.RefreshToken) ?? throw new InvalidRefreshTokenException();
            token.Revoke(_dateTime.Now);
            await _refreshTokenRepository.UpdateAsync(token);
        });
}
