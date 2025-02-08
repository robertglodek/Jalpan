namespace Taskly.Services.Identity.Application.Commands.Validators;

[UsedImplicitly]
public sealed class RevokeRefreshTokenValidator : AbstractValidator<RevokeRefreshToken>
{
    public RevokeRefreshTokenValidator()
    {
        RuleFor(model => model.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required.").WithErrorCode("required");
    }
}