using Taskly.Services.Task.Application.DTO;

namespace Taskly.Services.Task.Infrastructure.Mongo.Documents;

internal static class Extensions
{
    public static Domain.Entities.Task AsEntity(this TaskDocument document)
        => new(document.Id,
            document.UserId,
            document.Name,
            document.Description, 
            document.SectionId,
            document.GoalId,
            document.RootTaskId,
            document.PriorityLevel,
            document.Tags,
            document.Repeatable,
            document.Active, 
            document.Interval?.AsEntity(),
            document.DueDate);

    public static Domain.Entities.TaskInterval AsEntity(this TaskIntervalDocument document)
        => new(document.Interval, document.StartDate, document.Hour);

    public static TaskDocument AsDocument(this Domain.Entities.Task entity)
        => new()
        {
            Id = entity.Id,
            UserId = entity.UserId,
            Name = entity.Name,
            Description = entity.Description,
            SectionId = entity.SectionId,
            GoalId = entity.GoalId,
            RootTaskId = entity.RootTaskId,
            PriorityLevel = entity.PriorityLevel,
            Tags = entity.Tags,
            Repeatable = entity.Repeatable,
            Active = entity.Active,
            Interval = entity.Interval?.AsDocument(),
            DueDate = entity.DueDate,
        };

    public static TaskDto AsDto(this TaskDocument document)
        => new()
        {
            Id = document.Id,
            Name = document.Name,
            PriorityLevel = document.PriorityLevel,
            Repeatable = document.Repeatable,
            Active = document.Active,
        };

    public static TaskDetailsDto AsDetailsDto(this TaskDocument document)
        => new()
        {
            Id = document.Id,
            Name = document.Name,
            Description = document.Description,
            SectionId = document.SectionId,
            GoalId = document.GoalId,
            RootTaskId = document.RootTaskId,
            PriorityLevel = document.PriorityLevel,
            Tags = document.Tags,
            Repeatable = document.Repeatable,
            Active = document.Active,
            Interval = document.Interval?.AsDto(),
            DueDate = document.DueDate,
        };

    public static TaskIntervalDto AsDto(this TaskIntervalDocument document)
        => new()
        {
            Interval = document.Interval,
            StartDate = document.StartDate,
            Hour = document.Hour,
        };

    public static TaskIntervalDocument AsDocument(this Domain.Entities.TaskInterval entity)
        => new()
        {
            Interval = entity.Interval,
            StartDate = entity.StartDate,
            Hour = entity.Hour,
        };
}
