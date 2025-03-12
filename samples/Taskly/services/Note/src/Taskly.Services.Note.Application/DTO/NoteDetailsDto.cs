namespace Taskly.Services.Note.Application.DTO;

public sealed class NoteDetailsDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = null!;
    public string? Content { get; set; }
    public IEnumerable<LinkDto> Links { get; set; } = null!;
}