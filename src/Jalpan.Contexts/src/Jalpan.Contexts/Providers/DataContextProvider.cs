using Jalpan.Contexts.Accessors;

namespace Jalpan.Contexts.Providers;

internal sealed class DataContextProvider<T>(IContextProvider contextProvider, IDataContextAccessor<T> dataContextAccessor)
    : IDataContextProvider<T>
{
    public DataContext<T> Current()
    {
        if (dataContextAccessor.DataContext is not null)
        {
            return dataContextAccessor.DataContext;
        }
        var dataContext = new DataContext<T>(default, contextProvider.Current());
        dataContextAccessor.DataContext = dataContext;
        return dataContext;
    }
}