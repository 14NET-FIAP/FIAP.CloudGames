namespace FIAP.CloudGames.Domain.Services;

public class Result<T>
{
    public T Value { get; set; }
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }

    public Result()
    {
        Value = default!;
        IsSuccess = true;
        Message = null;
    }
    public Result(T value)
    {
        Value = value;
        IsSuccess = true;
        Message = string.Empty;
    }

    public static Result<T> Success(T value) => new Result<T> { Value = value, IsSuccess = true, Message = string.Empty };

    public static Result<T> Failure(string errorMessage) => new Result<T>
    { IsSuccess = false, Message = errorMessage, Value = default! };
}

public class ResultList<T>
{
    public List<T> Value { get; set; }
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }

    public ResultList()
    {
        Value = new List<T>();
        IsSuccess = true;
        Message = string.Empty;
    }

    public ResultList(T value)
    {
        Value = new List<T> { value };
        IsSuccess = true;
        Message = string.Empty;
    }

    public ResultList(List<T> value)
    {
        Value = value;
        IsSuccess = true;
        Message = null;
    }

    public static ResultList<T> Success(List<T> value) => new ResultList<T> { Value = value, IsSuccess = true, Message = null };
    public static ResultList<T> Success() => new ResultList<T> { Value = new List<T>(), IsSuccess = true, Message = null };
    public static ResultList<T> Failure(string errorMessage) => new ResultList<T>
    { IsSuccess = false, Message = errorMessage, Value = new List<T>() };
}

public class Result
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }

    public Result()
    {
        IsSuccess = true;
        Message = null;
    }

    public static Result Success() => new Result { IsSuccess = true, Message = null };
    public static Result Failure(string errorMessage) => new Result { IsSuccess = false, Message = errorMessage };
}
