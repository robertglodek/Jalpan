namespace Jalpan.GatewayApi.Auth;

internal interface IPolicyManager
{
    IDictionary<string, string>? GetClaims(string policy);
}