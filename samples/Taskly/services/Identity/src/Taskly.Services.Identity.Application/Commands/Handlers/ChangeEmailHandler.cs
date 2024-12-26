using Taskly.Services.Identity.Domain.Repositories;

namespace Taskly.Services.Identity.Application.Commands.Handlers;

[UsedImplicitly]
internal sealed class ChangeEmailHandler(
    IContextProvider contextProvider,
    IUserRepository userRepository,
    IDateTime dateTime,
    ILogger<ChangeEmailHandler> logger) : ICommandHandler<ChangeEmail, Empty>
{
    public async Task<Empty> HandleAsync(ChangeEmail command, CancellationToken cancellationToken = default)
        => await Empty.ExecuteAsync(async () =>
        {
            var context = contextProvider.Current();

            var user = await userRepository.GetAsync(Guid.Parse(context.UserId!));
            user!.UpdateEmail(command.Email);
            user.LastModifiedAt = dateTime.Now;
            await userRepository.UpdateAsync(user);
            
            logger.LogInformation("Changed email for the user with id: {UserId}", user.Id);
        });
}