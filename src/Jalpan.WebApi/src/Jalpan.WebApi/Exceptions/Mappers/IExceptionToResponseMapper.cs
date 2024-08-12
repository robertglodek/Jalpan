namespace Jalpan.WebApi.Exceptions.Mappers;

public interface IExceptionToResponseMapper
{
    ExceptionResponse? Map(Exception exception);
}