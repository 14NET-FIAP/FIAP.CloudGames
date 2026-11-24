using FIAP.CloudGames.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace FIAP.CloudGames.API.Attributes;

// Libera a action para Administradores ou para o próprio usuário cujo id está na rota.
public class IsSelfOrAdminAttribute : ActionFilterAttribute
{
    private const string UserIdRouteKey = "id";

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        ClaimsPrincipal user = context.HttpContext.User;

        if (IsAdmin(user) || IsSameUser(user, context.RouteData.Values[UserIdRouteKey]?.ToString()))
        {
            base.OnActionExecuting(context);
            return;
        }

        context.Result = new ObjectResult(new { message = "Apenas o próprio usuário ou Administradores podem executar essa função" })
        {
            StatusCode = StatusCodes.Status403Forbidden
        };
    }

    private static bool IsAdmin(ClaimsPrincipal user) =>
        Enum.TryParse<TypeUserEnum>(user.FindFirst(ClaimTypes.Actor)?.Value, out var userType)
        && userType == TypeUserEnum.Admin;

    private static bool IsSameUser(ClaimsPrincipal user, string? routeUserId) =>
        Guid.TryParse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value, out Guid tokenUserId)
        && Guid.TryParse(routeUserId, out Guid requestedUserId)
        && tokenUserId == requestedUserId;
}
