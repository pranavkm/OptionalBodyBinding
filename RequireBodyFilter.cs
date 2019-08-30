using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace OptionalBody
{
    public class RequireBodyFilter : ActionFilterAttribute
    {
        public RequireBodyFilter()
        {
            // Run before the ModelStateInvalidFilter
            Order = -2001;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var controllerActionDescriptor = (ControllerActionDescriptor)context.ActionDescriptor;
            var methodParameters = controllerActionDescriptor.MethodInfo.GetParameters();
            for (var i = 0; i < context.ActionDescriptor.Parameters.Count; i++)
            {
                var parameter = context.ActionDescriptor.Parameters[i];
                if (parameter.BindingInfo?.BindingSource != BindingSource.Body)
                {
                    continue;
                }

                if (methodParameters[i].HasDefaultValue)
                {
                    continue;
                }

                var boundValue = context.ActionArguments[parameter.Name];
                if (boundValue == null)
                {
                    context.ModelState.AddModelError(parameter.Name, "Request body is required.");
                    break;
                }
            }
        }
    }
}
