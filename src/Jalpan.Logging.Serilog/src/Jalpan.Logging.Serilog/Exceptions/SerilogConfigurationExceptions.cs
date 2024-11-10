using Jalpan.Exceptions;

namespace Jalpan.Logging.Serilog.Exceptions;

public sealed class SerilogConfigurationExceptions(string message) : CustomException(message);