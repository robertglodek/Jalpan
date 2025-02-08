namespace Taskly.Services.Identity.Application.Commands.Validators;

[UsedImplicitly]
public sealed class RevokeAccessTokenValidator : AbstractValidator<RevokeAccessToken>
{
    public RevokeAccessTokenValidator()
    {
        RuleFor(model => model.AccessToken)
            .NotEmpty().WithMessage("Access token is required.").WithErrorCode("required");
    }
}