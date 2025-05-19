using Taskly.Services.Notification.Domain.Exceptions;

namespace Taskly.Services.Notification.Domain.Entities
{
    public sealed class NotificationAction : AggregateRoot
    {
        public Guid UserId { get; private set; }
        public Guid TaskId { get; private set; }
        public DateTime? LastModifiedAt { get; set; }
        public DateTime NotificationDate { get; private set; }
        public NotificationAction(Guid id, DateTime createdAt, DateTime? lastModifiedAt, Guid userId, Guid taskId, DateTime notificationDate)
        {
            Id = id;

            if (Guid.Empty == userId)
            {
                throw new InvalidUserIdException();
            }

            if(Guid.Empty == taskId)
            {
                throw new InvalidTaskIdException();
            }

            UserId = userId;
            TaskId = taskId;
            NotificationDate = notificationDate;
            CreatedAt = createdAt;
            LastModifiedAt = lastModifiedAt;
        }
    }
}
