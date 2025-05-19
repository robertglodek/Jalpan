using Jalpan;
using Jalpan.Handlers;
using Jalpan.Time;
using Microsoft.Extensions.Logging;
using Taskly.Services.Notification.Application.Exceptions;
using Taskly.Services.Notification.Domain.Repositories;
using Taskly.Services.Notification.Domain.ValueObjects;

namespace Taskly.Services.Notification.Application.Events.External.Handlers;

internal sealed class UserUpdatedHandler(IUserRepository userRepository, ILogger<UserUpdatedHandler> logger, IDateTime dateTime) : IEventHandler<UserUpdated>
{
    public async Task HandleAsync(UserUpdated @event, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Received '{EventName}' event with user id: {UserId}", nameof(UserUpdated).Underscore(), @event.Id);
        var user = await userRepository.GetAsync(@event.Id, cancellationToken) ?? throw new UserNotFoundException(@event.Id);

        user.UpdateEmail(@event.Email);
        user.Role = Role.From(@event.Role);
        user.UpdatePermissions(@event.Permissions?.Select(Permission.From));
        user.LastModifiedAt = dateTime.Now;

        await userRepository.UpdateAsync(user, cancellationToken);
        await Task.CompletedTask;
    }
}
