using Jalpan.Exceptions;

namespace Jalpan.WebApi.Swagger.Exceptions;

public sealed class SwaggerConfigurationException(string message) : CustomException(message);