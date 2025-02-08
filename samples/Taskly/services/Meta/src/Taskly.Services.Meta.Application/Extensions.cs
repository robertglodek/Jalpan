using Jalpan;
using Jalpan.Dispatchers;
using Jalpan.Transactions;
using Jalpan.Validation;

namespace Taskly.Services.Meta.Application;

public static class Extensions
{
    public static IJalpanBuilder AddApplication(this IJalpanBuilder builder)
    {
        builder.AddHandlers();
        builder.AddDispatchers();
        builder.AddTransactionalDecorators();
        builder.AddValidation();
        return builder;
    }
}