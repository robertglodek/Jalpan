using Taskly.Services.Note.Application.DTO;

namespace Taskly.Services.Note.Application.Queries;

public sealed class GetNote : IQuery<NoteDetailsDto>
{
    public Guid Id { get; set; }
}