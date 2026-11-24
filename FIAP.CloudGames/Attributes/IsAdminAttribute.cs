using FIAP.CloudGames.API.Extensions;
using FIAP.CloudGames.API.Middlewares;
using FIAP.CloudGames.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace FIAP.CloudGames.API.Attributes;

public class IsAdminAttribute : ActionFilterAttribute
{
    private const string AdminOnlyMessage = "Apenas Administradores podem executar essa função";

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var userTypeClaim = context.HttpContext.User.FindFirst(ClaimTypes.Actor)?.Value;

        if (!Enum.TryParse<TypeUserEnum>(userTypeClaim, out var userType) ||
            userType != TypeUserEnum.Admin)
        {
            context.Result = CreateForbiddenResult(context.HttpContext);
            return;
        }

        base.OnActionExecuting(context);
    }

    private static ObjectResult CreateForbiddenResult(HttpContext httpContext)
    {
        string traceId = httpContext.GetTraceId();

        // Atributos de filtro não recebem DI pelo construtor; o logger vem do escopo da requisição.
        httpContext.RequestServices.GetRequiredService<ILogger<IsAdminAttribute>>()
            .LogWarning("Acesso negado em {Method} {Path}: usuário não é Administrador. TraceId: {TraceId}",
                httpContext.Request.Method, httpContext.Request.Path, traceId);

        return new ObjectResult(ErrorResult.Failure(AdminOnlyMessage, traceId))
        {
            StatusCode = StatusCodes.Status403Forbidden
        };
    }
}
