using Jalpan.Contexts.Providers;
using Jalpan.Handlers;
using Jalpan.Persistence.MongoDB.Repositories;
using Taskly.Services.Meta.Application.DTO;
using Taskly.Services.Meta.Application.Queries;
using Taskly.Services.Meta.Infrastructure.Mongo.Documents;

namespace Taskly.Services.Meta.Infrastructure.Queries.Handlers;

[UsedImplicitly]
internal sealed class GetTagHandler(
    IMongoDbRepository<TagDocument, Guid> tagRepository,
    IContextProvider contextProvider) : IQueryHandler<GetTag, TagDto>
{
    public async Task<TagDto?> HandleAsync(GetTag query, CancellationToken cancellationToken = default)
    {
        var context = contextProvider.Current();
        var tag = await tagRepository.GetAsync(n => n.UserId == Guid.Parse(context.UserId!) && n.Id == query.Id,
            cancellationToken);

        return tag?.AsDto();
    }
}