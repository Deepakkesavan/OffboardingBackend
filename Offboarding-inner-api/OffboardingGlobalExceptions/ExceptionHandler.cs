using Offboarding_inner_api.OffboardingGlobalExceptions.Exceptions;
using Offboarding_inner_api.OffboardingGlobalExceptions.Model;
using System.Text.Json;

namespace Offboarding_inner_api.OffboardingGlobalExceptions
{
    public class ExceptionHandler
    {
        private readonly RequestDelegate _next;

        public ExceptionHandler(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                BaseException baseException = MapExceptionToResponse(ex);

                context.Response.StatusCode = baseException.StatusCode;
                context.Response.ContentType = "application/json";

                var baseResponse = new BaseResponse<object>
                {
                    Errors = new List<BaseException> { baseException },
                    Result = null
                };

                var json = JsonSerializer.Serialize(baseResponse);
                await context.Response.WriteAsync(json);
            }
        }

        private static BaseException MapExceptionToResponse(Exception ex)
        {
            BaseException baseException = new BaseException();
            switch (ex)
            {
                case NotFoundException notFoundException:
                    baseException.StatusCode = StatusCodes.Status404NotFound;
                    baseException.Message = notFoundException.Message;
                    baseException.StackTrace = ex.StackTrace;
                    baseException.InnerException = ex.InnerException?.Message;
                    break;

                case ConflictException conflictException:
                    baseException.StatusCode = StatusCodes.Status409Conflict;
                    baseException.Message = conflictException.Message;
                    baseException.StackTrace = ex.StackTrace;
                    baseException.InnerException = ex.InnerException?.Message;
                    break;
                case BadRequestException badRequestException:
                    baseException.StatusCode = StatusCodes.Status400BadRequest;
                    baseException.Message = badRequestException.Message;
                    break;
                default:
                    baseException.StatusCode = StatusCodes.Status500InternalServerError;
                    baseException.Message = ex.Message;
                    baseException.StackTrace = ex.StackTrace;
                    baseException.InnerException = ex.InnerException?.Message;
                    break;

            }
            return baseException;
        }
    }
}
