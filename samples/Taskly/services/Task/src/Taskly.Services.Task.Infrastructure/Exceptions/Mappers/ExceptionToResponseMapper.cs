﻿using Jalpan;
using Jalpan.Validation;
using Jalpan.WebApi.Exceptions;
using Jalpan.WebApi.Exceptions.Mappers;
using System.Collections.Concurrent;
using System.Net;
using Taskly.Services.Task.Application.Exceptions;
using Taskly.Services.Task.Domain.Exceptions;

namespace Taskly.Services.Task.Infrastructure.Exceptions.Mappers;

[UsedImplicitly]
internal sealed class ExceptionToResponseMapper : IExceptionToResponseMapper
{
    private static readonly ConcurrentDictionary<Type, string> Codes = new();

    public ExceptionResponse Map(Exception exception)
        => exception switch
        {
            DomainException ex => new ExceptionResponse(new { code = GetCode(ex), reason = ex.Message },
                HttpStatusCode.BadRequest),
            AppException ex => new ExceptionResponse(new { code = GetCode(ex), reason = ex.Message },
                HttpStatusCode.BadRequest),
            ValidationException ex => new ExceptionResponse(
                new { code = "validation", reason = ex.Message, errors = ex.Errors }, HttpStatusCode.BadRequest),
            _ => new ExceptionResponse(new { code = "error", reason = "There was an error." },
                HttpStatusCode.BadRequest)
        };

    private static string GetCode(Exception exception)
    {
        var type = exception.GetType();
        if (Codes.TryGetValue(type, out var code))
        {
            return code;
        }

        var exceptionCode = exception switch
        {
            DomainException domainException when !string.IsNullOrWhiteSpace(domainException.Code) => domainException
                .Code,
            AppException appException when !string.IsNullOrWhiteSpace(appException.Code) => appException.Code,
            _ => exception.GetType().Name.Underscore().Replace("_exception", string.Empty)
        };

        Codes.TryAdd(type, exceptionCode);

        return exceptionCode;
    }
}
