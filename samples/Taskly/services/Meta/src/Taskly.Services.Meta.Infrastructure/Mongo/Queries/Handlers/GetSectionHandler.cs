using Jalpan.Contexts.Providers;
using Jalpan.Handlers;
using Jalpan.Persistence.MongoDB.Repositories;
using Taskly.Services.Meta.Application.DTO;
using Taskly.Services.Meta.Application.Queries;
using Taskly.Services.Meta.Infrastructure.Mongo.Documents;

namespace Taskly.Services.Meta.Infrastructure.Mongo.Queries.Handlers;

[UsedImplicitly]
internal sealed class GetSectionHandler(
    IMongoDbRepository<SectionDocument, Guid> sectionRepository,
    IContextProvider contextProvider) : IQueryHandler<GetSection, SectionDto>
{
    public async Task<SectionDto?> HandleAsync(GetSection query, CancellationToken cancellationToken = default)
    {
        var context = contextProvider.Current();
        var section = await sectionRepository.GetAsync(n => n.UserId == Guid.Parse(context.UserId!) && n.Id == query.Id,
            cancellationToken);

        return section?.AsDto();
    }
}