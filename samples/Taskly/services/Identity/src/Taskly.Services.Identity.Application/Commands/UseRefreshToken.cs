using Taskly.Services.Identity.Application.DTO;

namespace Taskly.Services.Identity.Application.Commands;

[UsedImplicitly]
public sealed class UseRefreshToken(string refreshToken) : ICommand<AuthDto>
{
    public string RefreshToken { get; set; } = refreshToken;
}
