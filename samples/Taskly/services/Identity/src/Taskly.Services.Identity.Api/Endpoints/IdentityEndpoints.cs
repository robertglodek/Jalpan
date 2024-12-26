using Jalpan.WebApi.MinimalApi;

namespace Taskly.Services.Identity.Api.Endpoints;

[UsedImplicitly]
public sealed class IdentityEndpoints : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this);
    }
}
