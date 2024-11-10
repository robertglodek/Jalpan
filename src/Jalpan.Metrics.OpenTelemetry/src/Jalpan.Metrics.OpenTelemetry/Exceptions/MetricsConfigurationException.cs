using Jalpan.Exceptions;

namespace Jalpan.Metrics.OpenTelemetry.Exceptions;

public sealed class MetricsConfigurationException(string message) : CustomException(message);