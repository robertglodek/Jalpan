using System.Diagnostics.Metrics;

namespace Jalpan.Metrics.OpenTelemetry;

public class ObservableValue<T> where T : struct
{
    private T _value;

    public ObservableValue(IMetrics metrics, string name)
    {
        metrics.ObservableGauge(name, () => new Measurement<T>(_value));
    }

    public void Update(T value) => _value = value;
}