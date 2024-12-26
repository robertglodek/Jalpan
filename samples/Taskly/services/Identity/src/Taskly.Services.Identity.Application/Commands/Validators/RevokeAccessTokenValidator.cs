namespace Taskly.Services.Identity.Application.Commands.Validators;

[UsedImplicitly]
public sealed class RevokeAccessTokenValidator : AbstractValidator<RevokeAccessToken>
{
    public RevokeAccessTokenValidator()
    {
        RuleFor(model => model.AccessToken)
            .NotEmpty().WithMessage("access_token_must_not_be_empty");
    }
}