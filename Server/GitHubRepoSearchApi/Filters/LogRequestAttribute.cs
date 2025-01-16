using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Server.Filters
{
    /// <summary>
    /// Logs details about incoming HTTP requests.
    /// </summary>
    public class LogRequestAttribute : ActionFilterAttribute
    {
        private readonly ILogger<LogRequestAttribute> _logger;

        public LogRequestAttribute(ILogger<LogRequestAttribute> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Executes before the action method is invoked, logging request details.
        /// </summary>
        /// <param name="context">The action execution context.</param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var request = context.HttpContext.Request;

            // Log HTTP method and request path
            _logger.LogInformation("Incoming request: {Method} {Path}", request.Method, request.Path);

            // Log query parameters, if any
            if (request.Query.Any())
            {
                foreach (var (key, value) in request.Query)
                {
                    _logger.LogInformation("Query parameter: {Key} = {Value}", key, value);
                }
            }

            base.OnActionExecuting(context);
        }
    }
}
