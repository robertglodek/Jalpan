using Jalpan.Handlers;
using Jalpan.WebApi.MinimalApi;
using JetBrains.Annotations;
using Taskly.Services.Identity.Application.Commands;

namespace Taskly.Services.Identity.Api.Endpoints;

[UsedImplicitly]
public sealed class IdentityEndpoints : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(SignUp, pattern: "signup")
            .MapPost(SignIn, pattern: "signin")
            .MapPut(ChangeEmail, pattern: "change_email")
            .MapPut(ChangePassword, pattern: "change_password")
            .MapPost(RevokeAccessToken, pattern: "access_tokens/revoke")
            .MapPost(UseRefreshToken, pattern: "refresh_tokens/use")
            .MapPost(RevokeRefreshToken, pattern: "refresh_tokens/revoke");
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
}