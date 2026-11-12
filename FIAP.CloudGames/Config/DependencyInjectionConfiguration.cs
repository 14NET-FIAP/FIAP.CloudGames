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

        #endregion

        #region Repositorys

        services.AddScoped<IUserRepository, UserRepository>();

        #endregion 

        return services;
    }
}
