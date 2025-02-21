using AuthApi.Domain;

namespace AuthApi.Config
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ApiError ex) // Captura erros customizados
            {
                await HandleExceptionAsync(context, ex);
            }
            catch (Exception ex) // Captura erros genéricos
            {
                await HandleExceptionAsync(context, new ApiError(ex.Message, 400));
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, ApiError error)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = error.Status;
            return context.Response.WriteAsync(error.ToJson());
        }
    }
}