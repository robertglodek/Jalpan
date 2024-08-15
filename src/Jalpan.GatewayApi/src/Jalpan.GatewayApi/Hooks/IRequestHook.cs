using Microsoft.AspNetCore.Http;

namespace Jalpan.GatewayApi.Hooks;

public interface IRequestHook
{
    Task InvokeAsync(HttpRequest request, ExecutionData data);
}