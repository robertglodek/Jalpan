using Taskly.Services.Identity.Application.DTO;
using Taskly.Services.Identity.Core.Entities;
using Taskly.Services.Identity.Domain.ValueObjects;

namespace Taskly.Services.Identity.Infrastructure.Mongo.Documents;

internal static class Extensions
{
    public static User AsEntity(this UserDocument document)
        => new(document.Id, document.Email, document.Password, (Role)document.Role, document.CreatedAt, document.LastModifiedAt);

    public static UserDocument AsDocument(this User entity)
        => new()
        {
            Id = entity.Id,
            Email = entity.Email,
            Password = entity.Password,
            Role = entity.Role,
            CreatedAt = entity.CreatedAt,
            LastModifiedAt = entity.LastModifiedAt    
        };

    public static UserDto AsDto(this UserDocument document)
        => new()
        {
            Id = document.Id,
            Email = document.Email,
            Role = document.Role,
            CreatedAt = document.CreatedAt,
            LastModifiedAt = document.LastModifiedAt
        };

    public static RefreshToken AsEntity(this RefreshTokenDocument document)
        => new(document.Id, document.UserId, document.Token, document.CreatedAt, document.RevokedAt);
    
    public static RefreshTokenDocument AsDocument(this RefreshToken entity)
        => new()
        {
            Id = entity.Id,
            UserId = entity.UserId,
            Token = entity.Token,
            CreatedAt = entity.CreatedAt,
            RevokedAt = entity.RevokedAt
        };
}