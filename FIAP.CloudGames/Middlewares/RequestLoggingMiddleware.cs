using System.Diagnostics;
using System.Security.Claims;

namespace FIAP.CloudGames.API.Middlewares;

public class RequestLoggingMiddleware
{
    private const string AnonymousUser = "anonymous";

    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        long startTimestamp = Stopwatch.GetTimestamp();

        try
        {
            await _next(context);
        }
        finally
        {
            LogRequest(context, Stopwatch.GetElapsedTime(startTimestamp));
        }
    }

    private void LogRequest(HttpContext context, TimeSpan elapsed)
    {
        string userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? AnonymousUser;

        _logger.LogInformation("HTTP {Method} {Path} respondeu {StatusCode} em {ElapsedMilliseconds} ms. UserId: {UserId}",
            context.Request.Method,
            context.Request.Path.Value,
            context.Response.StatusCode,
            Math.Round(elapsed.TotalMilliseconds, 2),
            userId);
    }
}
