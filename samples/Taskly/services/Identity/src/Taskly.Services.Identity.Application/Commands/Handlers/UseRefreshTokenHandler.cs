using Taskly.Services.Identity.Application.DTO;
using Taskly.Services.Identity.Application.Exceptions;
using Taskly.Services.Identity.Application.Services;
using Taskly.Services.Identity.Domain.Exceptions;
using Taskly.Services.Identity.Domain.Repositories;

namespace Taskly.Services.Identity.Application.Commands.Handlers;

[UsedImplicitly]
internal sealed class UseRefreshTokenHandler(
    IDateTime dateTime,
    IRefreshTokenRepository refreshTokenRepository,
    IUserRepository userRepository,
    IJwtProvider jwtProvider) : ICommandHandler<UseRefreshToken, AuthDto>
{
    public async Task<AuthDto> HandleAsync(UseRefreshToken command, CancellationToken cancellationToken = default)
    {
        var token = await refreshTokenRepository.GetAsync(command.RefreshToken, cancellationToken);
        if (token is null)
        {
            throw new InvalidRefreshTokenException();
        }

        if (token.Revoked)
        {
            throw new RevokedRefreshTokenException();
        }

        if (token.ExpiresAt < dateTime.Now)
        {
            throw new ExpiredRefreshTokenException();
        }

        var user = await userRepository.GetAsync(token.UserId, cancellationToken);
        if (user is null)
        {
            throw new UserNotFoundException(token.UserId);
        }

        var claims = user.Permissions.Any()
            ? new Dictionary<string, IEnumerable<string>>
            {
                ["permissions"] = user.Permissions
            }
            : null;

        var accessToken = jwtProvider.Create(user.Id, user.Email, user.Role, claims: claims);
        var refreshToken = new RefreshTokenDto
        {
            ExpiresAt = token.ExpiresAt,
            Token = token.Token
        };

        return new AuthDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }
}