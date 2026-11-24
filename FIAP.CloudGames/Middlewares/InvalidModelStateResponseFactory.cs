using FIAP.CloudGames.API.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FIAP.CloudGames.API.Middlewares;

// Não é estática porque o tipo é usado como categoria do ILogger<T> (allow-list do Mongo).
public class InvalidModelStateResponseFactory
{
    private const string ErrorMessageSeparator = "; ";
    private const string DefaultValidationErrorMessage = "Os dados enviados são inválidos.";

    public static IActionResult Create(ActionContext context)
    {
        HttpContext httpContext = context.HttpContext;
        string traceId = httpContext.GetTraceId();
        string message = BuildMessage(context.ModelState);

        // O delegate do ApiBehaviorOptions não recebe DI; o logger vem do escopo da requisição.
        ILogger<InvalidModelStateResponseFactory> logger =
            httpContext.RequestServices.GetRequiredService<ILogger<InvalidModelStateResponseFactory>>();

        logger.LogWarning("Requisição {Method} {Path} rejeitada com {StatusCode} por dados inválidos: {ErrorMessage}. TraceId: {TraceId}",
            httpContext.Request.Method, httpContext.Request.Path, StatusCodes.Status400BadRequest, message, traceId);

        return new BadRequestObjectResult(ErrorResult.Failure(message, traceId));
    }

    private static string BuildMessage(ModelStateDictionary modelState)
    {
        string[] errorMessages = modelState.Values
            .SelectMany(entry => entry.Errors)
            .Select(error => error.ErrorMessage)
            .Where(errorMessage => !string.IsNullOrWhiteSpace(errorMessage))
            .ToArray();

        return errorMessages.Length == 0
            ? DefaultValidationErrorMessage
            : string.Join(ErrorMessageSeparator, errorMessages);
    }
}
