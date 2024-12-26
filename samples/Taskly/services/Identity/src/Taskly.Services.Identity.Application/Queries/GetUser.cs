using Taskly.Services.Identity.Application.DTO;

namespace Taskly.Services.Identity.Application.Queries;

[UsedImplicitly]
public sealed class GetUser : IQuery<UserDetailsDto>
{
    public Guid UserId { get; set; }
}
