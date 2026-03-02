public class RequestContextMiddleware
{
    private readonly RequestDelegate _next;
    private const string SessionHeaderKey = "X-Session-Id";

    public RequestContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, RequestContext requestContext)
    {
        if (context.Request.Headers.TryGetValue(SessionHeaderKey, out var sessionId))
        {
            requestContext.SessionId = sessionId.ToString();
        }

        await _next(context);
    }
}