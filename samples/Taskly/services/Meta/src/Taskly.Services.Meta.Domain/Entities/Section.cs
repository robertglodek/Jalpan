﻿using Taskly.Services.Meta.Domain.Exceptions;

namespace Taskly.Services.Meta.Domain.Entities;

public sealed class Section : AggregateRoot
{
    public Section(Guid id, string name, Guid userId, string? description, Guid? goalId)
    {
        if (Guid.Empty == userId)
        {
            throw new InvalidUserIdException();
        }

        if (goalId.HasValue && goalId.Value == Guid.Empty)
        {
            throw new InvalidGoalIdException();
        }
        
        Id = id;
        Name = name;
        Description = description;
        UserId = userId;
        GoalId = goalId;
    }
    
    public string Name { get; set; }
    public string? Description { get; set; }
    public Guid? GoalId { get; set; }
    public Guid UserId { get; set; }
}