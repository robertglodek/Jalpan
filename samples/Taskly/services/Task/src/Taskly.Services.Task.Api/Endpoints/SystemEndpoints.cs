﻿using Jalpan;
using Jalpan.WebApi.MinimalApi;
using Microsoft.Extensions.Options;

namespace Taskly.Services.Task.Api.Endpoints;

public sealed class SystemEndpoints : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
           .MapGet(() => "pong", "Ping", "/ping")
           .MapGet((IOptions<AppOptions> appOptions) =>
                Results.Ok(new { appOptions.Value.Name, appOptions.Value.Service, appOptions.Value.Version, appOptions.Value.Instance }), "About", "/about");
    }
}

