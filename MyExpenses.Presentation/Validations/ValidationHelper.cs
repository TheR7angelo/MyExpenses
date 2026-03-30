using System.ComponentModel.DataAnnotations;
using Domain.Models.Validation;
using MyExpenses.Presentation.Validations.Attributes;

namespace MyExpenses.Presentation.Validations;

public static class ValidationHelper
{
    /// <summary>
    /// Wraps a validation result, enriching it with additional details such as error codes and internal messages.
    /// </summary>
    /// <param name="attribute">The validation attribute that triggered the validation error.</param>
    /// <param name="result">The original validation result from the validation process.</param>
    /// <param name="context">The validation context containing metadata about the validated object.</param>
    /// <param name="errorCode">The error code that corresponds to the validation failure.</param>
    /// <returns>
    /// A new <see cref="DomainValidationResult"/> object containing the supplied error code, formatted error message,
    /// internal message, and the member names involved in the validation error, or <c>ValidationResult.Success</c>
    /// if the original result is successful.
    /// </returns>
    public static ValidationResult? Wrap(
        ValidationAttribute attribute,
        ValidationResult? result,
        ValidationContext context,
        ErrorCode errorCode)
    {
        if (result == ValidationResult.Success || result == null) return ValidationResult.Success;

        var internalMessage = ValidationCacheHelper.GetInternalMessage(attribute);

        return new DomainValidationResult(
            errorCode,
            attribute.FormatErrorMessage(context.DisplayName),
            internalMessage,
            [context.MemberName!]
        );
    }
}