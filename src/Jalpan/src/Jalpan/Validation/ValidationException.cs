using FluentValidation.Results;
using Jalpan.Exceptions;

namespace Jalpan.Validation;

public sealed class ValidationException() : CustomException("One or more validation failures have occurred.")
{
    public ValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        Errors = failures
            .GroupBy(f => f.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(f => new ValidationError(f.ErrorMessage, f.ErrorCode)).ToArray()
            );
    }

    public IDictionary<string, ValidationError[]> Errors { get; } = new Dictionary<string, ValidationError[]>();
}