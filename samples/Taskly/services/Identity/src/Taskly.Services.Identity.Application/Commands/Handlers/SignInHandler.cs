using Jalpan.Messaging.Brokers;
using Jalpan.Security.Rng;
using Microsoft.Extensions.Options;
using Taskly.Services.Identity.Application.DTO;
using Taskly.Services.Identity.Application.Events;
using Taskly.Services.Identity.Application.Services;
using Taskly.Services.Identity.Domain.Entities;
using Taskly.Services.Identity.Domain.Exceptions;
using Taskly.Services.Identity.Domain.Repositories;

namespace Taskly.Services.Identity.Application.Commands.Handlers;

[UsedImplicitly]
internal sealed class SignInHandler(
    IUserRepository userRepository,
    IPasswordService passwordService,
    ILogger<SignInHandler> logger,
    IJwtProvider jwtProvider,
    IRng rng,
    IRefreshTokenRepository refreshTokenRepository,
    IOptions<RefreshTokenOptions> refreshTokenOptions,
    IDateTime dateTime,
    IMessageBroker messageBroker) : ICommandHandler<SignIn, AuthDto>
{
    public async Task<AuthDto> HandleAsync(SignIn command, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetAsync(command.Email, cancellationToken);
        if (user is null)
        {
            logger.LogError("User with email {Email} was not found.", command.Email);
            throw new InvalidCredentialsException(command.Email);
        }

        if (!passwordService.IsValid(user.Password, command.Password))
        {
            logger.LogError("Invalid password for user with id {UserId}", user.Id.Value);
            throw new InvalidCredentialsException(command.Email);
        }

        var claims = user.Permissions.Any()
            ? new Dictionary<string, IEnumerable<string>>
            {
                ["permissions"] = user.Permissions
            }
            : null;

        var accessToken = jwtProvider.Create(user.Id, user.Email, user.Role, claims: claims);
        var refreshToken = await CreateRefreshTokenAsync(user.Id);

        logger.LogInformation("User with id {UserId} has been authenticated.", user.Id);

        await messageBroker.SendAsync(new SignedIn(user.Id, user.Role), cancellationToken);

        return new AuthDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    private async Task<RefreshTokenDto> CreateRefreshTokenAsync(Guid userId)
    {
        var token = rng.Generate(30);
        var expiresAt = dateTime.Now.Add(refreshTokenOptions.Value.Expiry);
        var refreshToken = new RefreshToken(new AggregateId(), userId, token, dateTime.Now, expiresAt);
        await refreshTokenRepository.AddAsync(refreshToken);
        return new RefreshTokenDto
        {
            Token = token,
            ExpiresAt = expiresAt
        };
    }
}