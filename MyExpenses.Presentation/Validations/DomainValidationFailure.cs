using System.Globalization;
using System.Resources;
using Domain.Models.Validation;
using FluentValidation;

namespace MyExpenses.Presentation.Validations;

public static class RuleExtensions
{
    public static IRuleBuilderOptions<T, TProperty> WithError<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> rule,
        ErrorCode code,
        ResourceManager resourceManager,
        string resourcePropertyName)
    {
        return rule
            .WithErrorCode(((int)code).ToString())
            .WithState(_ => new DomainValidationFailure(code, resourceManager, resourcePropertyName));
    }
}

public class DomainValidationFailure(ErrorCode errorCode, ResourceManager resourceManager, string resourcePropertyName) : FluentValidation.Results.ValidationFailure
{
    public string ErrorCodeString => $"{(int)DomainCode}({DomainCode})";
    public string? InternalMessage => ResourceManager.GetString(ResourcePropertyName, CultureInfo.InvariantCulture);

    private ErrorCode DomainCode { get; set; } = errorCode;
    private ResourceManager ResourceManager { get; set; } = resourceManager;
    private string ResourcePropertyName { get; set; } = resourcePropertyName;
}