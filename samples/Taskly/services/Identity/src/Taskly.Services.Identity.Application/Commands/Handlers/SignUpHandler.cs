using Jalpan.Messaging.Brokers;
using Taskly.Services.Identity.Application.Events;
using Taskly.Services.Identity.Application.Services;
using Taskly.Services.Identity.Domain.Entities;
using Taskly.Services.Identity.Domain.Exceptions;
using Taskly.Services.Identity.Domain.Repositories;
using Taskly.Services.Identity.Domain.ValueObjects;

namespace Taskly.Services.Identity.Application.Commands.Handlers;

[UsedImplicitly]
internal sealed class SignUpHandler(
    IUserRepository userRepository,
    IDateTime dateTime,
    IPasswordService passwordService,
    ILogger<SignUpHandler> logger, 
    IMessageBroker messageBroker) : ICommandHandler<SignUp, Empty>
{
    public async Task<Empty> HandleAsync(SignUp command, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetAsync(command.Email);
        if (user is not null)
        {
            logger.LogError("Email already in use: {Email}", command.Email);
            throw new EmailInUseException(command.Email);
        }

        var role = Role.From(command.Role);
        var password = passwordService.Hash(command.Password);

        user = new User(command.UserId, command.Email, password, role, dateTime.Now, permissions: command.Permissions);
        await userRepository.AddAsync(user);

        logger.LogInformation("Created an account for the user with id: {UserId}", user.Id);
        await messageBroker.SendAsync(new SignedUp(command.UserId, command.Email, command.Role), cancellationToken);
        return Empty.Value;
    }
}