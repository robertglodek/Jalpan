namespace Taskly.Services.Identity.Application.Commands;

public sealed class SignUp : ICommand<Empty>
{
    public Guid UserId { get; }
    public string Email { get; } = null!;
    public string Password { get; } = null!;
    public string Role { get; } = null!;

    public SignUp(Guid userId, string email, string password, string role)
    {
        UserId = userId == Guid.Empty ? Guid.NewGuid() : userId;
        Email = email;
        Password = password;
        Role = role;
    }
}
