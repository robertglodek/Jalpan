using Taskly.Services.Meta.Application.DTO;

namespace Taskly.Services.Meta.Application.Queries;

public sealed class GetTag : IQuery<TagDto>
{
    public Guid Id { get; set; }
}