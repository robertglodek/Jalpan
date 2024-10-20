using Taskly.Services.Identity.Application.DTO;

namespace Taskly.Services.Identity.Application.Commands.Handlers;

internal sealed class SignInHandler : ICommandHandler<SignIn, AuthDto>
{
    public SignInHandler()
    {

    }

    public Task<AuthDto> HandleAsync(SignIn command, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

