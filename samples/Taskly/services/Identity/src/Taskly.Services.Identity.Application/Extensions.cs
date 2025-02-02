using Jalpan.Dispatchers;
using Jalpan.Messaging;
using Jalpan.Transactions;
using Jalpan.Validation;
using Microsoft.Extensions.DependencyInjection;
using Taskly.Services.Identity.Application.Commands;
using Taskly.Services.Identity.Application.Commands.Handlers;
using Taskly.Services.Identity.Application.Exceptions;

namespace Taskly.Services.Identity.Application;

public static class Extensions
{
    private const string RefreshTokenSectionName = "auth:refreshToken";
    public static IJalpanBuilder AddApplication(this IJalpanBuilder builder)
    {
        builder.Services.AddScoped<ICommandHandler<SignUp, Empty>, SignUpHandler>();
        builder.AddHandlers();
        builder.AddDispatchers();
        builder.AddTransactionalDecorators();
        builder.AddValidation();
        builder.AddExceptionToMessageResolver<ExceptionToMessageResolver>();
        builder.Services.Configure<RefreshTokenOptions>(builder.Configuration.GetSection(RefreshTokenSectionName));
        return builder;
    }
}