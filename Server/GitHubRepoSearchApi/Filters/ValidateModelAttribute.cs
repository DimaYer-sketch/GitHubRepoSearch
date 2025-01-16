using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Server.Filters
{
    /// <summary>
    /// Validates the incoming request model.
    /// If the model is invalid, returns a 400 Bad Request response.
    /// </summary>
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Executes before the action method is invoked, validating the model state.
        /// </summary>
        /// <param name="context">The action execution context.</param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Check if the model state is valid
            if (!context.ModelState.IsValid)
            {
                // Return a 400 Bad Request response with validation errors
                context.Result = new BadRequestObjectResult(context.ModelState);
            }
        }
    }
}
