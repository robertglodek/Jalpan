using Jalpan;
using Jalpan.WebApi.MinimalApi;
using Jalpan.WebApi.Swagger;
using Serilog;

namespace Taskly.Services.Identity.Api;

public static class Extensions
{
    public static IJalpanBuilder AddApi(this IJalpanBuilder builder)
        => builder.AddSwaggerDocs();


    public static WebApplication UseApi(this WebApplication app)
    {
        app.UseSerilogRequestLogging();
        app.UseSwaggerDocs();
        app.MapEndpoints();

        return app;
    }
}
