using FIAP.CloudGames.Domain.Exceptions;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Text.Json;
using MvcJsonOptions = Microsoft.AspNetCore.Mvc.JsonOptions;

namespace FIAP.CloudGames.API.Middlewares;

public class ExceptionHandlingMiddleware
{
    private const int StatusCodeClientClosedRequest = 499;
    private const string UnexpectedErrorMessage = "Ocorreu um erro inesperado. Informe o TraceId ao suporte.";

    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IOptions<MvcJsonOptions> jsonOptions)
    {
        _next = next;
        _logger = logger;
        _jsonOptions = jsonOptions.Value.JsonSerializerOptions;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            _logger.LogInformation("Requisição {Method} {Path} cancelada pelo cliente",
                context.Request.Method, context.Request.Path);
            context.Response.StatusCode = StatusCodeClientClosedRequest;
        }
        catch (Exception exception) when (!context.Response.HasStarted)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        string traceId = Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;
        (int statusCode, string message) = MapException(exception);

        LogException(context, exception, statusCode, traceId);

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(ErrorResult.Failure(message, traceId), _jsonOptions);
    }

    private static (int StatusCode, string Message) MapException(Exception exception) => exception switch
    {
        DomainException => (StatusCodes.Status400BadRequest, exception.Message),
        KeyNotFoundException => (StatusCodes.Status404NotFound, exception.Message),
        _ => (StatusCodes.Status500InternalServerError, UnexpectedErrorMessage)
    };

    private void LogException(HttpContext context, Exception exception, int statusCode, string traceId)
    {
        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Erro não tratado em {Method} {Path}. TraceId: {TraceId}",
                context.Request.Method, context.Request.Path, traceId);
            return;
        }

        _logger.LogWarning("Requisição {Method} {Path} rejeitada com {StatusCode} ({ExceptionType}): {ErrorMessage}. TraceId: {TraceId}",
            context.Request.Method, context.Request.Path, statusCode, exception.GetType().Name, exception.Message, traceId);
    }
}
