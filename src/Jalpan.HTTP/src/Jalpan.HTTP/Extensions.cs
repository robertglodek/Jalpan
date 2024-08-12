using Jalpan.Exceptions;
using Jalpan.HTTP.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Polly;
using Polly.Extensions.Http;
using System.Security.Cryptography.X509Certificates;

namespace Jalpan.HTTP;

public static class Extensions
{
    private const string SectionName = "httpClient";
    private const string RegistryName = "http.client";

    public static IJalpanBuilder AddHttpClient(this IJalpanBuilder builder, string sectionName = SectionName)
    {
        if (string.IsNullOrWhiteSpace(sectionName))
        {
            sectionName = SectionName;
        }

        if (!builder.TryRegister(RegistryName))
        {
            return builder;
        }

        var section = builder.Configuration.GetSection(sectionName);
        var options = section.BindOptions<HttpClientOptions>();
        
        if (string.IsNullOrWhiteSpace(options.Name))
        {
            throw new ConfigurationException("HTTP client name cannot be empty.", nameof(options.Name));
        }

        builder.Services.Configure<HttpClientOptions>(section);

        var httpClientbuilder = builder.Services
           .AddHttpClient(options.Name)
           .AddTransientHttpErrorPolicy(_ => HttpPolicyExtensions.HandleTransientHttpError()
               .WaitAndRetryAsync(options.Resiliency.Retries, retry =>
                   options.Resiliency.Exponential
                       ? TimeSpan.FromSeconds(Math.Pow(2, retry))
                       : options.Resiliency.RetryInterval ?? TimeSpan.FromSeconds(2)));

        var certificateLocation = options.Certificate?.Location;
        if (options.Certificate is not null && !string.IsNullOrWhiteSpace(certificateLocation))
        {
            var certificate = new X509Certificate2(certificateLocation, options.Certificate.Password);
            httpClientbuilder.ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler();
                handler.ClientCertificates.Add(certificate);
                return handler;
            });
        }

        if (options.RequestMasking.Enabled)
        {
            builder.Services.Replace(ServiceDescriptor.Singleton<IHttpMessageHandlerBuilderFilter, HttpLoggingFilter>());
        }

        builder.Services.AddSingleton(options);

        return builder;
    }
}