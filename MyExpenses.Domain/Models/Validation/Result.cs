using Domain.Models.Dependencies;

namespace Domain.Models.Validation;

public class Result
{
    public bool IsSuccess { get; private set; }
    public string? InternalMessage { get; private set; }
    public ErrorCode ErrorCode { get; private set; }

    protected Result(bool success, ErrorCode errorCode, string? internalMessage)
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

public class DeletionResult : Result
{
    public Dictionary<DependencyType, int[]>? DeletedItems { get; private set; }

    private DeletionResult(bool success, ErrorCode errorCode, string? internalMessage, Dictionary<DependencyType, int[]>? deletedItems = null)
        : base(success, errorCode, internalMessage)
    {
        DeletedItems = deletedItems;
    }

    public static DeletionResult Success(string? internalMessage = null, Dictionary<DependencyType, int[]>? deletedItems = null) =>
        new(true, ErrorCode.None, internalMessage, deletedItems);

    public new static DeletionResult Failure(ErrorCode errorCode, string internalMessage) =>
        new(false, errorCode, internalMessage);
}