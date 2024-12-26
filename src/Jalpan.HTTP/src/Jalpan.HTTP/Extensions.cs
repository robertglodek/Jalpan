using Jalpan.HTTP.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Polly;
using Polly.Extensions.Http;
using System.Security.Cryptography.X509Certificates;
using Jalpan.HTTP.Client;
using Jalpan.HTTP.Exceptions;
using Jalpan.HTTP.Factories;
using Jalpan.HTTP.Serialization;

namespace Jalpan.HTTP;

public static class Extensions
{
    private const string DefaultSectionName = "httpClient";
    private const string RegistryKey = "http.client";

    public static IJalpanBuilder AddHttpClient(this IJalpanBuilder builder, string sectionName = DefaultSectionName)
    {
        sectionName = string.IsNullOrEmpty(sectionName) ? DefaultSectionName : sectionName;

        if (!builder.TryRegister(RegistryKey))
        {
            return builder;
        }

        var section = builder.Configuration.GetSection(sectionName);
        var options = section.BindOptions<HttpClientOptions>();
        
        builder.Services.Configure<HttpClientOptions>(section);

        builder.Services.AddSingleton<IHttpClientSerializer, SystemTextJsonHttpClientSerializer>();
        builder.Services.AddTransient<IJalpanHttpClientFactory, JalpanHttpClientFactory>();
        builder.Services.AddTransient<IJalpanHttpClient, JalpanHttpClient>();
        
        
        foreach (var (name, option) in options.Clients)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new HttpConfigurationException("HTTP client's name cannot be empty.");
            }
            
            var httpClientBuilder = builder.Services
                .AddHttpClient(name)
                .AddTransientHttpErrorPolicy(_ => HttpPolicyExtensions.HandleTransientHttpError()
                    .WaitAndRetryAsync(option.Resiliency.Retries, retry =>
                        option.Resiliency.Exponential
                            ? TimeSpan.FromSeconds(Math.Pow(2, retry))
                            : option.Resiliency.RetryInterval ?? TimeSpan.FromSeconds(2)));

            var certificateLocation = option.Certificate?.Location;
            if (option.Certificate is null || string.IsNullOrWhiteSpace(certificateLocation)) continue;
            var certificate = new X509Certificate2(certificateLocation, option.Certificate.Password);
            httpClientBuilder.ConfigurePrimaryHttpMessageHandler(() =>
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

        return builder;
    }
}