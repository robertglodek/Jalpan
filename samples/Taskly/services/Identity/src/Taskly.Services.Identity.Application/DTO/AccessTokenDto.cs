namespace Taskly.Services.Identity.Application.DTO;

public sealed class AccessTokenDto
{
    public string Token { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
}