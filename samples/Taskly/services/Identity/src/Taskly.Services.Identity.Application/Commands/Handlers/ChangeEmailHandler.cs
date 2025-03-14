﻿using Taskly.Services.Identity.Domain.Exceptions;
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
            
            var currentUserId = Guid.Parse(context.UserId!);

            // Check if the email is already used by another user
            var userWithSameEmail = await userRepository.GetAsync(command.Email, cancellationToken);
            if (userWithSameEmail is not null && userWithSameEmail.Id != currentUserId)
            {
                throw new EmailInUseException($"The email '{command.Email}' is already in use by another user.");
            }

            var user = await userRepository.GetAsync(currentUserId, cancellationToken);
            user!.UpdateEmail(command.Email);
            user.LastModifiedAt = dateTime.Now;
            await userRepository.UpdateAsync(user, cancellationToken);

            logger.LogInformation("Changed email for the user with id: {UserId}", user.Id);
        });
}