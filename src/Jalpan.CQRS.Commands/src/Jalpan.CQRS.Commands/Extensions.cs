using Jalpan.CQRS.Commands.Dispatchers;
using Jalpan.Types;
using Microsoft.Extensions.DependencyInjection;

namespace Jalpan.CQRS.Commands;

public static class Extensions
{
    public static IJalpanBuilder AddCommandHandlers(this IJalpanBuilder builder)
    {
        builder.Services.Scan(s =>
            s.FromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
                .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<,>))
                    .WithoutAttribute(typeof(DecoratorAttribute)))
                .AsImplementedInterfaces()
                .WithTransientLifetime());

        return builder;
    }

    public static IJalpanBuilder AddInMemoryCommandDispatcher(this IJalpanBuilder builder)
    {
        builder.Services.AddSingleton<ICommandDispatcher, CommandDispatcher>();
        return builder;
    }

    public static IJalpanBuilder AddCommands(this IJalpanBuilder builder) => builder.AddCommandHandlers().AddInMemoryCommandDispatcher();
}