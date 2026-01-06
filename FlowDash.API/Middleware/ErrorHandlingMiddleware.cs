using FlowDash.Application.Exceptions.Client;
using FlowDash.Application.Exceptions.Server;
using FlowDash.Contract.Error;
using System.ComponentModel.DataAnnotations;

namespace FlowDash.API.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        private readonly IHostEnvironment _environment;

        public ErrorHandlingMiddleware
        (
            RequestDelegate next,
            ILogger<ErrorHandlingMiddleware> logger,
            IHostEnvironment environment
        )
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Unhandled exception | TraceId: {TraceId}",
                    context.TraceIdentifier);

                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var statusCode = exception switch
            {
                ValidationException => StatusCodes.Status400BadRequest,
                UnauthorizedException => StatusCodes.Status401Unauthorized,
                ForbiddenException => StatusCodes.Status403Forbidden,
                NotFoundException => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status500InternalServerError
            };

            var response = new ErrorResponse
            (
                statusCode,
                GetSafeMessage(exception),
                context.TraceIdentifier
            );

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            await context.Response.WriteAsJsonAsync(response);
        }

        private string GetSafeMessage(Exception exception)
        {
            if (exception is AppException)
                return exception.Message;

            return _environment.IsDevelopment()
                ? exception.Message
                : "An unexpected error occurred";
        }
    }
}
