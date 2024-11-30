namespace Jalpan.Contexts.Accessors;

public sealed class DataContextAccessor<T> : IDataContextAccessor<T>
{
    private static readonly AsyncLocal<DataContextHolder<T>> Holder = new();

    public DataContext<T>? DataContext
    {
        get => Holder.Value?.Context;
        set
        {
            var holder = Holder.Value;
            if (holder != null)
            {
                holder.Context = null;
            }

            if (value is not null)
            {
                Holder.Value = new DataContextHolder<T> { Context = value };
            }
        }
    }

    private class DataContextHolder<TValue>
    {
        public DataContext<TValue>? Context;
    }
}