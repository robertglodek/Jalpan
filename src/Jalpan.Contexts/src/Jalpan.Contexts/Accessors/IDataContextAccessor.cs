namespace Jalpan.Contexts.Accessors;

public interface IDataContextAccessor<T>
{
    DataContext<T>? DataContext { get; set; }
}