namespace Taskly.Services.Note.Application.Commands.Validators;

[UsedImplicitly]
public sealed class UpsertNoteLinkValidator : AbstractValidator<UpsertNoteLink>
{
    public UpsertNoteLinkValidator()
    {
        RuleFor(model => model.Url)
            .NotEmpty().WithMessage("Note link url is required.").WithErrorCode("required")
            .MaximumLength(50).WithMessage("Note name maximum length is 50.").WithErrorCode("max_length");

        RuleFor(model => model.Name)
            .MaximumLength(20).When(u => !string.IsNullOrEmpty(u.Name))
            .WithMessage("Note content maximum length is 5000.").WithErrorCode("max_length");
    }
}