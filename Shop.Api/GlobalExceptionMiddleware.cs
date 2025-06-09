using CorrelationId.Abstractions;
using Newtonsoft.Json;
using Formatting = System.Xml.Formatting;

namespace Shop.Api;

public class GlobalExceptionMiddleware(RequestDelegate next, ICorrelationContextAccessor correlationContextAccessor)
{
    public async Task Invoke(HttpContext context, IWebHostEnvironment env, ILoggerFactory loggerFactory)
    {
        try
        {
            await next.Invoke(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex, env,
                correlationContextAccessor.CorrelationContext.CorrelationId, loggerFactory.CreateLogger(ex.Source));
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception,
        IWebHostEnvironment env, string correlationId, ILogger logger)
    {
        var code = StatusCodes.Status500InternalServerError;
        var prodMessage = string.Empty;
        var correlationIdMessage = $"Request CorrelationId is '{correlationId}'.";

        logger?.LogError(exception, $"{correlationIdMessage}:{exception.Message}");

        var result = env.IsDevelopment() ?
            JsonConvert.SerializeObject(new { error = exception.Message, correlationId, stackTrace = exception.StackTrace }, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }) :
            JsonConvert.SerializeObject(new { error = $"Error occured while processing your request. {correlationIdMessage}{prodMessage}" });

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = code;
        return context.Response.WriteAsync(result);
    }
}
