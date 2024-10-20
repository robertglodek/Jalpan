using Jalpan;
using Jalpan.Logging.Serilog;

namespace Taskly.Services.Task.Infrastructure;

public static class Extensions
{
    public static IJalpanBuilder AddInfrastructure(this IJalpanBuilder builder)
        => builder.AddLogger();
}
