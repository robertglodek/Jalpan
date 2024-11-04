using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Jalpan.Auth.Jwt.Managers;

internal sealed class InMemoryAccessTokenManager(IMemoryCache cache,
    IHttpContextAccessor httpContextAccessor,
    IOptions<AuthOptions> options) : IAccessTokenManager
{
    private readonly IMemoryCache _cache = cache;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly TimeSpan _expiry = options.Value.Jwt.Expiry;

    public Task<bool> IsCurrentActiveToken()
        => IsActiveAsync(GetCurrentAsync());

    public Task DeactivateCurrentAsync()
        => DeactivateAsync(GetCurrentAsync());

    public Task<bool> IsActiveAsync(string token)
        => Task.FromResult(string.IsNullOrWhiteSpace(_cache.Get<string>(GetCacheKey(token))));

    public Task DeactivateAsync(string token)
    {
        _cache.Set(GetCacheKey(token), "revoked", new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _expiry
        });

        return Task.CompletedTask;
    }

    private string GetCurrentAsync()
    {
        if(_httpContextAccessor.HttpContext == null)
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
