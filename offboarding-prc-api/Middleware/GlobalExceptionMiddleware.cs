namespace offboarding_prc_api.Middleware;
using System.Net;
using System.Text.Json;

public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception on {Method} {Path}",
                context.Request.Method, context.Request.Path);

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var body = JsonSerializer.Serialize(new
            {
                message = "An unexpected error occurred. Please try again.",
                detail = ex.Message   // remove this line in production
            });

            await context.Response.WriteAsync(body);
        }
    }
}
