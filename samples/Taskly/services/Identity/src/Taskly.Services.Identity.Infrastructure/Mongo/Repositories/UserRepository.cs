using Jalpan.Persistence.MongoDB.Repositories;
using Taskly.Services.Identity.Domain.Entities;
using Taskly.Services.Identity.Domain.Repositories;
using Taskly.Services.Identity.Infrastructure.Mongo.Documents;

namespace Taskly.Services.Identity.Infrastructure.Mongo.Repositories;

internal sealed class UserRepository(IMongoDbRepository<UserDocument, Guid> repository) : IUserRepository
{
    public async Task<User?> GetAsync(Guid id)
    {
        var user = await repository.GetAsync(id);

        return user?.AsEntity();
    }

    public async Task<User?> GetAsync(string email)
    {
        var user = await repository.GetAsync(x => x.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase));

        return user?.AsEntity();
    }

    public Task AddAsync(User user) => repository.AddAsync(user.AsDocument());
    public Task UpdateAsync(User user) => repository.UpdateAsync(user.AsDocument());
}