using System.Diagnostics;
using Jalpan.Contexts.Accessors;
using Microsoft.AspNetCore.Http;

namespace Jalpan.Contexts.Providers;

internal sealed class ContextProvider(IHttpContextAccessor httpContextAccessor, IContextAccessor contextAccessor)
    : IContextProvider
{
    public IContext Current()
    {
        if (contextAccessor.Context is not null)
        {
            return contextAccessor.Context;
        }

        var httpContext = httpContextAccessor.HttpContext;
        var userId = httpContext?.User.Identity?.Name;
        var activityId = Activity.Current?.Id ?? ActivityTraceId.CreateRandom().ToString();
        var context = new Context(activityId, userId);
        contextAccessor.Context = context;

        return context;
    }
}