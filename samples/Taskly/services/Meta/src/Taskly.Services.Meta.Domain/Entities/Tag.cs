using Taskly.Services.Meta.Domain.Exceptions;
using Taskly.Services.Meta.Domain.ValueObjects;

namespace Taskly.Services.Meta.Domain.Entities;

public sealed class Tag : AggregateRoot
{
    public Tag(Guid id, string name, Guid userId, Colour colour)
    {
        UpdateName(name);
        if (Guid.Empty == userId)
        {
            throw new InvalidUserIdException();
        }
        
        Id = id;
        Name = name;
        UpdateColour(colour);
        UserId = userId;
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new InvalidTagNameException();
        }
        Name = name;
    }
    
    public void UpdateColour(Colour colour)
    {
        Colour = colour;
    }
    
    public string Name { get; private set; }
    public Colour Colour { get; private set;  }
    public Guid UserId { get; }
}