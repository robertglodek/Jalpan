namespace Taskly.Services.Identity.Application.Commands.Validators;

[UsedImplicitly]
public sealed class SignUpValidator : AbstractValidator<SignUp>
{
    public SignUpValidator()
    {
        RuleFor(model => model.Password)
            .NotEmpty().WithMessage("Password is required.").WithErrorCode("required")
            .MinimumLength(8).WithMessage("Password must have at least 8 characters.").WithErrorCode("min_length")
            .Matches("[A-Z]").WithMessage("Uppercase character required.").WithErrorCode("uppercase_required")
            .Matches("[a-z]").WithMessage("Lowercase character required.").WithErrorCode("lowercase_required")
            .Matches(@"\d").WithMessage("Digit required").WithErrorCode("digit_required")
            .Matches(@"[^\w\d]").WithMessage("Special character required").WithErrorCode("special_character_required");

        RuleFor(model => model.Email)
            .NotEmpty().WithMessage("Email is required.").WithErrorCode("required")
            .EmailAddress().WithMessage("Email is invalid.").WithErrorCode("invalid");

        RuleFor(model => model.Role)
            .NotEmpty().WithMessage("Role required.").WithErrorCode("required");
    }
}