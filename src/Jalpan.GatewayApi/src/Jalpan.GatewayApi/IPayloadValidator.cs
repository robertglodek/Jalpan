using Microsoft.AspNetCore.Http;

namespace Jalpan.GatewayApi;

public interface IPayloadValidator
{
    Task<bool> TryValidate(ExecutionData executionData, HttpResponse httpResponse);
    Task<IEnumerable<Error>> GetValidationErrorsAsync(PayloadSchema payloadSchema);
}