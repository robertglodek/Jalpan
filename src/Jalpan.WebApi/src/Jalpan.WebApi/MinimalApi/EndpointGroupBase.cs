using Microsoft.AspNetCore.Builder;

namespace Jalpan.WebApi.MinimalApi;

public abstract class EndpointGroupBase
{
    public abstract string Name { get; }
    public abstract void Map(WebApplication app);
}
