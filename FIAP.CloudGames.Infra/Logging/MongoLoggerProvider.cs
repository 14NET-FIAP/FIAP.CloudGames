using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.Concurrent;

namespace FIAP.CloudGames.Infra.Logging;

[ProviderAlias("MongoDb")]
public sealed class MongoLoggerProvider : ILoggerProvider, ISupportExternalScope
{
    private static readonly string WriterCategoryName = typeof(MongoLogWriterService).FullName!;

    private readonly MongoLogChannel _channel;
    private readonly ConcurrentDictionary<string, ILogger> _loggers = new();
    private IExternalScopeProvider? _scopeProvider;

    public MongoLoggerProvider(MongoLogChannel channel) => _channel = channel;

    public ILogger CreateLogger(string categoryName)
    {
        if (categoryName == WriterCategoryName)
            return NullLogger.Instance;

        return _loggers.GetOrAdd(categoryName, name => new MongoLogger(name, _channel, () => _scopeProvider));
    }

    public void SetScopeProvider(IExternalScopeProvider scopeProvider) => _scopeProvider = scopeProvider;

    public void Dispose() => _loggers.Clear();
}
