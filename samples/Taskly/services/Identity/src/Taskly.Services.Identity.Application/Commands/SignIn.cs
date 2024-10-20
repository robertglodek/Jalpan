using Taskly.Services.Identity.Application.DTO;

namespace Taskly.Services.Identity.Application.Commands;

public sealed class SignIn : ICommand<AuthDto>
{
    public string Email { get; set; }
    public string Password { get; set; }

    public SignIn(string email, string password)
    {
        Email = email;
        Password = password;
    }
}
