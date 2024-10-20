using Jalpan;
using Jalpan.WebApi.Swagger;
using Serilog;
using Taskly.Services.Invntory.Api.Endpoints;

namespace Taskly.Services.Invntory.Api;

public static class Extensions
{
    public static IJalpanBuilder AddApi(this IJalpanBuilder builder)
        => builder.AddSwaggerDocs();

    public static WebApplication UseApi(this WebApplication app)
    {
        app.UseSerilogRequestLogging();
        app.UseSwaggerDocs();
        app.MapSystemEndpoints();
        return app;
    }
}
