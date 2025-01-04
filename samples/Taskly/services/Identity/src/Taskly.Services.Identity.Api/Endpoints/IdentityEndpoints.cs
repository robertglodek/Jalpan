using Jalpan.Handlers;
using Jalpan.WebApi.MinimalApi;
using JetBrains.Annotations;
using Taskly.Services.Identity.Application.Commands;
using Taskly.Services.Identity.Domain.ValueObjects;

namespace Taskly.Services.Identity.Api.Endpoints;

[UsedImplicitly]
public sealed class IdentityEndpoints : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(SignUp, pattern: "signup")
            .MapPost(SignIn, pattern: "signin")
            .MapPost(SetLock, pattern: "lock",
                config: config => config.RequireAuthorization(n => n.RequireRole(Role.Admin)))
            .MapPut(ChangeEmail, pattern: "change_email", config: config => config.RequireAuthorization())
            .MapPut(ChangePassword, pattern: "change_password", config: config => config.RequireAuthorization())
            .MapPost(RevokeAccessToken, pattern: "access_token/revoke",
                config: config => config.RequireAuthorization(n => n.RequireRole(Role.Admin)))
            .MapPost(UseRefreshToken, pattern: "refresh_tokens/use")
            .MapPost(RevokeRefreshToken, pattern: "refresh_tokens/revoke",
                config: config => config.RequireAuthorization(n => n.RequireRole(Role.Admin)));
    }

    private static async Task<IResult> SignUp(IDispatcher dispatcher, SignUp command)
    {
        await dispatcher.SendAsync(command);
        return Results.NoContent();
    }

    private static async Task<IResult> SignIn(IDispatcher dispatcher, SignIn command)
    {
        var result = await dispatcher.SendAsync(command);
        return Results.Ok(result);
    }
    
    private static async Task<IResult> UseRefreshToken(IDispatcher dispatcher, UseRefreshToken command)
    {
        var result = await dispatcher.SendAsync(command);
        return Results.Ok(result);
    }

    private static async Task<IResult> ChangeEmail(IDispatcher dispatcher, ChangeEmail command)
    {
        await dispatcher.SendAsync(command);
        return Results.NoContent();
    }

    private static async Task<IResult> ChangePassword(IDispatcher dispatcher, ChangePassword command)
    {
        await dispatcher.SendAsync(command);
        return Results.NoContent();
    }

    private static async Task<IResult> RevokeAccessToken(IDispatcher dispatcher, RevokeAccessToken command)
    {
        await dispatcher.SendAsync(command);
        return Results.NoContent();
    }

    private static async Task<IResult> RevokeRefreshToken(IDispatcher dispatcher, RevokeRefreshToken command)
    {
        await dispatcher.SendAsync(command);
        return Results.NoContent();
    }
    
    private static async Task<IResult> SetLock(IDispatcher dispatcher, SetLock command)
    {
        await dispatcher.SendAsync(command);
        return Results.NoContent();
    }
}