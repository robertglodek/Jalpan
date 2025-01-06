namespace Taskly.Services.Identity.Application.Events.Rejected;

[PublicContract]
[Message("identity", "user_sign_in_rejected")]
public sealed class SignInRejected(string email, string reason, string code) : IRejectedEvent
{
    public string Email { get; } = email;
    public string Reason { get; } = reason;
    public string Code { get; } = code;
}