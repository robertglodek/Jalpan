namespace Jalpan;

public interface IStartupInitializer : IInitializer
{
    /// <summary>
    /// Add new initializer.
    /// </summary>
    /// <param name="initializer">Initializer to be added.</param>
    void AddInitializer(IInitializer initializer);
}
