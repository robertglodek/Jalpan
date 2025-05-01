using Taskly.Services.Notification.Application.DTO;
using Taskly.Services.Notification.Domain.Entities;
using Taskly.Services.Notification.Domain.ValueObjects;

namespace Taskly.Services.Notification.Infrastructure.Mongo.Documents;

internal static class Extensions
{
    public static Domain.Entities.Notification AsEntity(this NotificationDocument document)
        => new(document.Id, document.CreatedAt, document.LastModifiedAt, document.UserId, document.Name, document.Search, document.Tags, document.SectionId, document.GoalId, document.Schedule.AsEntity());


    public static Schedule AsEntity(this NotificationDocument.ScheduleDocument document)
       => new(document.ReminderTiming, document.ScheduleFrequency, document.CustomOffsetValue, document.SendTime);


    public static NotificationDocument AsDocument(this Domain.Entities.Notification entity)
        => new()
        {
            Id = entity.Id,
            Name = entity.Name,
            UserId = entity.UserId,
            Search = entity.Search,
            Tags = entity.Tags,
            SectionId = entity.SectionId,
            GoalId = entity.GoalId,
            Schedule = entity.Schedule.AsDocument(),
            CreatedAt = entity.CreatedAt,
            LastModifiedAt = entity.LastModifiedAt,
        };

    public static NotificationDocument.ScheduleDocument AsDocument(this Schedule entity)
        => new()
        {
            ReminderTiming = entity.ReminderTiming,
            CustomOffsetValue = entity.CustomOffsetValue,
            SendTime = entity.SendTime,
        };

    public static ScheduleDto AsDto(this NotificationDocument.ScheduleDocument document)
        => new()
        {
            ReminderTiming = document.ReminderTiming,
            ScheduleFrequency = document.ScheduleFrequency,
            CustomOffsetValue = document.CustomOffsetValue,
            SendTime = document.SendTime
        };

    public static NotificationDto AsDto(this NotificationDocument document)
        => new()
        {
            Id = document.Id,
            Name = document.Name,
            UserId = document.UserId,
            Search = document.Search,
            Tags = document.Tags,
            SectionId = document.SectionId,
            GoalId = document.GoalId
        };

    public static NotificationDetailsDto AsDetailsDto(this NotificationDocument document)
        => new()
        {
            Id = document.Id,
            Name = document.Name,
            UserId = document.UserId,
            Search = document.Search,
            Tags = document.Tags,
            SectionId = document.SectionId,
            GoalId = document.GoalId,
            Schedule = document.Schedule.AsDto()
        };

    public static UserDocument AsDocument(this User entity)
        => new()
        {
            Id = entity.Id,
            Email = entity.Email,
            Role = entity.Role,
            LastModifiedAt = entity.LastModifiedAt,
            Permissions = entity.Permissions
        };

    public static User AsEntity(this UserDocument document)
        => new(document.Id, document.Email, Role.From(document.Role), document.CreatedAt, document.LastModifiedAt, document.Permissions);
}