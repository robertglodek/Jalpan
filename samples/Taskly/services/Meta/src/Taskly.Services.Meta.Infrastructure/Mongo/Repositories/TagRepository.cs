using Jalpan.Persistence.MongoDB.Repositories;
using Taskly.Services.Meta.Domain.Repositories;
using Taskly.Services.Meta.Infrastructure.Mongo.Documents;

namespace Taskly.Services.Meta.Infrastructure.Mongo.Repositories;

internal sealed class TagRepository(IMongoDbRepository<TagDocument, Guid> repository) : ITagRepository
{
   
}