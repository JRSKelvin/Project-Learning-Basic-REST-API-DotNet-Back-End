using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Project_Learning_Basic_REST_API_DotNet_Back_End.Models.Common;

namespace Project_Learning_Basic_REST_API_DotNet_Back_End.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError($"An Error Occurred While Processing Your Request");
            var errorResponse = new ErrorResponse
            {
                Message = exception.Message,
            };
            switch (exception)
            {
                case BadHttpRequestException:
                    errorResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.Title = exception.GetType().Name;
                    break;
                default:
                    errorResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.Title = "Internal Server Error";
                    break;
            }
            httpContext.Response.StatusCode = errorResponse.StatusCode;
            await httpContext.Response.WriteAsJsonAsync(errorResponse, cancellationToken);
            return true;
        }
    }
}
