using FIAP.CloudGames.DataBase.Context;
using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver.Core.Configuration;
using FIAP.CloudGames.API.Middlewares;

namespace FIAP.CloudGames.API.Config;

public static class ApplicationExtentionsConfig
{
    public static IApplicationBuilder UseApiMiddlewares(this IApplicationBuilder app)
    {
        app.UseMiddleware<RequestLoggingMiddleware>();
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        return app;
    }
}