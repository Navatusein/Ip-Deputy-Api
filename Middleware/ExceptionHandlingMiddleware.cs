using IpDeputyApi.Dto;
using IpDeputyApi.Exceptions;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Serilog;

namespace IpDeputyApi.Middleware;

public class ExceptionHandlingMiddleware
{
    private const int ClientClosedRequest = 499;

    private static Serilog.ILogger Logger => Log.ForContext<ExceptionHandlingMiddleware>();
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            if (!context.Response.HasStarted)
                context.Response.StatusCode = ClientClosedRequest;
        }
        catch (Exception exception)
        {
            if (context.Response.HasStarted)
            {
                Logger.Error(exception, "Unhandled exception after response started");
                throw;
            }

            await HandleExceptionAsync(context, exception);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var errorDto = exception switch
        {
            HttpException httpException => new ErrorDto(httpException.StatusCode, httpException.Message, httpException.ResponseData),
            DbUpdateException { InnerException: PostgresException { SqlState: PostgresErrorCodes.ForeignKeyViolation } } =>
                new ErrorDto(StatusCodes.Status409Conflict, "Запис пов'язаний з іншими даними"),
            DbUpdateException { InnerException: PostgresException { SqlState: PostgresErrorCodes.UniqueViolation } } =>
                new ErrorDto(StatusCodes.Status409Conflict, "Запис з такими даними вже існує"),
            PostgresException { SqlState: PostgresErrorCodes.ForeignKeyViolation } =>
                new ErrorDto(StatusCodes.Status409Conflict, "Запис пов'язаний з іншими даними"),
            PostgresException { SqlState: PostgresErrorCodes.UniqueViolation } =>
                new ErrorDto(StatusCodes.Status409Conflict, "Запис з такими даними вже існує"),
            NpgsqlException and not PostgresException or TimeoutException => new ErrorDto(StatusCodes.Status503ServiceUnavailable, "База даних тимчасово недоступна"),
            BadHttpRequestException badRequest => new ErrorDto(badRequest.StatusCode, "Некоректний запит"),
            _ => new ErrorDto(StatusCodes.Status500InternalServerError, "Внутрішня помилка сервера")
        };

        var statusCode = int.Parse(errorDto.Code);

        if (statusCode >= StatusCodes.Status500InternalServerError)
            Logger.Error(exception, "Unhandled exception");
        else
            Logger.Information("Request failed with {StatusCode}: {Message}", statusCode, exception.Message);

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(errorDto);
    }
}
