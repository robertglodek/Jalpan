using Jalpan.Attributes;
using Jalpan.Types;

namespace Taskly.Services.Notification.Application.Events.External;

[Message("users", "user_created", "notifications.user_created")]
public record UserCreated(Guid Id, string Role, string Email, IEnumerable<string> Permissions) : IEvent;
