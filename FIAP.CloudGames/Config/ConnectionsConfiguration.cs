using FIAP.CloudGames.DataBase.Context;
using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver.Core.Configuration;

namespace FIAP.CloudGames.API.Config;

public static class ConnectionsConfiguration
{
    public static IServiceCollection AddAppConnections(
    this IServiceCollection services,
    IConfiguration configuration)
    {
        //var mongoConnectionString = configuration["MongoDb:ConnectionString"];
        //var databaseName = configuration["MongoDb:DatabaseName"]!;

        //services.AddDbContext<AppDbContext>(options => options.UseMongoDB(mongoConnectionString, databaseName));

        var connectionString = configuration.GetConnectionString("SqlServer")
            ?? throw new InvalidOperationException("Connection string 'SqlServer' não encontrada.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure()));


        return services;
    }
}
