namespace Taskly.Services.Meta.Application.Commands.Validators;

[UsedImplicitly]
public sealed class AddSectionValidator : AbstractValidator<AddSection>
{
    public AddSectionValidator()
    {
        RuleFor(model => model.Name)
            .NotEmpty().WithMessage("Section name is required.").WithErrorCode("required")
            .MaximumLength(20).WithMessage("Section name maximum length is 20.").WithErrorCode("max_length");
        
        RuleFor(model => model.Description)
            .MaximumLength(200).WithMessage("Section description maximum length is 200.").WithErrorCode("max_length");
    }
}