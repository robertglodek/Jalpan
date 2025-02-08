namespace Taskly.Services.Identity.Application.Commands.Validators;

[UsedImplicitly]
public sealed class UseRefreshTokenValidator : AbstractValidator<UseRefreshToken>
{
    public UseRefreshTokenValidator()
    {
        RuleFor(model => model.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required.").WithErrorCode("required");
    }
}