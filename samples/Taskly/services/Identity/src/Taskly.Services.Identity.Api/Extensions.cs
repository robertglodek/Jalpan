using HealthChecks.UI.Client;
using Jalpan;
using Jalpan.Auth.Jwt;
using Jalpan.Logging.Serilog;
using Jalpan.WebApi.Exceptions;
using Jalpan.WebApi.MinimalApi;
using Jalpan.WebApi.Swagger;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using Taskly.Services.Identity.Api.Exceptions.Mappers;

namespace Taskly.Services.Identity.Api;

public static class Extensions
{
    public static IJalpanBuilder AddApi(this IJalpanBuilder builder)
    {
        builder.AddSwaggerDocs();
        builder.AddErrorHandler<ExceptionToResponseMapper>();
        builder.Services.AddHealthChecks().AddSelfCheck();
        return builder;
    }
    
    private static void AddSelfCheck(this IHealthChecksBuilder builder)
        => builder.AddCheck("Self health check", () => HealthCheckResult.Healthy());

    public static WebApplication UseApi(this WebApplication app)
    {
        app.UseErrorHandler();
        app.UseContextLogger();
        app.UseSwaggerDocs();
        app.UseSerilogRequestLogging();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseAccessTokenValidator();
        app.MapEndpoints();
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        return app;
    }
}