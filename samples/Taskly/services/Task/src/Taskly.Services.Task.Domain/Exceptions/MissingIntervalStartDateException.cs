namespace Taskly.Services.Task.Domain.Exceptions;

public sealed class MissingIntervalStartDateException()
    : DomainException("Interval start date is required for the selected interval.")
{
    public override string Code => "missing_interval_start_date";
}
