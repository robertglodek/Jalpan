using Jalpan.Types;
using Taskly.Services.Notification.Domain.Enums;

namespace Taskly.Services.Notification.Application.Commands;

public sealed class CreateNotificationSchedule(string name,
    string? search,
    IEnumerable<string>? tags,
    Guid? sectionId,
    Guid? goalId,
    CreateNotificationSchedule.CreateSchedule schedule)
    : ICommand<Guid>
{
    public string Name { get; } = name;
    public string? Search { get; } = search;
    public IEnumerable<string>? Tags { get; } = tags;
    public Guid? SectionId { get; } = sectionId;
    public Guid? GoalId { get; } = goalId;
    public CreateSchedule Schedule { get; } = schedule;

    public sealed class CreateSchedule(ReminderSchedule reminderSchedule, ScheduleFrequency scheduleFrequency, int? customOffsetValue, TimeSpan sendTime)
    {
        public ReminderSchedule ReminderTiming { get; } = reminderSchedule;
        public ScheduleFrequency ScheduleFrequency { get; set; } = scheduleFrequency;
        public int? CustomOffsetValue { get; } = customOffsetValue;
        public TimeSpan SendTime { get; } = sendTime;
    }
}
