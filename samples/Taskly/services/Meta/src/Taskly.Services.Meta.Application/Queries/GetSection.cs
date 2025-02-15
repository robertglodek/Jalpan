using Taskly.Services.Meta.Application.DTO;

namespace Taskly.Services.Meta.Application.Queries;

public sealed class GetSection : IQuery<SectionDto>
{
    public Guid Id { get; set; }
}