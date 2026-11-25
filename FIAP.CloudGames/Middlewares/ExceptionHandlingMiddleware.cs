using FIAP.CloudGames.API.Extensions;
using FIAP.CloudGames.Domain.Exceptions;
using Microsoft.Extensions.Options;
using System.Text.Json;
using MvcJsonOptions = Microsoft.AspNetCore.Mvc.JsonOptions;

namespace FIAP.CloudGames.API.Middlewares;

public class ExceptionHandlingMiddleware
{
    private const int StatusCodeClientClosedRequest = 499;
    private const string UnexpectedErrorMessage = "Ocorreu um erro inesperado. Informe o TraceId ao suporte.";
    private const string BadRequestMessage = "Requisição inválida.";
    private const string UnauthorizedMessage = "Autenticação necessária. Informe um token válido.";
    private const string ForbiddenMessage = "Você não tem permissão para acessar este recurso.";
    private const string NotFoundMessage = "Recurso não encontrado.";
    private const string MethodNotAllowedMessage = "Método HTTP não permitido para este recurso.";
    private const string UnsupportedMediaTypeMessage = "Tipo de conteúdo não suportado.";
    private const string DefaultClientErrorMessage = "Não foi possível processar a requisição.";

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
            await HandleErrorResponseWithoutBodyAsync(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            HandleClientCancellation(context);
        }
        catch (Exception exception) when (context.Response.HasStarted)
        {
            _logger.LogError(exception, "Erro após o início da resposta em {Method} {Path}. TraceId: {TraceId}",
                context.Request.Method, context.Request.Path, context.GetTraceId());
            throw;
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private void HandleClientCancellation(HttpContext context)
    {
        _logger.LogInformation("Requisição {Method} {Path} cancelada pelo cliente",
            context.Request.Method, context.Request.Path);

        if (context.Response.HasStarted)
            return;

        context.Response.StatusCode = StatusCodeClientClosedRequest;
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        string traceId = context.GetTraceId();
        (int statusCode, string message) = MapException(exception);

        LogException(context, exception, statusCode, traceId);

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        await WriteErrorResultAsync(context.Response, message, traceId);
    }

    private async Task HandleErrorResponseWithoutBodyAsync(HttpContext context)
    {
        if (!IsErrorResponseWithoutBody(context.Response))
            return;

        string traceId = context.GetTraceId();
        int statusCode = context.Response.StatusCode;

        LogErrorResponseWithoutBody(context, statusCode, traceId);

        // Sem Response.Clear(): preserva headers como WWW-Authenticate (401) e Allow (405).
        await WriteErrorResultAsync(context.Response, MapStatusCodeToMessage(statusCode), traceId);
    }

    private static bool IsErrorResponseWithoutBody(HttpResponse response) =>
        !response.HasStarted
        && response.StatusCode >= StatusCodes.Status400BadRequest
        && response.ContentLength is null
        && string.IsNullOrEmpty(response.ContentType);

    private Task WriteErrorResultAsync(HttpResponse response, string message, string traceId) =>
        response.WriteAsJsonAsync(ErrorResult.Failure(message, traceId), _jsonOptions);

    private static (int StatusCode, string Message) MapException(Exception exception) => exception switch
    {
        DomainException => (StatusCodes.Status400BadRequest, exception.Message),
        KeyNotFoundException => (StatusCodes.Status404NotFound, exception.Message),
        _ => (StatusCodes.Status500InternalServerError, UnexpectedErrorMessage)
    };

    private static string MapStatusCodeToMessage(int statusCode) => statusCode switch
    {
        StatusCodes.Status400BadRequest => BadRequestMessage,
        StatusCodes.Status401Unauthorized => UnauthorizedMessage,
        StatusCodes.Status403Forbidden => ForbiddenMessage,
        StatusCodes.Status404NotFound => NotFoundMessage,
        StatusCodes.Status405MethodNotAllowed => MethodNotAllowedMessage,
        StatusCodes.Status415UnsupportedMediaType => UnsupportedMediaTypeMessage,
        >= StatusCodes.Status500InternalServerError => UnexpectedErrorMessage,
        _ => DefaultClientErrorMessage
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

    private void LogErrorResponseWithoutBody(HttpContext context, int statusCode, string traceId)
    {
        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            _logger.LogError("Requisição {Method} {Path} finalizada com {StatusCode} sem corpo. TraceId: {TraceId}",
                context.Request.Method, context.Request.Path, statusCode, traceId);
            return;
        }

        _logger.LogWarning("Requisição {Method} {Path} rejeitada com {StatusCode}. TraceId: {TraceId}",
            context.Request.Method, context.Request.Path, statusCode, traceId);
    }
}
