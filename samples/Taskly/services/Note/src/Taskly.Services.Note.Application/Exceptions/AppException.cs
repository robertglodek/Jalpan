namespace Taskly.Services.Note.Application.Exceptions;

public abstract class AppException(string message) : Exception(message)
{
    public abstract string Code { get; }
}