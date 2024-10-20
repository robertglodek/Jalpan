namespace Taskly.Services.Identity.Application.Commands;

public sealed class ChangeEmail : ICommand<Empty>
{
    public string Email { get; set; }

    public ChangeEmail(string email)
    {
        Email = email;
    }
}
