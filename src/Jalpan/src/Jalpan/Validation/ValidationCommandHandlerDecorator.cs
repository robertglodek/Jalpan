﻿using FluentValidation;
using Jalpan.Attributes;
using Jalpan.Handlers;
using Jalpan.Types;

namespace Jalpan.Validation;

[Decorator]
internal sealed class ValidationCommandHandlerDecorator<TCommand, TResponse>(
    ICommandHandler<TCommand, TResponse> handler, IEnumerable<IValidator<TCommand>> validators)
    : ICommandHandler<TCommand, TResponse> where TCommand : class, ICommand<TResponse>
{
    public async Task<TResponse> HandleAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        if (validators.Any())
        {
            var context = new ValidationContext<TCommand>(command);

            var validationResults =
                await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults.Where(r => r.Errors.Count != 0)
                .SelectMany(r => r.Errors).ToList();

            if (failures.Count != 0)
            {
                throw new ValidationException(failures);
            }
        }

        var result = await handler.HandleAsync(command, cancellationToken);

        return result;
    }
}
