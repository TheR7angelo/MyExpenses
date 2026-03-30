using System.ComponentModel.DataAnnotations;
using Domain.Models.Validation;

namespace MyExpenses.Presentation.Validations.Attributes;

public class MaxLengthWithCodeAttribute(int length, ErrorCode errorCode) : MaxLengthAttribute(length)
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        => ValidationHelper.Wrap(this, base.IsValid(value, validationContext), validationContext, errorCode);
}