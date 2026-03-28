using System.ComponentModel.DataAnnotations;
using Domain.Models.Validation;

namespace MyExpenses.Presentation.Validations.Attributes;

public class RequiredWithCodeAttribute(ErrorCode errorCode) : RequiredAttribute
{
    public ErrorCode ErrorCode { get; } = errorCode;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var result = base.IsValid(value, validationContext);
        return result != ValidationResult.Success
            ? new DomainValidationResult(ErrorCode, ErrorMessage, [validationContext.MemberName!])
            : result;
    }
}