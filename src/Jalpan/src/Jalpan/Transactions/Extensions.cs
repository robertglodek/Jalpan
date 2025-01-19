using Jalpan.Handlers;

namespace Jalpan.Transactions;

public static class Extensions
{
    public static IJalpanBuilder AddTransactionalDecorators(this IJalpanBuilder builder)
    {
        builder.Services.TryDecorate(typeof(ICommandHandler<,>), typeof(TransactionalCommandHandlerDecorator<,>));
        builder.Services.TryDecorate(typeof(IQueryHandler<,>), typeof(TransactionalQueryHandlerDecorator<,>));
        builder.Services.TryDecorate(typeof(IEventHandler<>), typeof(TransactionalEventHandlerDecorator<>));

        return builder;
    }
}