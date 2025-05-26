using Jalpan.Persistence.MongoDB.Repositories;
using Taskly.Services.Task.Domain.Repositories;
using Taskly.Services.Task.Infrastructure.Mongo.Documents;

namespace Taskly.Services.Task.Infrastructure.Mongo.Repositories
{
    internal sealed class TaskRepository(IMongoDbRepository<TaskDocument, Guid> repository) : ITaskRepository
    {
        public async System.Threading.Tasks.Task AddAsync(Domain.Entities.Task task, CancellationToken cancellationToken = default)
         => await repository.AddAsync(task.AsDocument(), cancellationToken: cancellationToken);

        public async Task<Domain.Entities.Task?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var task = await repository.GetAsync(id, cancellationToken);
            return task?.AsEntity();
        }

        public async System.Threading.Tasks.Task UpdateAsync(Domain.Entities.Task task, CancellationToken cancellationToken = default)
         => await repository.UpdateAsync(task.AsDocument(), cancellationToken);

        public async System.Threading.Tasks.Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
            => await repository.DeleteAsync(id, cancellationToken: cancellationToken);

    }
}
