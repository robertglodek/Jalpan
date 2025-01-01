using Jalpan.Messaging.Brokers;
using Taskly.Services.Identity.Application.Events;
using Taskly.Services.Identity.Application.Exceptions;
using Taskly.Services.Identity.Domain.Repositories;

namespace Taskly.Services.Identity.Application.Commands.Handlers;

[UsedImplicitly]
internal sealed class SetLockHandler(
    IUserRepository userRepository,
    IDateTime dateTime,
    ILogger<SetLockHandler> logger,
    IMessageBroker messageBroker)
    : ICommandHandler<SetLock, Empty>
{
    public async Task<Empty> HandleAsync(SetLock command, CancellationToken cancellationToken = default)
        => await Empty.ExecuteAsync(async () =>
        {
            var user = await userRepository.GetAsync(command.UserId);
            if (user is null)
            {
                throw new UserNotFoundException(command.UserId);
            }

            user.SetLock(command.To, command.Reason);
            user.LastModifiedAt = dateTime.Now;
            await userRepository.UpdateAsync(user);
            
            logger.LogInformation("Changed lock state for the user with id: {UserId}", user.Id);
            
            await messageBroker.SendAsync(new LockSet(command.UserId, command.To), cancellationToken);
        });
}