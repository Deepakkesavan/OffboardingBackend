using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Offboarding_inner_api.OffboardingGlobalExceptions.Model;

namespace Offboarding_inner_api.Middleware
{
    public class UnifiedResponseFilter: IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.Result is ObjectResult objectResult)
            {
                var response = new BaseResponse<object>
                {
                    Result = objectResult.Value
                };

                context.Result = new JsonResult(response)
                {
                    StatusCode = objectResult.StatusCode ?? StatusCodes.Status200OK
                };
            }
            else if (context.Result is EmptyResult)
            {
                context.Result = new JsonResult(new BaseResponse<object>())
                {
                    StatusCode = StatusCodes.Status204NoContent
                };
            }
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            
        }
    }
    
}
