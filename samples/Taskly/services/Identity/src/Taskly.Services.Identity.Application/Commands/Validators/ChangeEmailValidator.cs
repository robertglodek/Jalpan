namespace Taskly.Services.Identity.Application.Commands.Validators;

[UsedImplicitly]
public sealed class ChangeEmailValidator : AbstractValidator<ChangeEmail>
{
    public ChangeEmailValidator()
    {
        RuleFor(model => model.Email)
            .NotEmpty().WithMessage("Email must not be empty.").WithErrorCode("required")
            .EmailAddress().WithMessage("Email format is not valid.").WithErrorCode("required");
    }
}