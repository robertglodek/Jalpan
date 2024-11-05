using Microsoft.AspNetCore.Builder;

namespace Jalpan.WebApi.MinimalApi;

public abstract class EndpointGroupBase
{
    public abstract void Map(WebApplication app);
}
