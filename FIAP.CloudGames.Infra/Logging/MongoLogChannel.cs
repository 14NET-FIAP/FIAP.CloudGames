using System.Threading.Channels;

namespace FIAP.CloudGames.Infra.Logging;

public sealed class MongoLogChannel
{
    private const int Capacity = 10_000;

    private readonly Channel<MongoLogDocument> _channel = Channel.CreateBounded<MongoLogDocument>(
        new BoundedChannelOptions(Capacity)
        {
            FullMode = BoundedChannelFullMode.DropOldest,
            SingleReader = true,
            SingleWriter = false
        });

    public ChannelReader<MongoLogDocument> Reader => _channel.Reader;

    public void Write(MongoLogDocument document) => _channel.Writer.TryWrite(document);
}
