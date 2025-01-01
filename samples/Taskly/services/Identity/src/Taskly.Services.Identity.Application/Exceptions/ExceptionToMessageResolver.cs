using Jalpan.Messaging.Exceptions;
using Taskly.Services.Identity.Application.Commands;
using Taskly.Services.Identity.Application.Events.Rejected;
using Taskly.Services.Identity.Domain.Exceptions;

namespace Taskly.Services.Identity.Application.Exceptions;

[UsedImplicitly]
internal sealed class ExceptionToMessageResolver : IExceptionToMessageResolver
{
    public IRejectedEvent? Map(ICommand command, Exception exception)
        => exception switch

        {
            EmailInUseException ex => new SignUpRejected(ex.Email, ex.Message, ex.Code),
            InvalidCredentialsException ex => new SignInRejected(ex.Email, ex.Message, ex.Code),
            InvalidEmailException ex => command switch
            {
                SignIn cmd => new SignInRejected(cmd.Email, ex.Message, ex.Code),
                SignUp cmd => new SignUpRejected(cmd.Email, ex.Message, ex.Code),
                _ => null
            },
            _ => null
        };
}