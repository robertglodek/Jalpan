using Jalpan;
using Jalpan.Contexts;

namespace ShelfShare.Services.Inventory.Application;

public static class Extensions
{
    public static IJalpanBuilder AddApplication(this IJalpanBuilder builder)
        => builder.AddContexts();
}
