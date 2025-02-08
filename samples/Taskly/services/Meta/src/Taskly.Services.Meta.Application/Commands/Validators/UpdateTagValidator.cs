namespace Taskly.Services.Meta.Application.Commands.Validators;

[UsedImplicitly]
public sealed class UpdateTagValidator : AbstractValidator<UpdateTag>
{
    public UpdateTagValidator()
    {
        RuleFor(model => model.Name)
            .NotEmpty().WithMessage("Tag name is required.").WithErrorCode("required")
            .MaximumLength(15).WithMessage("Tag name maximum length is 15.").WithErrorCode("max_length");
    }
}