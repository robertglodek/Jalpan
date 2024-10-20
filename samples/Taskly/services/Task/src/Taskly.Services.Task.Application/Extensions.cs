using Jalpan;
using Jalpan.Contexts;

namespace Taskly.Services.Task.Application;

public static class Extensions
{
    public static IJalpanBuilder AddApplication(this IJalpanBuilder builder)
        => builder.AddContexts();
}
