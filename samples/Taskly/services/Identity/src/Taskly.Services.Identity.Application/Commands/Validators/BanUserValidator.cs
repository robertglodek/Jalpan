namespace Taskly.Services.Identity.Application.Commands.Validators;

[UsedImplicitly]
public sealed class BanUserValidator : AbstractValidator<SetLock>
{
    public BanUserValidator()
    {
        RuleFor(model => model.UserId).NotEmpty().WithMessage("user_id_must_not_be_empty");
    }
}