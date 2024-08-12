using Jalpan.CQRS.Events.Dispatchers;
using Jalpan.Types;
using Microsoft.Extensions.DependencyInjection;

namespace Jalpan.CQRS.Events;

public static class Extensions
{
    public static IJalpanBuilder AddEventHandlers(this IJalpanBuilder builder)
    {
        builder.Services.Scan(s =>
            s.FromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
                .AddClasses(c => c.AssignableTo(typeof(IEventHandler<>))
                    .WithoutAttribute(typeof(DecoratorAttribute)))
                .AsImplementedInterfaces()
                .WithTransientLifetime());

        return builder;
    }

    public static IJalpanBuilder AddInMemoryEventDispatcher(this IJalpanBuilder builder)
    {
        builder.Services.AddSingleton<IEventDispatcher, EventDispatcher>();
        return builder;
    }

    public static IJalpanBuilder AddEvents(this IJalpanBuilder builder) => builder.AddEventHandlers().AddInMemoryEventDispatcher();
}
