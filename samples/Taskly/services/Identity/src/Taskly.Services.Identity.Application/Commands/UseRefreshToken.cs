using Taskly.Services.Identity.Application.DTO;

namespace Taskly.Services.Identity.Application.Commands;

public sealed class UseRefreshToken : ICommand<AuthDto>
{
    public string RefreshToken { get; set; } = null!;

    public UseRefreshToken(string refreshToken)
    {
        RefreshToken = refreshToken;
    }
}
