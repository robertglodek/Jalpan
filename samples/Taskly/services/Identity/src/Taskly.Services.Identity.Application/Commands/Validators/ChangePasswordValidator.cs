namespace Taskly.Services.Identity.Application.Commands.Validators;

[UsedImplicitly]
public sealed class ChangePasswordValidator : AbstractValidator<ChangePassword>
{
    public ChangePasswordValidator()
    {
        RuleFor(model => model.CurrentPassword)
            .NotEmpty().WithMessage("current_password_must_not_be_empty");
        RuleFor(model => model.NewPassword)
            .NotEmpty().WithMessage("new_password_must_not_be_empty");
    }
}