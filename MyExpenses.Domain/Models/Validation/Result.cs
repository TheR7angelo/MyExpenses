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

    public static Result Failure(ErrorCode errorCode, string internalMessage)
        => new(false, errorCode, internalMessage);
}

public class Result<T> : Result
{
    public T? Value { get; private set; }

    protected Result(bool success, ErrorCode errorCode, string? internalMessage, T? value = default)
        : base(success, errorCode, internalMessage)
    {
        Value = value;
    }

    public static Result<T> Success(T? value = default, string? internalMessage = null)
        => new(true, ErrorCode.None, internalMessage, value);

    public new static Result<T> Failure(ErrorCode errorCode, string internalMessage)
        => new(false, errorCode, internalMessage);
}

public class DeletionResult : Result<Dictionary<DependencyType, int[]>>
{
    private DeletionResult(bool success, ErrorCode errorCode, string? internalMessage, Dictionary<DependencyType, int[]>? deletedItems = null)
        : base(success, errorCode, internalMessage, deletedItems)
    {
    }

    public static DeletionResult Success(string? internalMessage = null, Dictionary<DependencyType, int[]>? deletedItems = null) =>
        new(true, ErrorCode.None, internalMessage, deletedItems);

    public new static DeletionResult Failure(ErrorCode errorCode, string internalMessage) =>
        new(false, errorCode, internalMessage);

    public Dictionary<DependencyType, int[]>? DeletedItems => Value;
}

public static class ResultMappingExtensions
{
    public static Result<TDestination> Map<TSource, TDestination>(
        this Result<TSource> src,
        Func<TSource, TDestination> map)
    {
        if (!src.IsSuccess)
        {
            return Result<TDestination>.Failure(
                src.ErrorCode,
                src.InternalMessage ?? string.Empty);
        }

        var value = src.Value is null ? default : map(src.Value);

        return Result<TDestination>.Success(
            value,
            src.InternalMessage ?? string.Empty);
    }
}