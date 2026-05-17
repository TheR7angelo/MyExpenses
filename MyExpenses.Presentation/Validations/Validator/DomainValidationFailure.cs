using System.Globalization;
using System.Resources;
using Domain.Models.Validation;
using FluentValidation;

namespace MyExpenses.Presentation.Validations.Validator;

/// <summary>
/// Provides extension methods for validation rules within the application.
/// </summary>
/// <remarks>
/// This class is designed to extend the functionality of validation rules. It serves as a centralized place for implementing
/// reusable and composable validation logic across the application.
/// </remarks>
public static class RuleExtensions
{
    /// <summary>
    /// Provides extension methods for configuring validation rules with custom error handling within the application.
    /// </summary>
    /// <remarks>
    /// This class allows customization of validation failures by associating them with domain-specific error codes and localizable error messages.
    /// </remarks>
    extension<T, TProperty>(IRuleBuilderOptions<T, TProperty> rule)
    {
        /// <summary>
        /// Configures a validation rule with custom error handling using a specific error code,
        /// a resource manager for localization, and resource property information for message formatting.
        /// </summary>
        /// <param name="code">
        /// The <see cref="ErrorCode"/> representing the specific error type associated with the validation failure.
        /// </param>
        /// <param name="resourceManager">
        /// The <see cref="ResourceManager"/> used to retrieve localized error messages for the validation failure.
        /// </param>
        /// <param name="resourcePropertyName">
        /// The name of the property within the resource file used to fetch the localized error message.
        /// </param>
        /// <param name="args">
        /// An array of arguments used to format the localized error message.
        /// </param>
        /// <returns>
        /// An <see cref="IRuleBuilderOptions{T, TProperty}"/> that applies the custom error handling configuration to the validation rule.
        /// </returns>
        public IRuleBuilderOptions<T, TProperty> WithError(ErrorCode code,
            ResourceManager resourceManager,
            string resourcePropertyName,
            params object?[] args)
        {
            return rule
                .WithErrorCode(((int)code).ToString())
                .WithState(_ => new DomainValidationFailure(code, resourceManager, resourcePropertyName, args));
        }

        /// <summary>
        /// Configures a validation rule with custom error handling by associating it
        /// with a specific <see cref="ErrorCode"/>, localizable error messages, and dynamically
        /// extracted arguments for message formatting.
        /// </summary>
        /// <param name="code">
        /// The <see cref="ErrorCode"/> indicating the domain-specific error type to be used for the validation failure.
        /// </param>
        /// <param name="resourceManager">
        /// The <see cref="ResourceManager"/> responsible for retrieving localized error messages.
        /// </param>
        /// <param name="resourcePropertyName">
        /// The name of the resource file property that contains the format string for the error message.
        /// </param>
        /// <param name="argExtractors">
        /// An array of delegate functions that extract values from the validated instance
        /// to be used as arguments in formatting the localized error message.
        /// </param>
        /// <returns>
        /// An <see cref="IRuleBuilderOptions{T, TProperty}"/> instance with the applied custom error handling configuration.
        /// </returns>
        public IRuleBuilderOptions<T, TProperty> WithError(ErrorCode code,
            ResourceManager resourceManager,
            string resourcePropertyName,
            params Func<T, object?>[] argExtractors)
        {
            return rule
                .WithErrorCode(((int)code).ToString())
                .WithState(instance =>
                {
                    var args = argExtractors.Select(func => func(instance)).ToArray();
                    return new DomainValidationFailure(code, resourceManager, resourcePropertyName, args);
                });
        }
    }
}

/// <summary>
/// Represents a failure that occurs during domain validation.
/// </summary>
/// <remarks>
/// This class is used to encapsulate information about a validation failure specific to the domain layer.
/// It provides a standardized way of describing and managing validation issues that arise due to business rules.
/// </remarks>
public class DomainValidationFailure(
    ErrorCode errorCode,
    ResourceManager resourceManager,
    string resourcePropertyName,
    params object?[] args)
    : FluentValidation.Results.ValidationFailure
{
    public string ErrorCodeString => $"{(int)DomainCode}({DomainCode})";
    public string InternalMessage
        => string.Format(CultureInfo.CurrentCulture, ResourceManager.GetString(ResourcePropertyName, CultureInfo.CurrentCulture) ?? string.Empty, args);

    private ErrorCode DomainCode { get; set; } = errorCode;
    private ResourceManager ResourceManager { get; set; } = resourceManager;
    private string ResourcePropertyName { get; set; } = resourcePropertyName;
}