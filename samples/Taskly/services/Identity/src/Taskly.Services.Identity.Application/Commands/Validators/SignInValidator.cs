namespace Taskly.Services.Identity.Application.Commands.Validators;

[UsedImplicitly]
public sealed class SignInValidator : AbstractValidator<SignIn>
{
    public SignInValidator()
    {
        RuleFor(model => model.Email)
            .NotEmpty().WithMessage("Email is required.").WithErrorCode("required")
            .EmailAddress().WithMessage("Email is invalid.").WithErrorCode("invalid");

        RuleFor(model => model.Password)
            .NotEmpty().WithMessage("Password is required.").WithErrorCode("required");
    }
}