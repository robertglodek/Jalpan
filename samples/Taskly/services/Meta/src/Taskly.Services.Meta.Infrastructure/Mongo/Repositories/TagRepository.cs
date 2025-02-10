using Jalpan.Persistence.MongoDB.Repositories;
using Taskly.Services.Meta.Domain.Entities;
using Taskly.Services.Meta.Domain.Repositories;
using Taskly.Services.Meta.Infrastructure.Mongo.Documents;

namespace Taskly.Services.Meta.Infrastructure.Mongo.Repositories;

internal sealed class TagRepository(IMongoDbRepository<TagDocument, Guid> repository) : ITagRepository
{
    public async Task<Tag?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var tag = await repository.GetAsync(id, cancellationToken);
        return tag?.AsEntity();
    }

    public async Task AddAsync(Tag tag, CancellationToken cancellationToken = default)
        => await repository.AddAsync(tag.AsDocument(), cancellationToken: cancellationToken);

    public async Task UpdateAsync(Tag tag, CancellationToken cancellationToken = default)
        => await repository.UpdateAsync(tag.AsDocument(), cancellationToken);

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        => await repository.DeleteAsync(id, cancellationToken: cancellationToken);
}