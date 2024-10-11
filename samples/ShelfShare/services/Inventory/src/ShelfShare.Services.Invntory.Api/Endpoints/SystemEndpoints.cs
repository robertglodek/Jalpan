using Jalpan;
using Microsoft.Extensions.Options;

namespace ShelfShare.Services.Invntory.Api.Endpoints;

public static class SystemEndpoints
{
    public static void MapSystemEndpoints(this WebApplication app)
    {
        app.MapGet("/ping", () => "pong").WithName("Ping");

        app.MapGet("/about", (IOptions<AppOptions> appOptions) =>
            Results.Ok(new { appOptions.Value.Name, appOptions.Value.Service, appOptions.Value.Version, appOptions.Value.Instance }))
        .WithName("About");
    }
}
