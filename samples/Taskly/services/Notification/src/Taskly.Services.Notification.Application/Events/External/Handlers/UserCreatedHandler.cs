using Jalpan;
using Jalpan.Handlers;
using Jalpan.Time;
using Microsoft.Extensions.Logging;
using Taskly.Services.Notification.Application.Exceptions;
using Taskly.Services.Notification.Domain.Entities;
using Taskly.Services.Notification.Domain.Repositories;
using Taskly.Services.Notification.Domain.ValueObjects;

namespace Taskly.Services.Notification.Application.Events.External.Handlers;

internal sealed class UserCreatedHandler(IUserRepository userRepository, ILogger<UserCreatedHandler> logger, IDateTime dateTime) : IEventHandler<UserCreated>
{
    public async Task HandleAsync(UserCreated @event, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetAsync(@event.Id, cancellationToken);
        if(user is not null)
        {
            throw new UserAlreadyCreatedException(@event.Id);
        }
        logger.LogInformation("Received '{EventName}' event with user id: {UserId}", nameof(UserCreated).Underscore(), @event.Id);
        user = new User(@event.Id, @event.Email, Role.From(@event.Role), dateTime.Now, null, @event.Permissions);
        await userRepository.AddAsync(user, cancellationToken);

        await Task.CompletedTask;
    }
}
