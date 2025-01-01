using Microsoft.Extensions.DependencyInjection;

namespace Taskly.Services.Identity.Application.Context;

public static class Extensions
{
    public static IServiceCollection AddDataContextDecorators(this IServiceCollection services)
    {
        services.TryDecorate(typeof(ICommandHandler<,>), typeof(ContextCommandHandlerDecorator<,>));
        services.TryDecorate(typeof(IQueryHandler<,>), typeof(ContextQueryHandlerDecorator<,>));
        return services;
    }
}