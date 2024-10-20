namespace Taskly.Services.Identity.Application.Commands;

public sealed class ChangePassword : ICommand<Empty>
{
    public Guid UserId { get; set; }
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }

    public ChangePassword(Guid userId, string currentPassword, string newPassword)
    {
        UserId = userId;
        CurrentPassword = currentPassword;
        NewPassword = newPassword;
    }
}
