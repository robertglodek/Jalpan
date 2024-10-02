using Jalpan.Attributes;
using Microsoft.AspNetCore.Builder;

namespace Jalpan.WebApi.Contracts;

public static class Extensions
{
    public static IApplicationBuilder UsePublicContracts(this IApplicationBuilder app,
        string endpoint = "/contracts",
        Type? attributeType = null,
        bool attributeRequired = true)
        => app.UseMiddleware<PublicContractsMiddleware>(string.IsNullOrWhiteSpace(endpoint) ? "/contracts" :
            endpoint.StartsWith('/') ? endpoint : $"/{endpoint}", attributeType ?? typeof(PublicContractAttribute), attributeRequired);
}
