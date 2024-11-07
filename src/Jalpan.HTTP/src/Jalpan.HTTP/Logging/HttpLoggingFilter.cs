using Microsoft.Extensions.Http;

namespace Jalpan.HTTP.Logging;

internal sealed class HttpLoggingFilter(ILoggerFactory loggerFactory, IOptions<HttpClientOptions> options) : IHttpMessageHandlerBuilderFilter
{
    private readonly ILoggerFactory _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));

    public Action<HttpMessageHandlerBuilder> Configure(Action<HttpMessageHandlerBuilder> next)
        => next is null
                ? throw new ArgumentNullException(nameof(next))
                : builder =>
                {
                    next(builder);
                    var logger = _loggerFactory.CreateLogger($"System.Net.Http.HttpClient.{builder.Name}.LogicalHandler");
                    builder.AdditionalHandlers.Insert(0, new LoggingScopeHttpMessageHandler(logger, options));
                };
}