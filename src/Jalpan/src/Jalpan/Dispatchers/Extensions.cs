using Jalpan.Attributes;
using Jalpan.Handlers;

namespace Jalpan.Dispatchers;

public static class Extensions
{
    public static IJalpanBuilder AddHandlers(this IJalpanBuilder builder)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToArray();

        RegisterHandler(builder, assemblies, typeof(ICommandHandler<,>));
        RegisterHandler(builder, assemblies, typeof(IEventHandler<>));
        RegisterHandler(builder, assemblies, typeof(IQueryHandler<,>));

        return builder;
    }

    private static void RegisterHandler(IJalpanBuilder builder, Assembly[] assemblies, Type handlerType)
    {
        builder.Services.Scan(scan => scan.FromAssemblies(assemblies)
            .AddClasses(classes => classes.AssignableTo(handlerType)
                .WithoutAttribute<DecoratorAttribute>())
            .AsImplementedInterfaces()
            .WithScopedLifetime());
    }

    public static IJalpanBuilder AddDispatchers(this IJalpanBuilder builder)
    {
        builder.Services
            .AddSingleton<IDispatcher, InMemoryDispatcher>()
            .AddSingleton<ICommandDispatcher, InMemoryCommandDispatcher>()
            .AddSingleton<IEventDispatcher, InMemoryEventDispatcher>()
            .AddSingleton<IQueryDispatcher, InMemoryQueryDispatcher>();

        return builder;
    }
}