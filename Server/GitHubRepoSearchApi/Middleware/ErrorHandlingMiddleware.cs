using System.Net;
using System.Text.Json;

namespace Server.Middleware
{
    /// <summary>
    /// Middleware for handling unhandled exceptions in the application.
    /// </summary>
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Invokes the middleware to handle exceptions.
        /// </summary>
        /// <param name="context">The HTTP context for the current request.</param>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Pass the request to the next middleware or controller
                await _next(context);
            }
            catch (Exception ex)
            {
                // Handle any unhandled exceptions
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Handles the exception and writes a JSON error response.
        /// </summary>
        /// <param name="context">The HTTP context for the current request.</param>
        /// <param name="exception">The exception that occurred.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Log the exception details
            _logger.LogError(exception, "An unhandled exception occurred.");

            // Set the HTTP status code and content type
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            // Create the error response
            var errorResponse = new
            {
                message = "An unexpected error occurred.",
                details = exception.Message // Optional: remove in production for security
            };

            // Serialize and write the error response
            return context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }
    }
}
