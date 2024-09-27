using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Jalpan;

public interface IJalpanBuilder
{
    IServiceCollection Services { get; }
    IConfiguration Configuration { get; }
    IHttpClientBuilder HttpClientBuilder { get; set; }
    bool TryRegister(string name);
}
