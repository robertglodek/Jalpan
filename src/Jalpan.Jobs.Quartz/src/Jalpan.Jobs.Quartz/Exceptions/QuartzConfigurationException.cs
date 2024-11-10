using Jalpan.Exceptions;

namespace Jalpan.Jobs.Quartz.Exceptions;

public sealed class QuartzConfigurationException(string message) : CustomException(message);