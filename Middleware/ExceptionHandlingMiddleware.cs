using IpDeputyApi.Dtos;
using IpDeputyApi.Exceptions;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text.Json;

namespace IpDeputyApi.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private static Serilog.ILogger Logger => Serilog.Log.ForContext<ExceptionHandlingMiddleware>();
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
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = context.Response;

            var errorDto = new ErrorDto();

            Logger.Error(exception, "Handle Exception");

            switch (exception)
            {
                case ApplicationException:
                    errorDto.Code = StatusCodes.Status400BadRequest.ToString();
                    errorDto.Message = "Application Exception Occured, please retry after sometime";
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    break;
                case NotImplementedException:
                    errorDto.Code = StatusCodes.Status403Forbidden.ToString();
                    errorDto.Message = "NotImplementedException";
                    response.StatusCode = StatusCodes.Status403Forbidden;
                    break;
                case HttpException:
                    var httpException = (HttpException)exception;
                    errorDto.Code = httpException.StatusCode.ToString();
                    errorDto.Message = httpException.Message;
                    errorDto.Data = httpException.Data;
                    response.StatusCode = httpException.StatusCode;
                    break;
                default:
                    errorDto.Code = StatusCodes.Status500InternalServerError.ToString();
                    errorDto.Message = "Internal Server Error, Please retry after sometime";
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            await context.Response.WriteAsync(JsonSerializer.Serialize(errorDto));
        }
    }
}
