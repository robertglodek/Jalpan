using Jalpan;
using Jalpan.Logging.Serilog;

namespace ShelfShare.Services.Inventory.Infrastructure;

public static class Extensions
{
    public static IJalpanBuilder AddInfrastructure(this IJalpanBuilder builder)
        => builder.AddLogger();
}
