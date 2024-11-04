using Jalpan.Contexts.Providers;
using Jalpan.Time;
using Taskly.Services.Identity.Application.Exceptions;
using Taskly.Services.Identity.Application.Services;
using Taskly.Services.Identity.Core.Exceptions;

namespace Taskly.Services.Identity.Application.Commands.Handlers;

internal sealed class ChangePasswordHandler(IContextProvider contextProvider, IUserRepository userRepository, IDateTime dateTime, IPasswordService passwordService)
    : ICommandHandler<ChangePassword, Empty>
{
    private readonly IContextProvider _contextProvider = contextProvider;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IDateTime _dateTime = dateTime;
    private readonly IPasswordService _passwordService = passwordService;

    public async Task<Empty> HandleAsync(ChangePassword command, CancellationToken cancellationToken = default)
        => await Empty.ExecuteAsync(async () =>
        {
            var context = _contextProvider.Current();
            if (string.IsNullOrEmpty(context.UserId) || !Guid.TryParse(context.UserId, out var userId))
            {
                throw new UserUnauthorizedException();
            }

            var user = await _userRepository.GetAsync(userId) ?? throw new UserNotFoundException(userId);
            if (!_passwordService.IsValid(user.Password, command.CurrentPassword))
            {          
                throw new InvalidCredentialsException();
            }

            var password = _passwordService.Hash(command.NewPassword);
            user.UpdatePassword(password, _dateTime.Now);
            await _userRepository.UpdateAsync(user);
        });
}