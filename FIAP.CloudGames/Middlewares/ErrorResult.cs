using FIAP.CloudGames.Domain.Services;

namespace FIAP.CloudGames.API.Middlewares;

public class ErrorResult : Result
{
    public string TraceId { get; set; } = string.Empty;

    public static ErrorResult Failure(string errorMessage, string traceId) =>
        new ErrorResult { IsSuccess = false, Message = errorMessage, TraceId = traceId };
}
