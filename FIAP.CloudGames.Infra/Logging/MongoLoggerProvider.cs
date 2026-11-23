using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.Concurrent;

namespace FIAP.CloudGames.Infra.Logging;

[ProviderAlias("MongoDb")]
public sealed class MongoLoggerProvider : ILoggerProvider
{
    private static readonly string WriterCategoryName = typeof(MongoLogWriterService).FullName!;

    private readonly MongoLogChannel _channel;
    private readonly ConcurrentDictionary<string, ILogger> _loggers = new();

    public MongoLoggerProvider(MongoLogChannel channel) => _channel = channel;

    public ILogger CreateLogger(string categoryName)
    {
        // Evita recursão: falhas do próprio writer não podem voltar para a fila do Mongo.
        if (categoryName == WriterCategoryName)
            return NullLogger.Instance;

        return _loggers.GetOrAdd(categoryName, name => new MongoLogger(name, _channel));
    }

    public void Dispose() => _loggers.Clear();
}
