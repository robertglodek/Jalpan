namespace Taskly.Services.Note.Application.Commands.Validators;

[UsedImplicitly]
public sealed class AddNoteValidator : AbstractValidator<AddNote>
{
    public AddNoteValidator()
    {
        RuleFor(model => model.Name)
            .NotEmpty().WithMessage("Note name is required.").WithErrorCode("required")
            .MaximumLength(20).WithMessage("Note name maximum length is 20.").WithErrorCode("max_length");

        RuleFor(model => model.Content)
            .NotEmpty().WithMessage("Note content is required.").WithErrorCode("required")
            .MaximumLength(5000).WithMessage("Note content maximum length is 5000.").WithErrorCode("max_length");
        
        RuleForEach(u => u.Links)
            .SetValidator(new UpsertNoteLinkValidator())
            .When(u => u.Links != null);
    }
}