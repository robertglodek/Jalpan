using Taskly.Services.Meta.Domain.Exceptions;

namespace Taskly.Services.Meta.Domain.Entities;

public sealed class Goal : AggregateRoot
{
    public Goal(Guid id, string name, Guid userId, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidGoalNameException();
        }

        if (Guid.Empty == userId)
        {
            throw new InvalidUserIdException();
        }

        Id = id;
        Name = name;
        Description = description;
        UserId = userId;
    }
    
    public string Name { get; set; }
    public string? Description { get; set; }
    public Guid UserId { get; set; }
}