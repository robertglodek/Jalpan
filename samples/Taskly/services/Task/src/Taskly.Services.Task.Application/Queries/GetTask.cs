using Jalpan.Types;
using Taskly.Services.Task.Application.DTO;

namespace Taskly.Services.Task.Application.Queries;

public sealed class GetTask(Guid id) : IQuery<TaskDetailsDto>
{
    public Guid Id { get; } = id;
}
