namespace Taskly.Services.Identity.Application.Commands.Validators;

[UsedImplicitly]
public sealed class SignInValidator : AbstractValidator<SignIn>
{
    public SignInValidator()
    {
        RuleFor(model => model.Email)
            .NotEmpty().WithMessage("email_must_not_be_empty")
            .EmailAddress().WithMessage("email_format_is_invalid");

        RuleFor(model => model.Password)
            .NotEmpty().WithMessage("password_must_not_be_empty");
    }
}