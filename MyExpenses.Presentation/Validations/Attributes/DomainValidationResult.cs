using System.ComponentModel.DataAnnotations;
using Domain.Models.Validation;

namespace MyExpenses.Presentation.Validations.Attributes;

public class DomainValidationResult(ErrorCode errorCode, string? errorMessage, string? internalMessage, IEnumerable<string>? memberNames)
    : ValidationResult(errorMessage, memberNames)
{
    public ErrorCode ErrorCode { get; } = errorCode;

    public string InternalMessage { get; } = internalMessage ?? string.Empty;
}