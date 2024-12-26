namespace Taskly.Services.Identity.Application.Commands.Validators;

[UsedImplicitly]
public sealed class RevokeRefreshTokenValidator : AbstractValidator<RevokeRefreshToken>
{
    public RevokeRefreshTokenValidator()
    {
        RuleFor(model => model.RefreshToken)
            .NotEmpty().WithMessage("refresh_token_must_not_be_empty");
    }
}