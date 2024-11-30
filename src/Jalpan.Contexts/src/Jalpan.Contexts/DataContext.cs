namespace Jalpan.Contexts;

public record DataContext<T>(T? Data, IContext Context);