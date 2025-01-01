using Microsoft.Extensions.DependencyInjection;

namespace Taskly.Services.Identity.Application.Lock;

public static class Extensions
{
    public static IServiceCollection AddIsUserLockedCheckDecorators(this IServiceCollection services)
    {
        services.TryDecorate(typeof(ICommandHandler<,>), typeof(IsUserLockedCommandHandlerDecorator<,>));
        services.TryDecorate(typeof(IQueryHandler<,>), typeof(IsUserLockedQueryHandlerDecorator<,>));
        return services;
    }
}