using Jalpan.Persistence.MongoDB.Repositories;
using Taskly.Services.Meta.Domain.Entities;
using Taskly.Services.Meta.Domain.Repositories;
using Taskly.Services.Meta.Infrastructure.Mongo.Documents;

namespace Taskly.Services.Meta.Infrastructure.Mongo.Repositories;

internal sealed class SectionRepository(IMongoDbRepository<SectionDocument, Guid> repository) : ISectionRepository
{
    public async Task<Section?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var section = await repository.GetAsync(id, cancellationToken);
        return section?.AsEntity();
    }

    public async Task AddAsync(Section section, CancellationToken cancellationToken = default)
        => await repository.AddAsync(section.AsDocument(), cancellationToken: cancellationToken);

    public async Task UpdateAsync(Section section, CancellationToken cancellationToken = default)
        => await repository.UpdateAsync(section.AsDocument(), cancellationToken);

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        => await repository.DeleteAsync(id, cancellationToken: cancellationToken);
}