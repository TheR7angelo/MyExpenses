using System.ComponentModel.DataAnnotations;
using Domain.Models.Validation;

namespace MyExpenses.Presentation.Validations.Attributes;

public class DomainValidationResult(ErrorCode errorCode, string? errorMessage, IEnumerable<string>? memberNames)
    : ValidationResult(errorMessage, memberNames)
{
    public ErrorCode ErrorCode { get; } = errorCode;
}