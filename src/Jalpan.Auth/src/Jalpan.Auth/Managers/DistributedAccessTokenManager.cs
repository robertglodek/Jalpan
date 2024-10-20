using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Jalpan.Auth.Managers;

internal sealed class DistributedAccessTokenManager(IDistributedCache cache, 
    IHttpContextAccessor httpContextAccessor,
    IOptions<AuthOptions> options) : IAccessTokenManager
{
    private readonly IDistributedCache _cache = cache;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly TimeSpan _expiry = options.Value.Jwt.Expiry;

    public Task<bool> IsCurrentActiveToken()
        => IsActiveAsync(GetCurrentAsync());

    public Task DeactivateCurrentAsync()
        => DeactivateAsync(GetCurrentAsync());

    public async Task<bool> IsActiveAsync(string token)
       => string.IsNullOrWhiteSpace(await _cache.GetStringAsync(GetCacheKey(token)));

    public Task DeactivateAsync(string token)
       => _cache.SetStringAsync(GetCacheKey(token),
           "revoked", new DistributedCacheEntryOptions
           {
               AbsoluteExpirationRelativeToNow = _expiry
           });

    private string GetCurrentAsync()
    {
        if (_httpContextAccessor.HttpContext == null)
        {
            return string.Empty;
        }

        var authorizationHeader = _httpContextAccessor.HttpContext!.Request.Headers.Authorization;

        if (authorizationHeader == StringValues.Empty)
        {
            return string.Empty;
        }

        var token = authorizationHeader.SingleOrDefault()?.Split(' ').Last();

        return token ?? string.Empty;
    }

    private static string GetCacheKey(string token) => $"blacklisted-tokens:{token}";
}
