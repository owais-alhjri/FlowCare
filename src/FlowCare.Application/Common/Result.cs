namespace FlowCare.Application.Common;

public class Result
{
    public bool IsSuccess { get; }
    public string? Error { get; }

    public bool IsFailure => !IsSuccess;


    protected Result(bool isSuccess, string? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, null);
    public static Result Fail(string error) => new(false, error);
}

public class Result<T> : Result
{
    public T? Value { get; }

    protected Result(bool isSuccess, T? value, string? error) : base(isSuccess, error)
    {
        Value = value;
    }

    public static Result<T> Success(T value) => new(true, value, null);
    public new static Result<T> Fail(string error) => new(false, default, error);
}