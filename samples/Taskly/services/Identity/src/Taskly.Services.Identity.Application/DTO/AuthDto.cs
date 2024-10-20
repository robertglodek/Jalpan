namespace Taskly.Services.Identity.Application.DTO;

public sealed class AuthDto
{
    public TokenDto AccessToken { get; set; } = null!;
    public TokenDto RefreshToken { get; set; } = null!;
    public string Role { get; set; } = null!;

    public sealed class TokenDto
    {
        public string Token { get; set; } = null!;
        public DateTime ExpiresOn { get; set; }
    }
}
