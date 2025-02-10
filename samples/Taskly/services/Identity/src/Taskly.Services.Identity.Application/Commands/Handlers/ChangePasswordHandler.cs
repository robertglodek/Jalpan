using Taskly.Services.Identity.Application.Services;
using Taskly.Services.Identity.Domain.Exceptions;
using Taskly.Services.Identity.Domain.Repositories;

namespace Taskly.Services.Identity.Application.Commands.Handlers;

[UsedImplicitly]
internal sealed class ChangePasswordHandler(
    IContextProvider contextProvider,
    IUserRepository userRepository,
    IDateTime dateTime,
    IPasswordService passwordService,
    ILogger<ChangePasswordHandler> logger)
    : ICommandHandler<ChangePassword, Empty>
{
    public async Task<Empty> HandleAsync(ChangePassword command, CancellationToken cancellationToken = default)
        => await Empty.ExecuteAsync(async () =>
        {
            var context = contextProvider.Current();

            var user = await userRepository.GetAsync(Guid.Parse(context.UserId!), cancellationToken);
            if (!passwordService.IsValid(user!.Password, command.CurrentPassword))
            {
                throw new InvalidCredentialsException(user.Email);
            }

            var password = passwordService.Hash(command.NewPassword);
            user.UpdatePassword(password);
            user.LastModifiedAt = dateTime.Now;
            await userRepository.UpdateAsync(user, cancellationToken);

            logger.LogInformation("Changed password for the user with id: {UserId}", user.Id);
        });
}