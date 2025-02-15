using Jalpan.Handlers;
using Jalpan.WebApi.MinimalApi;
using Taskly.Services.Meta.Application.Commands;
using Taskly.Services.Meta.Application.Queries;

namespace Taskly.Services.Meta.Api.Endpoints;

[UsedImplicitly]
public sealed class SectionEndpoints : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(AddSection)
            .MapPut(UpdateSection)
            .MapDelete(DeleteSection)
            .MapGet(SearchSections)
            .MapGet(GetSection, pattern: "{id}");
    }

    private static async Task<IResult> AddSection(IDispatcher dispatcher, AddSection command)
    {
        var result = await dispatcher.SendAsync(command);
        return Results.CreatedAtRoute(routeName: nameof(GetSection), routeValues: new { id = result }, value: result);
    }

    private static async Task<IResult> UpdateSection(IDispatcher dispatcher, UpdateSection command, Guid id)
    {
        command.Id = id;
        await dispatcher.SendAsync(command);
        return Results.NoContent();
    }

    private static async Task<IResult> DeleteSection(IDispatcher dispatcher, Guid id)
    {
        await dispatcher.SendAsync(new DeleteSection(id));
        return Results.NoContent();
    }

    private static async Task<IResult> SearchSections(IDispatcher dispatcher, [AsParameters] SearchSections query)
    {
        var result = await dispatcher.QueryAsync(query);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetSection(IDispatcher dispatcher, Guid id)
    {
        var result = await dispatcher.QueryAsync(new GetSection { Id = id });
        return Results.Ok(result);
    }
}