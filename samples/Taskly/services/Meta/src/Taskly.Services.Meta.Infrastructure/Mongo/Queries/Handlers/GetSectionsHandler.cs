using Jalpan.Contexts.Providers;
using Jalpan.Handlers;
using Jalpan.Persistence.MongoDB.Repositories;
using Taskly.Services.Meta.Application.DTO;
using Taskly.Services.Meta.Application.Queries;
using Taskly.Services.Meta.Infrastructure.Mongo.Documents;

namespace Taskly.Services.Meta.Infrastructure.Mongo.Queries.Handlers;

[UsedImplicitly]
internal sealed class GetSectionsHandler(
    IMongoDbRepository<SectionDocument, Guid> sectionRepository,
    IContextProvider contextProvider) : IQueryHandler<GetSections, IEnumerable<SectionDto>>
{
    public async Task<IEnumerable<SectionDto>?> HandleAsync(GetSections query, CancellationToken cancellationToken = default)
    {
        var context = contextProvider.Current();

        var users = await sectionRepository.FindAsync(n => n.UserId == Guid.Parse(context.UserId!), cancellationToken);

        return users.Select(n => n.AsDto());
    }
}