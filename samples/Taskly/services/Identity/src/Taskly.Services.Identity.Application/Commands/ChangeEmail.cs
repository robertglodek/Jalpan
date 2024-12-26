namespace Taskly.Services.Identity.Application.Commands;

[UsedImplicitly]
public sealed class ChangeEmail(string email) : ICommand<Empty>
{
    public string Email { get; set; } = email;
}
