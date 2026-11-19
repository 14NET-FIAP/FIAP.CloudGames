using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System.Threading.Channels;

namespace FIAP.CloudGames.Infra.Logging;

public sealed class MongoLogWriterService : BackgroundService
{
    private const int MaxBatchSize = 100;
    private static readonly TimeSpan ShutdownFlushTimeout = TimeSpan.FromSeconds(5);
    private static readonly InsertManyOptions InsertOptions = new() { IsOrdered = false };

    private readonly MongoLogChannel _channel;
    private readonly IMongoCollection<MongoLogDocument> _collection;
    private readonly ILogger<MongoLogWriterService> _logger;

    public MongoLogWriterService(
        MongoLogChannel channel,
        IMongoCollection<MongoLogDocument> collection,
        ILogger<MongoLogWriterService> logger)
    {
        _channel = channel;
        _collection = collection;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        ChannelReader<MongoLogDocument> reader = _channel.Reader;

        try
        {
            while (await reader.WaitToReadAsync(stoppingToken))
                await WriteBatchAsync(ReadBatch(reader), stoppingToken);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            await FlushPendingAsync(reader);
        }
    }

    private async Task FlushPendingAsync(ChannelReader<MongoLogDocument> reader)
    {
        using var timeout = new CancellationTokenSource(ShutdownFlushTimeout);

        List<MongoLogDocument> batch = ReadBatch(reader);
        while (batch.Count > 0 && !timeout.IsCancellationRequested)
        {
            await WriteBatchAsync(batch, timeout.Token);
            batch = ReadBatch(reader);
        }
    }

    private static List<MongoLogDocument> ReadBatch(ChannelReader<MongoLogDocument> reader)
    {
        var batch = new List<MongoLogDocument>(MaxBatchSize);

        while (batch.Count < MaxBatchSize && reader.TryRead(out MongoLogDocument? document))
            batch.Add(document);

        return batch;
    }

    private async Task WriteBatchAsync(List<MongoLogDocument> batch, CancellationToken ct)
    {
        if (batch.Count == 0)
            return;

        try
        {
            await _collection.InsertManyAsync(batch, InsertOptions, ct);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Falha ao gravar {LogCount} logs na collection {CollectionName} do MongoDB; lote descartado",
                batch.Count, _collection.CollectionNamespace.CollectionName);
        }
    }
}
