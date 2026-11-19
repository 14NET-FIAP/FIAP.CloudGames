using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using System.Globalization;

namespace FIAP.CloudGames.Infra.Logging;

public sealed class MongoLogger : ILogger
{
    private const string OriginalFormatKey = "{OriginalFormat}";

    private readonly string _categoryName;
    private readonly MongoLogChannel _channel;
    private readonly Func<IExternalScopeProvider?> _scopeProviderAccessor;

    public MongoLogger(string categoryName, MongoLogChannel channel, Func<IExternalScopeProvider?> scopeProviderAccessor)
    {
        _categoryName = categoryName;
        _channel = channel;
        _scopeProviderAccessor = scopeProviderAccessor;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        var document = new MongoLogDocument
        {
            Timestamp = DateTime.UtcNow,
            Level = logLevel.ToString(),
            Category = _categoryName,
            EventId = eventId.Id,
            Message = formatter(state, exception),
            ExceptionType = exception?.GetType().FullName,
            Exception = exception?.ToString()
        };

        FillProperties(document, state);
        FillScopes(document);

        _channel.Write(document);
    }

    private static void FillProperties<TState>(MongoLogDocument document, TState state)
    {
        if (state is not IEnumerable<KeyValuePair<string, object?>> properties)
            return;

        foreach (KeyValuePair<string, object?> property in properties)
        {
            if (property.Key == OriginalFormatKey)
            {
                document.MessageTemplate = property.Value?.ToString();
                continue;
            }

            document.Properties[property.Key] = ToBsonValue(property.Value);
        }
    }

    private void FillScopes(MongoLogDocument document)
    {
        _scopeProviderAccessor()?.ForEachScope((scope, scopes) =>
        {
            if (scope is not IEnumerable<KeyValuePair<string, object?>> scopeProperties)
                return;

            foreach (KeyValuePair<string, object?> property in scopeProperties)
            {
                if (property.Key == OriginalFormatKey)
                    continue;

                scopes[property.Key] = ToBsonValue(property.Value);
            }
        }, document.Scopes);
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
