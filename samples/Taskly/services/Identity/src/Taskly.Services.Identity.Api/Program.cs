using HealthChecks.UI.Client;
using Jalpan;
using Jalpan.WebApi.MinimalApi;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Taskly.Services.Identity.Application;
using Taskly.Services.Identity.Infrastructure;

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