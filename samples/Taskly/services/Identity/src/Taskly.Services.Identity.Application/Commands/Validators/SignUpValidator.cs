using FluentValidation;

namespace Taskly.Services.Identity.Application.Commands.Validators;

internal class SignUpValidator : AbstractValidator<SignUp>
{
    public SignUpValidator()
    {
        RuleFor(model => model.Password)
           .NotEmpty().WithMessage("password_required")
           .MinimumLength(8).WithMessage("password_too_short")
           .Matches(@"[A-Z]").WithMessage("password_uppercase_required")
           .Matches(@"[a-z]").WithMessage("password_lowercase_required")
           .Matches(@"\d").WithMessage("password_digit_required")
           .Matches(@"[^\w\d]").WithMessage("password_special_character_required");

        RuleFor(model => model.Email)
            .NotEmpty().WithMessage("email_must_not_be_empty")
            .EmailAddress().WithMessage("email_format_is_invalid");

        RuleFor(model => model.Role)
            .NotEmpty().WithMessage("role_required");

    } 
}
