using Jalpan.Pagination;
using Taskly.Services.Note.Application.DTO;

namespace Taskly.Services.Note.Application.Queries;

public sealed class SearchNotes : PagedQueryBase, IQuery<PagedResult<NoteDto>>
{
    public Guid? GoalId { get; set; }
    public Guid? SectionId { get; set; }
    
}