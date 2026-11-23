using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using System.Diagnostics;
using System.Globalization;

namespace FIAP.CloudGames.Infra.Logging;

public sealed class MongoLogger : ILogger
{
    private const string OriginalFormatKey = "{OriginalFormat}";

    private readonly string _categoryName;
    private readonly MongoLogChannel _channel;

    public MongoLogger(string categoryName, MongoLogChannel channel)
    {
        _categoryName = categoryName;
        _channel = channel;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        _channel.Write(new MongoLogDocument
        {
            Timestamp = DateTime.UtcNow,
            Level = logLevel.ToString(),
            Category = _categoryName,
            Message = formatter(state, exception),
            // Mesmo TraceId devolvido no ErrorResult: correlaciona chamada do endpoint, comandos SQL e erro.
            TraceId = Activity.Current?.TraceId.ToString(),
            Properties = ToProperties(state),
            ExceptionType = exception?.GetType().FullName,
            Exception = exception?.ToString()
        });
    }

    private static BsonDocument ToProperties<TState>(TState state)
    {
        var properties = new BsonDocument();

        if (state is not IEnumerable<KeyValuePair<string, object?>> stateProperties)
            return properties;

        foreach (KeyValuePair<string, object?> property in stateProperties)
        {
            if (property.Key == OriginalFormatKey)
                continue;

            properties[property.Key] = ToBsonValue(property.Value);
        }

        return properties;
    }

    private static BsonValue ToBsonValue(object? value) => value switch
    {
        null => BsonNull.Value,
        string text => new BsonString(text),
        bool boolean => new BsonBoolean(boolean),
        int number => new BsonInt32(number),
        long number => new BsonInt64(number),
        double number => new BsonDouble(number),
        float number => new BsonDouble(number),
        decimal number => new BsonDecimal128(number),
        DateTime dateTime => new BsonDateTime(dateTime),
        DateTimeOffset dateTimeOffset => new BsonDateTime(dateTimeOffset.UtcDateTime),
        IFormattable formattable => new BsonString(formattable.ToString(null, CultureInfo.InvariantCulture)),
        _ => new BsonString(value.ToString())
    };
}
