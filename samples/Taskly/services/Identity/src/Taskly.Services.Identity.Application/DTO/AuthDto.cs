namespace Taskly.Services.Identity.Application.DTO;

public sealed class AuthDto
{
    public AccessTokenDto AccessToken { get; set; } = null!;
    public RefreshTokenDto RefreshToken { get; set; } = null!;
}
