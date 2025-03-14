namespace Taskly.Services.Identity.Application.Events.Rejected;

[PublicContract]
[Message("identity", "user_sign_up_rejected")]
public sealed class SignUpRejected(string email, string reason, string code) : IRejectedEvent
{
    public string Email { get; } = email;
    public string Reason { get; } = reason;
    public string Code { get; } = code;
}