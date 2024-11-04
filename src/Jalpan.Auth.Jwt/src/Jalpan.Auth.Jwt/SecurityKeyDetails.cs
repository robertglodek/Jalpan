using Microsoft.IdentityModel.Tokens;

namespace Jalpan.Auth.Jwt;

internal sealed record SecurityKeyDetails(SecurityKey Key, string Algorithm);