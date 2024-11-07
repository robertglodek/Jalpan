using System.Diagnostics.Metrics;

namespace Jalpan.Metrics.OpenTelemetry;

internal sealed class Metrics(string name) : IMetrics
{
    private readonly Meter _meter = new(name);

    public Counter<T> Counter<T>(string key) where T : struct => _meter.CreateCounter<T>(Key(key));
        
    public Histogram<T> Histogram<T>(string key) where T : struct => _meter.CreateHistogram<T>(Key(key));
    
    public ObservableGauge<T> ObservableGauge<T>(string key, Func<Measurement<T>> measurement) where T : struct
        => _meter.CreateObservableGauge(Key(key), measurement);

    private string Key(string key) => $"{name}_{key}";
}