namespace Taskly.Services.Identity.Application.Commands.Handlers;

internal sealed class SignUpHandler : ICommandHandler<SignUp, Empty>
{
    public SignUpHandler()
    {

    }

    public Task<Empty> HandleAsync(SignUp command, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
