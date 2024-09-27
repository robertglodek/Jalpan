using System.Diagnostics.Metrics;

namespace Jalpan.Metrics.OpenTelemetry;

internal sealed class Metrics(string name) : IMetrics
{
    private readonly string _name = name;
    private readonly Meter _meter = new(name);

    public Counter<T> Counter<T>(string name) where T : struct => _meter.CreateCounter<T>(Key(name));
        
    public Histogram<T> Histogram<T>(string name) where T : struct => _meter.CreateHistogram<T>(Key(name));
    
    public ObservableGauge<T> ObservableGauge<T>(string name, Func<Measurement<T>> measurement) where T : struct
        => _meter.CreateObservableGauge(Key(name), measurement);

    private string Key(string key) => $"{_name}_{key}";
}