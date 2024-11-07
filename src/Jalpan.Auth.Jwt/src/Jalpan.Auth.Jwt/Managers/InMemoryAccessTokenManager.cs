using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace Jalpan.Auth.Jwt.Managers;

internal sealed class InMemoryAccessTokenManager(
    IMemoryCache cache,
    IHttpContextAccessor httpContextAccessor,
    IOptions<AuthOptions> options) : IAccessTokenManager
{
    private readonly TimeSpan _expiry = options.Value.Jwt.Expiry;

    public Task<bool> IsCurrentActiveToken()
        => IsActiveAsync(GetCurrentAsync());

    public Task DeactivateCurrentAsync()
        => DeactivateAsync(GetCurrentAsync());

    public Task<bool> IsActiveAsync(string token)
        => Task.FromResult(string.IsNullOrWhiteSpace(cache.Get<string>(GetCacheKey(token))));

    public Task DeactivateAsync(string token)
    {
        cache.Set(GetCacheKey(token), "revoked", new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _expiry
        });

        return Task.CompletedTask;
    }

    private string GetCurrentAsync()
    {
        if(httpContextAccessor.HttpContext == null)
        {
            return string.Empty;
        }

        var authorizationHeader = httpContextAccessor.HttpContext!.Request.Headers.Authorization;

        if (authorizationHeader == StringValues.Empty)
        {
            return string.Empty;
        }

        var token = authorizationHeader.SingleOrDefault()?.Split(' ').Last();

        return token ?? string.Empty;
    }

    private static string GetCacheKey(string token) => $"revoked-tokens:{token}";
}
