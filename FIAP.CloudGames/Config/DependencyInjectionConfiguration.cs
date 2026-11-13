using FIAP.CloudGames.Application.Interfaces;
using FIAP.CloudGames.Application.Services;
using FIAP.CloudGames.DataBase.Repositories;
using FIAP.CloudGames.Domain.Repositories;

namespace FIAP.CloudGames.API.Config;

public static class DependencyInjectionConfiguration
{
    public static IServiceCollection AddDependencyInjection(this IServiceCollection services)
    {
        services.AddHttpClient();

        #region Services

        services.AddScoped<IUsersService, UsersService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IGamesService, GamesService>();

        #endregion

        #region Repositorys

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IGameRepository, GameRepository>();
        services.AddScoped<IPromotionRepository, PromotionRepository>();


        #endregion 

        return services;
    }
}
