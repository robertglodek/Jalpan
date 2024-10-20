using Jalpan.Contexts.Providers;
using Jalpan.Time;
using Taskly.Services.Identity.Application.Exceptions;
using Taskly.Services.Identity.Core.Exceptions;

namespace Taskly.Services.Identity.Application.Commands.Handlers;

internal sealed class ChangePasswordHandler(IContextProvider contextProvider, IUserRepository userRepository, IDateTime dateTime) : ICommandHandler<ChangePassword, Empty>
{
    private readonly IContextProvider _contextProvider = contextProvider;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IDateTime _dateTime = dateTime;

    public async Task<Empty> HandleAsync(ChangePassword command, CancellationToken cancellationToken = default)
        => await Empty.ExecuteAsync(async () =>
        {
            var context = _contextProvider.Current();
            if (string.IsNullOrEmpty(context.UserId) || !Guid.TryParse(context.UserId, out var userId))
            {
                throw new UserUnauthorizedException();
            }

            var user = await _userRepository.GetAsync(userId) ?? throw new UserNotFoundException(userId);
            user.UpdatePassword(command.pa, _dateTime.Now);
            await _userRepository.UpdateAsync(user);
        });
}