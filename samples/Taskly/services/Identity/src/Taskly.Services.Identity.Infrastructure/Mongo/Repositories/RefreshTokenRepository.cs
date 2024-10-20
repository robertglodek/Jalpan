using Taskly.Services.Identity.Core.Entities;
using Taskly.Services.Identity.Core.Exceptions;
using Taskly.Services.Identity.Infrastructure.Mongo.Documents;

namespace Taskly.Services.Identity.Infrastructure.Mongo.Repositories;

internal sealed class RefreshTokenRepository(IMongoDbRepository<RefreshTokenDocument, Guid> repository) : IRefreshTokenRepository
{
    private readonly IMongoDbRepository<RefreshTokenDocument, Guid> _repository = repository;

    public Task AddAsync(RefreshToken token) => _repository.AddAsync(token.AsDocument());
    public Task UpdateAsync(RefreshToken token) => _repository.UpdateAsync(token.AsDocument());
    public async Task<RefreshToken?> GetAsync(string token)
    {
        var refreshToken = await _repository.GetAsync(x => x.Token == token);

        return refreshToken?.AsEntity();
    }
}