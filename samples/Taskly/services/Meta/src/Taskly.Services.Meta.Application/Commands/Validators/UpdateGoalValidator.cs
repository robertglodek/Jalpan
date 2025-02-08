namespace Taskly.Services.Meta.Application.Commands.Validators;

[UsedImplicitly]
public sealed class UpdateGoalValidator : AbstractValidator<UpdateGoal>
{
    public UpdateGoalValidator()
    {
        RuleFor(model => model.Name)
            .NotEmpty().WithMessage("Goal name is required.").WithErrorCode("required")
            .MaximumLength(20).WithMessage("Goal name maximum length is 20.").WithErrorCode("max_length");
        
        RuleFor(model => model.Description)
            .MaximumLength(200).WithMessage("Goal description maximum length is 200.").WithErrorCode("max_length");
    }
}