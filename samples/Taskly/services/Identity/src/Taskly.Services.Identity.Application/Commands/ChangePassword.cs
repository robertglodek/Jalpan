﻿namespace Taskly.Services.Identity.Application.Commands;

[UsedImplicitly]
public sealed class ChangePassword(string currentPassword, string newPassword) : ICommand<Empty>
{
    public string CurrentPassword { get; } = currentPassword;
    public string NewPassword { get; } = newPassword;
}
