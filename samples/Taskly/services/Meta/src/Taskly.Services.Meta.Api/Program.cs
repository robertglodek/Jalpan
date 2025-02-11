using HealthChecks.UI.Client;
using Jalpan;
using Jalpan.Logging.Serilog;
using Jalpan.WebApi.MinimalApi;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Taskly.Services.Meta.Application;
using Taskly.Services.Meta.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseLogging();

builder.Services.AddJalpan(builder.Configuration, 
    jalpanBuilder => jalpanBuilder.AddApplication().AddInfrastructure());

var app = builder.Build();

app.UseInfrastructure();
app.MapEndpoints();
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();