using Taskly.Services.Task.Domain.Enums;

namespace Taskly.Services.Task.Application.DTO;

public sealed class TaskIntervalDto
{
    public Interval Interval { get; set; }
    public DateTime? StartDate { get; set; }
    public TimeSpan? Hour { get; set; }
}
