namespace Jalpan.Channels.Email.SendGrid;

public sealed class SendGridOptions
{
    public string ApiKey { get; init; } = null!;
    public string EmailFrom { get; set; } = null!;
}
