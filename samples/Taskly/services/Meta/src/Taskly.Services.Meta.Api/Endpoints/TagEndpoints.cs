using Jalpan.Handlers;
using Jalpan.WebApi.MinimalApi;
using Taskly.Services.Meta.Application.Commands;
using Taskly.Services.Meta.Application.Queries;

namespace Taskly.Services.Meta.Api.Endpoints;

[UsedImplicitly]
public sealed class TagEndpoints : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(AddTag)
            .MapPut(UpdateTag)
            .MapDelete(DeleteTag)
            .MapGet(SearchTags)
            .MapGet(GetTag, pattern: "{id}");
    }

    private static async Task<IResult> AddTag(IDispatcher dispatcher, AddTag command)
    {
        var result = await dispatcher.SendAsync(command);
        return Results.CreatedAtRoute(routeName: nameof(GetTag), routeValues: new { id = result }, value: result);
    }

    private static async Task<IResult> UpdateTag(IDispatcher dispatcher, UpdateTag command, Guid id)
    {
        command.Id = id;
        await dispatcher.SendAsync(command);
        return Results.NoContent();
    }

    private static async Task<IResult> DeleteTag(IDispatcher dispatcher, Guid id)
    {
        await dispatcher.SendAsync(new DeleteTag(id));
        return Results.NoContent();
    }

    private static async Task<IResult> SearchTags(IDispatcher dispatcher, [AsParameters] SearchTags query)
    {
        var result = await dispatcher.QueryAsync(query);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetTag(IDispatcher dispatcher, Guid id)
    {
        var result = await dispatcher.QueryAsync(new GetTag { Id = id });
        return Results.Ok(result);
    }
}