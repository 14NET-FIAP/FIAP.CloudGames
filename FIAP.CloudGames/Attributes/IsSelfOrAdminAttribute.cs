using FIAP.CloudGames.API.Extensions;
using FIAP.CloudGames.API.Middlewares;
using FIAP.CloudGames.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace FIAP.CloudGames.API.Attributes;

// Libera a action para Administradores ou para o próprio usuário cujo id está na rota.
public class IsSelfOrAdminAttribute : ActionFilterAttribute
{
    private const string UserIdRouteKey = "id";
    private const string SelfOrAdminOnlyMessage = "Apenas o próprio usuário ou Administradores podem executar essa função";

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        ClaimsPrincipal user = context.HttpContext.User;

        if (IsAdmin(user) || IsSameUser(user, context.RouteData.Values[UserIdRouteKey]?.ToString()))
        {
            base.OnActionExecuting(context);
            return;
        }

        context.Result = CreateForbiddenResult(context.HttpContext);
    }

    private static bool IsAdmin(ClaimsPrincipal user) =>
        Enum.TryParse<TypeUserEnum>(user.FindFirst(ClaimTypes.Actor)?.Value, out var userType)
        && userType == TypeUserEnum.Admin;

    private static bool IsSameUser(ClaimsPrincipal user, string? routeUserId) =>
        Guid.TryParse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value, out Guid tokenUserId)
        && Guid.TryParse(routeUserId, out Guid requestedUserId)
        && tokenUserId == requestedUserId;

    private static ObjectResult CreateForbiddenResult(HttpContext httpContext)
    {
        string traceId = httpContext.GetTraceId();

        // Atributos de filtro não recebem DI pelo construtor; o logger vem do escopo da requisição.
        httpContext.RequestServices.GetRequiredService<ILogger<IsSelfOrAdminAttribute>>()
            .LogWarning("Acesso negado em {Method} {Path}: usuário não é o dono do recurso nem Administrador. TraceId: {TraceId}",
                httpContext.Request.Method, httpContext.Request.Path, traceId);

        return new ObjectResult(ErrorResult.Failure(SelfOrAdminOnlyMessage, traceId))
        {
            StatusCode = StatusCodes.Status403Forbidden
        };
    }
}
