using System.Diagnostics;
using System.Security.Claims;
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

        IContext context;
        var httpContext = httpContextAccessor.HttpContext;
        var activityId = Activity.Current?.Id ?? ActivityTraceId.CreateRandom().ToString();
        if (httpContext is not null)
        {
            var userId = httpContext.User.Identity?.Name;
            var role = httpContext.User.FindFirst(ClaimTypes.Role)?.Value;
            var claims = httpContext.User.Claims
                .GroupBy(c => c.Type)
                .ToDictionary(g => g.Key, g => g.Select(c => c.Value).ToArray());
            
            context = new Context(activityId, userId, null, role, claims);
        }
        else
        {
            context = new Context(activityId);
        }
        
        contextAccessor.Context = context;

        return context;
    }
}