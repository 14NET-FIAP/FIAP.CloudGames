using FIAP.CloudGames.Application.Interfaces;
using FIAP.CloudGames.Application.Services;
using FIAP.CloudGames.DataBase.Repositories;
using FIAP.CloudGames.Domain.Repositories;
using FIAP.CloudGames.Infra.Data;

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
        services.AddScoped<IPromotionService, PromotionService>();

        #endregion

        #region Repositorys

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IGameRepository, GameRepository>();
        services.AddScoped<IPromotionRepository, PromotionRepository>();
        
        
        services.AddScoped<ISqlServerConnectionFactory, SqlServerConnectionFactory>();


        #endregion 

        return services;
    }
}
