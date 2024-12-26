using Taskly.Services.Identity.Application.DTO;

namespace Taskly.Services.Identity.Application.Commands;

[UsedImplicitly]
public sealed class SignIn(string email, string password) : ICommand<AuthDto>
{
    public string Email { get; set; } = email;
    public string Password { get; set; } = password;
}
