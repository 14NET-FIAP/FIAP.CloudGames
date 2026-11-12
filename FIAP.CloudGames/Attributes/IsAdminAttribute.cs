using FIAP.CloudGames.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace FIAP.CloudGames.API.Attributes;

public class IsAdminAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var userTypeClaim = context.HttpContext.User.FindFirst(ClaimTypes.Actor)?.Value;

        if (!Enum.TryParse<TypeUserEnum>(userTypeClaim, out var userType) ||
            userType != TypeUserEnum.Admin)
        {
            context.Result = new ObjectResult(new { message = "Apenas Administradores podem executar essa função" })
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
            return;
        }

        base.OnActionExecuting(context);
    }
}
