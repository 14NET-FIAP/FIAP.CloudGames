using FIAP.CloudGames.Infra.Logging;
using MongoDB.Driver;

namespace FIAP.CloudGames.API.Config;

public static class LoggingConfiguration
{
    private const string DefaultLogsCollectionName = "ApplicationLogs";

    public static IServiceCollection AddMongoDbLogging(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration["MongoDb:ConnectionString"]
            ?? throw new InvalidOperationException("Configuração 'MongoDb:ConnectionString' não encontrada.");
        var databaseName = configuration["MongoDb:DatabaseName"]
            ?? throw new InvalidOperationException("Configuração 'MongoDb:DatabaseName' não encontrada.");
        var collectionName = configuration["MongoDb:LogsCollectionName"] ?? DefaultLogsCollectionName;

        services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString));
        services.AddSingleton(provider => provider
            .GetRequiredService<IMongoClient>()
            .GetDatabase(databaseName)
            .GetCollection<MongoLogDocument>(collectionName));

        services.AddSingleton<MongoLogChannel>();
        services.AddSingleton<ILoggerProvider, MongoLoggerProvider>();
        services.AddHostedService<MongoLogWriterService>();

        return services;
    }
}
