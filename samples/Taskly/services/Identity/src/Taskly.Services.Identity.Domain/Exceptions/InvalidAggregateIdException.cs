namespace Taskly.Services.Identity.Domain.Exceptions;

public sealed class InvalidAggregateIdException() : DomainException($"Invalid aggregate id.")
{
    public override string Code => "invalid_aggregate_id";
}