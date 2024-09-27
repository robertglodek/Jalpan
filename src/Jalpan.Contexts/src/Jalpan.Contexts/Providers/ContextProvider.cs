using System.Diagnostics;
using Jalpan.Contexts.Accessors;
using Microsoft.AspNetCore.Http;

namespace Jalpan.Contexts.Providers;

internal sealed class ContextProvider(IHttpContextAccessor httpContextAccessor, IContextAccessor contextAccessor) : IContextProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IContextAccessor _contextAccessor = contextAccessor;

    public IContext Current()
    {
        if (_contextAccessor.Context is not null)
        {
            return _contextAccessor.Context;
        }

        var httpContext = _httpContextAccessor.HttpContext;
        var userId = httpContext?.User.Identity?.Name;
        var activityId = Activity.Current?.Id ?? ActivityTraceId.CreateRandom().ToString();
        var context = new Context(activityId, userId);
        _contextAccessor.Context = context;

        return context;
    }
}