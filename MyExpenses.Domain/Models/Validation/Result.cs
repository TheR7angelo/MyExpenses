namespace Domain.Models.Validation;

public class Result
{
    public bool IsSuccess { get; set; }
    public string? InternalMessage { get; }
    public ErrorCode ErrorCode { get; }

    private Result(bool success, ErrorCode errorCode, string? internalMessage)
    {
        IsSuccess = success;
        ErrorCode = errorCode;
        InternalMessage = internalMessage;
    }

    public static Result Success(string? internalMessage = null)
        => new(true, ErrorCode.None, internalMessage);

    public static Result Failure(ErrorCode errorCode, string internalMessage) =>
        new(false, errorCode, internalMessage);
}