namespace Taskly.Services.Identity.Application.Commands.Validators;

[UsedImplicitly]
public sealed class ChangePasswordValidator : AbstractValidator<ChangePassword>
{
    public ChangePasswordValidator()
    {
        RuleFor(model => model.CurrentPassword)
            .NotEmpty().WithMessage("Current password is required.").WithErrorCode("required");
        RuleFor(model => model.NewPassword)
            .NotEmpty().WithMessage("New password is required.").WithErrorCode("required");
    }
}