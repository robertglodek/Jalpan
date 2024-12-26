namespace Taskly.Services.Identity.Application.DTO;

public sealed class RefreshTokenDto
{
    public string Token { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
}