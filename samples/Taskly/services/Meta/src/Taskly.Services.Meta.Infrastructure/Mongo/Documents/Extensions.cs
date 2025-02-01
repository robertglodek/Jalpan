using Taskly.Services.Meta.Application.DTO;
using Taskly.Services.Meta.Domain.Entities;
using Taskly.Services.Meta.Domain.ValueObjects;

namespace Taskly.Services.Meta.Infrastructure.Mongo.Documents;

internal static class Extensions
{
    public static Goal AsEntity(this GoalDocument document)
        => new(document.Id, document.Name, document.UserId, document.Description);

    public static GoalDocument AsDocument(this Goal entity)
        => new()
        {
            Id = entity.Id,
            Name = entity.Name,
            UserId = entity.UserId,
            Description = entity.Description
        };

    public static GoalDto AsDto(this GoalDocument document)
        => new()
        {
            Id = document.Id,
            Name = document.Name,
            UserId = document.UserId,
            Description = document.Description
        };

    public static Tag AsEntity(this TagDocument document)
        => new(document.Id, document.Name, document.UserId, (Colour)document.Colour);

    public static TagDocument AsDocument(this Tag entity)
        => new()
        {
            Id = entity.Id,
            UserId = entity.UserId,
            Name = entity.Name,
            Colour = entity.Colour
        };

    public static TagDto AsDto(this TagDocument document)
        => new()
        {
            Id = document.Id,
            Name = document.Name,
            UserId = document.UserId,
            Colour = document.Colour
        };

    public static Section AsEntity(this SectionDocument document)
        => new(document.Id, document.Name, document.UserId, document.Description, document.GoalId);

    public static SectionDocument AsDocument(this Section entity)
        => new()
        {
            Id = entity.Id,
            UserId = entity.UserId,
            Name = entity.Name,
            Description = entity.Description,
            GoalId = entity.GoalId
        };

    public static SectionDto AsDto(this SectionDocument document)
        => new()
        {
            Id = document.Id,
            UserId = document.UserId,
            Name = document.Name,
            Description = document.Description,
            GoalId = document.GoalId
        };
}