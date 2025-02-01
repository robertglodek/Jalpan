using Taskly.Services.Meta.Domain.Exceptions;
using Taskly.Services.Meta.Domain.ValueObjects;

namespace Taskly.Services.Meta.Domain.Entities;

public sealed class Tag : AggregateRoot
{
    public Tag(Guid id, string name, Guid userId, Colour colour)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new InvalidTagNameException();
        }
        
        if (Guid.Empty == userId)
        {
            throw new InvalidUserIdException();
        }
        
        Id = id;
        Name = name;
        Colour = colour;
        UserId = userId;
    }
    
    public string Name { get; set; }
    public Colour Colour { get; set; }
    public Guid UserId { get; set; }
}