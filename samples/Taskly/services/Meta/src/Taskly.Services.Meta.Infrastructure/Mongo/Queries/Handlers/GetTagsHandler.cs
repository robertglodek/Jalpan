using Jalpan.Contexts.Providers;
using Jalpan.Handlers;
using Jalpan.Persistence.MongoDB.Repositories;
using Taskly.Services.Meta.Application.DTO;
using Taskly.Services.Meta.Application.Queries;
using Taskly.Services.Meta.Infrastructure.Mongo.Documents;

namespace Taskly.Services.Meta.Infrastructure.Mongo.Queries.Handlers;

[UsedImplicitly]
internal sealed class GetTagsHandler(
    IMongoDbRepository<TagDocument, Guid> tagRepository,
    IContextProvider contextProvider) : IQueryHandler<GetTags, IEnumerable<TagDto>>
{
    public async Task<IEnumerable<TagDto>?> HandleAsync(GetTags query, CancellationToken cancellationToken = default)
    {
        var context = contextProvider.Current();

        var users = await tagRepository.FindAsync(n => n.UserId == Guid.Parse(context.UserId!), cancellationToken);

        return users.Select(n => n.AsDto());
    }
}