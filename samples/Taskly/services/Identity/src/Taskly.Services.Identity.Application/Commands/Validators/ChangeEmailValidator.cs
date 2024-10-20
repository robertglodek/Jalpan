using FluentValidation;

namespace Taskly.Services.Identity.Application.Commands.Validators;

internal class ChangeEmailValidator : AbstractValidator<ChangeEmail>
{
    public ChangeEmailValidator()
    {
        RuleFor(model => model.Email)
            .NotEmpty().WithMessage("email_must_not_be_empty")
            .EmailAddress().WithMessage("email_format_is_invalid");
    }
}
