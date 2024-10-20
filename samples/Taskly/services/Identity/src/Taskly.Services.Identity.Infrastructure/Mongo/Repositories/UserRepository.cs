using Taskly.Services.Identity.Core.Entities;
using Taskly.Services.Identity.Core.Exceptions;
using Taskly.Services.Identity.Infrastructure.Mongo.Documents;

namespace Taskly.Services.Identity.Infrastructure.Mongo.Repositories;

internal sealed  class UserRepository(IMongoDbRepository<UserDocument, Guid> repository) : IUserRepository
{
    private readonly IMongoDbRepository<UserDocument, Guid> _repository = repository;

    public async Task<User?> GetAsync(Guid id)
    {
        var user = await _repository.GetAsync(id);

        return user?.AsEntity();
    }

    public async Task<User?> GetAsync(string email)
    {
        var user = await _repository.GetAsync(x => x.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase));

        return user?.AsEntity();
    }

    public Task AddAsync(User user) => _repository.AddAsync(user.AsDocument());
    public Task UpdateAsync(User user) => _repository.UpdateAsync(user.AsDocument());
}