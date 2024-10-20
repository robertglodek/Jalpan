using Taskly.Services.Identity.Application.DTO;

namespace Taskly.Services.Identity.Application.Queries;

public sealed class GetUser : IQuery<UserDto>
{
    public Guid UserId { get; set; }
}
