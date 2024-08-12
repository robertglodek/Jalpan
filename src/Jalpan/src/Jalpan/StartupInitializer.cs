namespace Jalpan;

public class StartupInitializer : IStartupInitializer
{
    private readonly IList<IInitializer> _initializers = [];

    public void AddInitializer(IInitializer initializer)
    {
        if (initializer == null) throw new ArgumentNullException(nameof(initializer));
        if (_initializers.Contains(initializer)) return;

        _initializers.Add(initializer);
    }

    public async Task InitializeAsync()
    {
        foreach (var initializer in _initializers)
            await initializer.InitializeAsync();
    }
}
