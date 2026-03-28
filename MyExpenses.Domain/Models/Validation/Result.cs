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

    public static Result Success()
        => new(true, ErrorCode.None, null);

    public static Result Failure(ErrorCode errorCode, string internalMessage) =>
        new(false, errorCode, internalMessage);
}