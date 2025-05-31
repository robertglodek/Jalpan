using FluentValidation;
using Jalpan.Time;
using JetBrains.Annotations;

namespace Taskly.Services.Task.Application.Commands.Validators;

[UsedImplicitly]
public sealed class UpdateTaskValidator : AbstractValidator<UpdateTask>
{
    public UpdateTaskValidator(IDateTime dateTime)
    {
        RuleFor(model => model.Name)
            .NotEmpty().WithMessage("Task name is required.").WithErrorCode("required")
            .MaximumLength(30).WithMessage("Task name maximum length is 30.").WithErrorCode("max_length");

        RuleFor(model => model.Description)
            .MaximumLength(500).WithMessage("Task description maximum length is 500.").WithErrorCode("max_length");

        RuleFor(model => model.Tags)
            .Must(tags => tags == null || tags.Count() <= 5)
            .WithMessage("You can assign at most 5 tags.").WithErrorCode("max_count");

        RuleFor(model => new { model.Interval, model.DueDate })
            .Must(x => !(x.Interval != null && x.DueDate != null))
            .WithMessage("Only one of Interval or DueDate can be set.").WithErrorCode("conflict");

        When(model => model.Interval != null, () =>
        {
            RuleFor(model => model.Interval!.StartDate)
                .Must(startDate => startDate == null || startDate >= dateTime.Now)
                .WithMessage("Start date must be in the future or today.").WithErrorCode("invalid_date");
        });
    }
}
