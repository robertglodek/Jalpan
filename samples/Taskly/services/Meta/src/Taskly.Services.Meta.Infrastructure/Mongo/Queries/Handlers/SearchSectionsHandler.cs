using Jalpan.Contexts.Providers;
using Jalpan.Handlers;
using Jalpan.Pagination;
using Jalpan.Persistence.MongoDB.Repositories;
using Taskly.Services.Meta.Application.DTO;
using Taskly.Services.Meta.Application.Queries;
using Taskly.Services.Meta.Infrastructure.Mongo.Documents;

namespace Taskly.Services.Meta.Infrastructure.Mongo.Queries.Handlers;

[UsedImplicitly]
internal sealed class SearchSectionsHandler(
    IMongoDbRepository<SectionDocument, Guid> sectionRepository,
    IContextProvider contextProvider) : IQueryHandler<SearchSections, PagedResult<SectionDto>>
{
    public async Task<PagedResult<SectionDto>?> HandleAsync(SearchSections query,
        CancellationToken cancellationToken = default)
    {
        var context = contextProvider.Current();
        
        PagedResult<SectionDocument> pagedResult;
        if (query.GoalId.HasValue)
        {
            pagedResult = await sectionRepository.BrowseAsync(
                n => n.UserId == Guid.Parse(context.UserId!) && n.GoalId == query.GoalId, query, cancellationToken);
        }
        else
        {
            pagedResult = await sectionRepository.BrowseAsync(n => n.UserId == Guid.Parse(context.UserId!), query,
                cancellationToken);
        }

        return pagedResult.Map(d => d.AsDto());
    }
}