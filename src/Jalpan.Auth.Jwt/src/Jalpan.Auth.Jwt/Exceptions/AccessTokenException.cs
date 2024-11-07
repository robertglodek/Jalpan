using Jalpan.Exceptions;

namespace Jalpan.Auth.Jwt.Exceptions;

public sealed class AccessTokenException(string message) : CustomException(message);
