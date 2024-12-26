namespace Taskly.Services.Identity.Application.Commands.Validators;

[UsedImplicitly]
public sealed class UseRefreshTokenValidator : AbstractValidator<UseRefreshToken>
{
    public UseRefreshTokenValidator()
    {
        RuleFor(model => model.RefreshToken)
            .NotEmpty().WithMessage("refresh_token_must_not_be_empty");
    }
}