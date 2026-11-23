using MongoDB.Bson;

namespace FIAP.CloudGames.Infra.Logging;

public sealed class MongoLogDocument
{
    public ObjectId Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string Level { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? TraceId { get; set; }
    public BsonDocument Properties { get; set; } = new();
    public string? ExceptionType { get; set; }
    public string? Exception { get; set; }
}
