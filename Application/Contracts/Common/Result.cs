namespace Application.Contracts.Common;

public enum ErrorCodes
{
    Validation = 1,
    Unauthorized = 2,
    Unknown = 999
}

public abstract class Result
{
    public bool IsSuccess { get; }
    public ErrorCodes Code { get; }
    public string Message { get; }

    protected Result(bool isSuccess, ErrorCodes code, string message)
    {
        IsSuccess = isSuccess;
        Code = code;
        Message = message;
    }
}

public sealed class Success<T> : Result
{
    public T Value { get; }

    public Success(T value)
        : base(true, ErrorCodes.Unknown, string.Empty)
    {
        Value = value;
    }
}

public sealed class Failure : Result
{
    public Failure(ErrorCodes code, string message)
        : base(false, code, message)
    {
    }
}