using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Jalpan.Auth.Accessors;

internal class UserAccessor(IHttpContextAccessor httpContextAccessor) : IUserAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    public string? Id => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
}