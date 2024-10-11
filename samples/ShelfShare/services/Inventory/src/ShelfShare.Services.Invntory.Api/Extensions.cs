using Jalpan;
using Jalpan.WebApi.Swagger;
using Serilog;
using ShelfShare.Services.Invntory.Api.Endpoints;

namespace ShelfShare.Services.Invntory.Api;

public static class Extensions
{
    public static IJalpanBuilder AddApi(this IJalpanBuilder builder)
        => builder.AddSwaggerDocs();


    public static WebApplication UseApi(this WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseSerilogRequestLogging();
        app.MapSystemEndpoints();

        return app;
    }
}
