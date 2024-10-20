using Taskly.Services.Identity.Application.DTO;

namespace Taskly.Services.Identity.Application.Commands.Handlers;

internal sealed class UseRefreshTokenHandler : ICommandHandler<UseRefreshToken, AuthDto>
{
    public UseRefreshTokenHandler()
    {

    }

    public Task<AuthDto> HandleAsync(UseRefreshToken command, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
