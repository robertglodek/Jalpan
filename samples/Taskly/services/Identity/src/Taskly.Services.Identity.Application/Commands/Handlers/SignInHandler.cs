using Jalpan.Time;
using Microsoft.Extensions.Logging;
using Taskly.Services.Identity.Application.DTO;
using Taskly.Services.Identity.Application.Events;
using Taskly.Services.Identity.Application.Services;
using Taskly.Services.Identity.Core.Entities;
using Taskly.Services.Identity.Core.Exceptions;
using Taskly.Services.Identity.Domain.ValueObjects;

namespace Taskly.Services.Identity.Application.Commands.Handlers;

internal sealed class SignInHandler(
    IUserRepository userRepository,
    IDateTime dateTime, IPasswordService passwordService,
    ILogger<SignInHandler> logger) : ICommandHandler<SignIn, AuthDto>
{
    private readonly IDateTime _dateTime = dateTime;

    public async Task<AuthDto> HandleAsync(SignIn command, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetAsync(command.Email);
        if (user is not null)
        {
            logger.LogError("Email already in use: {Email}", command.Email);
            throw new EmailInUseException(command.Email);
        }

        var role = Role.From(command.)
        var password = passwordService.Hash(command.Password);
        user = new User(command.UserId, command.Email, password, role, DateTime.UtcNow, command.Permissions);
        await userRepository.AddAsync(user);

        logger.LogInformation($"Created an account for the user with id: {user.Id}.");
        await _messageBroker.PublishAsync(new SignedUp(user.Id, user.Email, user.Role));
    }
}

