using FluentValidation;
using Jalpan.Handlers;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Jalpan.Validation;

public static class Extensions
{
    public static IJalpanBuilder AddValidation(this IJalpanBuilder builder)
    {
        builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        builder.Services.TryDecorate(typeof(ICommandHandler<,>), typeof(ValidationCommandHandlerDecorator<,>));

        return builder;
    }
}
