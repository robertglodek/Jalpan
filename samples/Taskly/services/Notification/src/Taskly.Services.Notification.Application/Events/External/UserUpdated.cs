using Jalpan.Attributes;
using Jalpan.Types;

namespace Taskly.Services.Notification.Application.Events.External;

[Message("users", "user_updated", "notifications.user_updated")]
public record UserUpdated(Guid Id, string Role, string Email, IEnumerable<string> Permissions) : IEvent;
