namespace Jalpan.GatewayApi;

internal interface ISchemaValidator
{
    Task<IEnumerable<Error>> ValidateAsync(string payload, string schema);
}