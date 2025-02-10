using Jalpan.Persistence.MongoDB.Repositories;
using Taskly.Services.Identity.Domain.Entities;
using Taskly.Services.Identity.Domain.Repositories;
using Taskly.Services.Identity.Infrastructure.Mongo.Documents;

namespace Taskly.Services.Identity.Infrastructure.Mongo.Repositories;

internal sealed class RefreshTokenRepository(IMongoDbRepository<RefreshTokenDocument, Guid> repository)
    : IRefreshTokenRepository
{
    public Task AddAsync(RefreshToken token, CancellationToken cancellationToken = default) =>
        repository.AddAsync(token.AsDocument(), cancellationToken: cancellationToken);

    public Task UpdateAsync(RefreshToken token, CancellationToken cancellationToken = default) =>
        repository.UpdateAsync(token.AsDocument(), cancellationToken);

    public async Task<RefreshToken?> GetAsync(string token, CancellationToken cancellationToken = default)
    {
        var refreshToken = await repository.GetAsync(x => x.Token == token, cancellationToken);

        return refreshToken?.AsEntity();
    }
}