using Microsoft.IdentityModel.Tokens;

namespace Jalpan.Auth;

internal sealed record SecurityKeyDetails(SecurityKey Key, string Algorithm);