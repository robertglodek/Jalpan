using Jalpan.Attributes;
using Jalpan.Types;

namespace Taskly.Services.Report.Application.Events.External
{
    [Message("tasks", "task_finished", "reports.task_finished")]
    public record TaskFinished(Guid Id, Guid UserId, string Name, bool Success, DateTime DateTime) : IEvent;
}
