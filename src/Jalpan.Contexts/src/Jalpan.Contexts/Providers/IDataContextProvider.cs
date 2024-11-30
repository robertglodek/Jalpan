namespace Jalpan.Contexts.Providers;

public interface IDataContextProvider<T>
{
    DataContext<T> Current();
}