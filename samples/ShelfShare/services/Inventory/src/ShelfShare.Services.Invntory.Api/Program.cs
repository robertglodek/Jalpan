using Jalpan;
using Jalpan.Logging.Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseLogging();

builder.Services.AddJalpan(builder.Configuration, configure =>
{
    configure.AddApplication().AddInfrastructure().AddApi();
});

var app = builder.Build();

app.UseApi();

app.Run();
