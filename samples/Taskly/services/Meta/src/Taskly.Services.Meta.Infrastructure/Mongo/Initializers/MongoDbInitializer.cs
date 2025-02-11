using Jalpan;
using Jalpan.Persistence.MongoDB;
using Jalpan.Persistence.MongoDB.Repositories;
using Taskly.Services.Meta.Infrastructure.Mongo.Documents;

namespace Taskly.Services.Meta.Infrastructure.Mongo.Initializers;

[UsedImplicitly]
internal sealed class MongoDbInitializer(IMongoDbRepository<GoalDocument, Guid> goalsRepository,
    IMongoDbRepository<SectionDocument, Guid> sectionsRepository,
    IMongoDbRepository<TagDocument, Guid> tagsRepository) : IInitializer
{
    public async Task InitializeAsync()
    {
        await InitializeGoalsAsync();
        await InitializeSectionsAsync();
        await InitializeTagsAsync();
    }

    private async Task InitializeGoalsAsync()
    {
        await goalsRepository.Collection.CreateIndexAsync(false, u => u.UserId);
    }
    
    private async Task InitializeSectionsAsync()
    {
        await sectionsRepository.Collection.CreateIndexAsync(false, u => u.UserId);
    }
    
    private async Task InitializeTagsAsync()
    {
        await tagsRepository.Collection.CreateIndexAsync(false, u => u.UserId);
    }
}