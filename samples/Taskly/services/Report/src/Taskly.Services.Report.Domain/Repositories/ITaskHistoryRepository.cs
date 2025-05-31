using Taskly.Services.Report.Domain.Entities;

namespace Taskly.Services.Report.Domain.Repositories;

public interface ITaskHistoryRepository
{
    Task AddAsync(TaskHistory taskHistory, CancellationToken cancellationToken = default);
}
