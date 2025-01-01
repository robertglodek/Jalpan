using Taskly.Services.Identity.Application.Context;
using Taskly.Services.Identity.Domain.Exceptions;
using Taskly.Services.Identity.Domain.Repositories;

namespace Taskly.Services.Identity.Application.Commands.Handlers;

[UsedImplicitly]
internal sealed class ChangeEmailHandler(
    IContextProvider contextProvider,
    IUserRepository userRepository,
    IDateTime dateTime,
    IDataContextProvider<IdentityDataContext> dataContextProvider,
    ILogger<ChangeEmailHandler> logger) : ICommandHandler<ChangeEmail, Empty>
{
    public async Task<Empty> HandleAsync(ChangeEmail command, CancellationToken cancellationToken = default)
        => await Empty.ExecuteAsync(async () =>
        {
            var context = contextProvider.Current();
            
            var dataContext = dataContextProvider.Current();
            
            
            var currentUserId = Guid.Parse(context.UserId!);

            // Check if the email is already used by another user
            var userWithSameEmail = await userRepository.GetAsync(command.Email);
            if (userWithSameEmail is not null && userWithSameEmail.Id != currentUserId)
            {
                throw new EmailInUseException($"The email '{command.Email}' is already in use by another user.");
            }

            var user = await userRepository.GetAsync(currentUserId);
            user!.UpdateEmail(command.Email);
            user.LastModifiedAt = dateTime.Now;
            await userRepository.UpdateAsync(user);

            logger.LogInformation("Changed email for the user with id: {UserId}", user.Id);
        });
}