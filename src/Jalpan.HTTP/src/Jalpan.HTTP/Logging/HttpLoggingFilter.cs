using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Jalpan.HTTP.Logging;

internal sealed class HttpLoggingFilter(ILoggerFactory loggerFactory, IOptions<HttpClientOptions> options) : IHttpMessageHandlerBuilderFilter
{
    private readonly ILoggerFactory _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
    private readonly IOptions<HttpClientOptions> _options = options;

    public Action<HttpMessageHandlerBuilder> Configure(Action<HttpMessageHandlerBuilder> next)
        => next is null
                ? throw new ArgumentNullException(nameof(next))
                : (builder =>
                {
                    next(builder);
                    var logger = _loggerFactory.CreateLogger($"System.Net.Http.HttpClient.{builder.Name}.LogicalHandler");
                    builder.AdditionalHandlers.Insert(0, new LoggingScopeHttpMessageHandler(logger, _options));
                });
}