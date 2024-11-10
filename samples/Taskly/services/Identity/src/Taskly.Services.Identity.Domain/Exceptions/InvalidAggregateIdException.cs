namespace Taskly.Services.Identity.Domain.Exceptions;

public sealed class InvalidAggregateIdException : DomainException
{
    public override string Code { get; } = "invalid_aggregate_id";
    
    public InvalidAggregateIdException() : base($"Invalid aggregate id.")
    {
    }
}