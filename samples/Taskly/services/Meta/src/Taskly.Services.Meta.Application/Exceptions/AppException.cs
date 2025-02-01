namespace Taskly.Services.Meta.Application.Exceptions;

public abstract class AppException(string message) : Exception(message)
{
    public abstract string Code { get; }
}