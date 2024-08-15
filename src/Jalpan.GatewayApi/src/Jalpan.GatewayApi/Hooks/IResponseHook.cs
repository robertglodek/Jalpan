using Microsoft.AspNetCore.Http;

namespace Jalpan.GatewayApi.Hooks;

public interface IResponseHook
{
    Task InvokeAsync(HttpResponse response, ExecutionData data);
}