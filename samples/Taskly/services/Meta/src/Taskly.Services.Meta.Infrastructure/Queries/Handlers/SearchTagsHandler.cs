using Jalpan.Contexts.Providers;
using Jalpan.Handlers;
using Jalpan.Pagination;
using Jalpan.Persistence.MongoDB.Repositories;
using Taskly.Services.Meta.Application.DTO;
using Taskly.Services.Meta.Application.Queries;
using Taskly.Services.Meta.Infrastructure.Mongo.Documents;

namespace Taskly.Services.Meta.Infrastructure.Queries.Handlers;

[UsedImplicitly]
internal sealed class SearchTagsHandler(
    IMongoDbRepository<TagDocument, Guid> tagRepository,
    IContextProvider contextProvider) : IQueryHandler<SearchTags, PagedResult<TagDto>>
{
    public async Task<PagedResult<TagDto>?> HandleAsync(SearchTags query, CancellationToken cancellationToken = default)
    {
        var context = contextProvider.Current();

        var pagedResult = await tagRepository.BrowseAsync(n => n.UserId == Guid.Parse(context.UserId!), query, cancellationToken);

        return pagedResult.Map(d => d.AsDto());
    }
}