using Domain.Models.Systems;
using Domain.Models.Validation;
using FluentValidation;
using MyExpenses.Presentation.Resources.Resx.SystemResources;
using MyExpenses.Presentation.Validations.Interfaces;
using MyExpenses.Presentation.ViewModels.Systems;

namespace MyExpenses.Presentation.Validations;

public class ColorViewModelValidator() : AbstractValidator<ColorViewModel>
{
    public ColorViewModelValidator(ISystemPresentationValidationService systemPresentationValidationService) : this()
    {
        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(SystemResources.ColorViewModelValidatorNameRequired)
            .WithError(ErrorCode.NameRequired, SystemResources.ResourceManager, nameof(SystemResources.ColorViewModelValidatorNameRequired))

            .Length(1, ColorDomain.MaxNameLength).WithMessage(string.Format(SystemResources.ColorViewModelValidatorNameTooLong, ColorDomain.MaxNameLength))
            .WithError(ErrorCode.NameTooLong, SystemResources.ResourceManager, nameof(SystemResources.ColorViewModelValidatorNameTooLong), ColorDomain.MaxNameLength)

            .MustAsync(async (name, cancellation) => !await systemPresentationValidationService.IsColorNameAvailableAsync(name!, cancellation))
            .WithMessage(SystemResources.ColorViewModelValidatorNameAlreadyUsed)
            .WithError(ErrorCode.NameAlreadyExists, SystemResources.ResourceManager, nameof(SystemResources.ColorViewModelValidatorNameAlreadyUsed))

            .When(x => x.IsNameDirty || x.Id is 0);

        RuleFor(x => x.HexadecimalColorCode)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(SystemResources.ColorViewModelValidatorHexadecimalCodeRequired)
            .WithError(ErrorCode.HexadecimalColorCodeRequired, SystemResources.ResourceManager, nameof(SystemResources.ColorViewModelValidatorHexadecimalCodeRequired))

            .Matches("#([A-Fa-f0-9]{8}|[A-Fa-f0-9]{4})").WithMessage(SystemResources.ColorViewModelValidatorHexadecimalCodeInvalidFormat)
            .WithError(ErrorCode.HexadecimalColorCodeInvalidFormat, SystemResources.ResourceManager, nameof(SystemResources.ColorViewModelValidatorHexadecimalCodeInvalidFormat))

            .MustAsync(async (hexadecimalCode, cancellation) => !await systemPresentationValidationService.IsColorHexadecimalCodeAvailableAsync(hexadecimalCode!, cancellation))
            .WithMessage(x => string.Format(SystemResources.ColorViewModelValidatorHexadecimalCodeAlreadyUsed, x.HexadecimalColorCode))
            .WithError(ErrorCode.HexadecimalColorCodeAlreadyExists, SystemResources.ResourceManager, nameof(SystemResources.ColorViewModelValidatorHexadecimalCodeAlreadyUsed), x => x.HexadecimalColorCode)

            .When(x => x.IsHexadecimalColorCodeDirty || x.Id is 0);
    }
}