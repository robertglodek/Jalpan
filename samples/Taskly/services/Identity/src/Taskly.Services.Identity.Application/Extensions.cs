using Jalpan.Dispatchers;
using Jalpan.Messaging;
using Jalpan.Validation;
using Microsoft.Extensions.DependencyInjection;
using Taskly.Services.Identity.Application.Context;
using Taskly.Services.Identity.Application.Exceptions;
using Taskly.Services.Identity.Application.Lock;

namespace Taskly.Services.Identity.Application;

public static class Extensions
{
    private const string RefreshTokenSectionName = "auth:refreshToken";
    public static IJalpanBuilder AddApplication(this IJalpanBuilder builder)
    {
        builder.AddHandlers();
        builder.AddDispatchers();
        builder.AddValidation();
        builder.AddExceptionToMessageResolver<ExceptionToMessageResolver>();
        builder.Services.Configure<RefreshTokenOptions>(builder.Configuration.GetSection(RefreshTokenSectionName));
        builder.Services.AddDataContextDecorators();
        builder.Services.AddIsUserLockedCheckDecorators();
        return builder;
    }
}