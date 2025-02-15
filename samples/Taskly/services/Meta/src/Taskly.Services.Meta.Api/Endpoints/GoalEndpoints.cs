using Jalpan.Handlers;
using Jalpan.WebApi.MinimalApi;
using Taskly.Services.Meta.Application.Commands;
using Taskly.Services.Meta.Application.Queries;

namespace Taskly.Services.Meta.Api.Endpoints;

[UsedImplicitly]
public sealed class GoalEndpoints : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(AddGoal)
            .MapPut(UpdateGoal)
            .MapDelete(DeleteGoal)
            .MapGet(GetGoals)
            .MapGet(GetGoal, pattern: "{id}");
    }

    private static async Task<IResult> AddGoal(IDispatcher dispatcher, AddGoal command)
    {
        var result = await dispatcher.SendAsync(command);
        return Results.CreatedAtRoute(routeName: nameof(GetGoal), routeValues: new { id = result }, value: result);
    }

    private static async Task<IResult> UpdateGoal(IDispatcher dispatcher, UpdateGoal command, Guid id)
    {
        command.Id = id;
        await dispatcher.SendAsync(command);
        return Results.NoContent();
    }

    private static async Task<IResult> DeleteGoal(IDispatcher dispatcher, Guid id)
    {
        await dispatcher.SendAsync(new DeleteGoal(id));
        return Results.NoContent();
    }

    private static async Task<IResult> GetGoals(IDispatcher dispatcher)
    {
        var result = await dispatcher.QueryAsync(new GetGoals());
        return Results.Ok(result);
    }

    private static async Task<IResult> GetGoal(IDispatcher dispatcher, Guid id)
    {
        var result = await dispatcher.QueryAsync(new GetGoal { Id = id });
        return Results.Ok(result);
    }
}