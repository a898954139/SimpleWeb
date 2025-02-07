using Serilog.Context;

namespace Seq.Middleware;

internal class RequestLogContextMiddleware(
    RequestDelegate next)
{
    public Task InvokeAsync(HttpContext context)
    {
        using (LogContext.PushProperty("CorrelationId", context.TraceIdentifier))
        { 
            return next(context);
        }
    }
}