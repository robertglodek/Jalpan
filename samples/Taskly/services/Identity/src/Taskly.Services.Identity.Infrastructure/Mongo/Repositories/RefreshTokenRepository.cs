using Jalpan.Persistence.MongoDB.Repositories;
using Taskly.Services.Identity.Domain.Entities;
using Taskly.Services.Identity.Domain.Repositories;
using Taskly.Services.Identity.Infrastructure.Mongo.Documents;

namespace Taskly.Services.Identity.Infrastructure.Mongo.Repositories;

internal sealed class RefreshTokenRepository(IMongoDbRepository<RefreshTokenDocument, Guid> repository)
    : IRefreshTokenRepository
{
    public Task AddAsync(RefreshToken token) => repository.AddAsync(token.AsDocument());
    public Task UpdateAsync(RefreshToken token) => repository.UpdateAsync(token.AsDocument());

    public async Task<RefreshToken?> GetAsync(string token)
    {
        var refreshToken = await repository.GetAsync(x => x.Token == token);

        return refreshToken?.AsEntity();
    }
}