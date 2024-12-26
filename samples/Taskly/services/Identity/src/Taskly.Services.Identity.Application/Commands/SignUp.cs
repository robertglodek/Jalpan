namespace Taskly.Services.Identity.Application.Commands;

[UsedImplicitly]
public sealed class SignUp(Guid userId, string email, string password, string role, IEnumerable<string> permissions)
    : ICommand<Empty>
{
    public Guid UserId { get; } = userId == Guid.Empty ? Guid.NewGuid() : userId;
    public string Email { get; } = email;
    public string Password { get; } = password;
    public string Role { get; } = role;
    public IEnumerable<string> Permissions { get; } = permissions;
}
