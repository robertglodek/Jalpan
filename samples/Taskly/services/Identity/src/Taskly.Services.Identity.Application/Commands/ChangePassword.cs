namespace Taskly.Services.Identity.Application.Commands;

public sealed class ChangePassword : ICommand<Empty>
{
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }

    public ChangePassword(string currentPassword, string newPassword)
    {
        CurrentPassword = currentPassword;
        NewPassword = newPassword;
    }
}
