using Jalpan.Exceptions;

namespace Jalpan.WebApi.CORS.Exceptions;

public sealed class CorsConfigurationException(string message) : CustomException(message);