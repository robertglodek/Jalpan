using System.Diagnostics.Metrics;

namespace Jalpan.Metrics.OpenTelemetry;

public interface IMetrics
{
    Counter<T> Counter<T>(string key) where T : struct;
    Histogram<T> Histogram<T>(string key) where T : struct;
    ObservableGauge<T> ObservableGauge<T>(string key, Func<Measurement<T>> measurement) where T : struct;
}