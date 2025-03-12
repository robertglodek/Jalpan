namespace Taskly.Services.Note.Domain.Entities;

public class AuditableAggregateRoot : AggregateRoot
{
    public DateTime Created { get; protected set; }

    public string? CreatedBy { get; protected set; }

    public DateTime LastModified { get; protected set; }

    public string? LastModifiedBy { get; protected set; }
}