using Domain.Models.Systems;
using Domain.Models.Validation;
using FluentValidation;
using MyExpenses.Presentation.Resources.Resx.LocationResources;
using MyExpenses.Presentation.ViewModels.Locations;

namespace MyExpenses.Presentation.Validations.Validator;

public class PlaceViewModelValidator : AbstractValidator<PlaceViewModel>
{
    public PlaceViewModelValidator()
    {
        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage(LocationResources.PlaceViewModelValidatorNameRequired)
            .WithError(ErrorCode.NameRequired, LocationResources.ResourceManager, nameof(LocationResources.PlaceViewModelValidatorNameRequired))
            .Length(1, PlaceDomain.MaxNameLength).WithMessage(string.Format(LocationResources.PlaceViewModelValidatorNameLength, PlaceDomain.MaxNameLength))
            .WithError(ErrorCode.NameTooLong, LocationResources.ResourceManager, nameof(LocationResources.PlaceViewModelValidatorNameLength))

            .When(x => x.IsNameDirty || x.Id is 0);

        RuleFor(x => x.Number)
            .Cascade(CascadeMode.Stop)
            .Length(0, PlaceDomain.MaxNumberLength).WithMessage(string.Format(LocationResources.PlaceViewModelValidatorNumberLength, PlaceDomain.MaxNumberLength))
            .WithError(ErrorCode.NameTooLong, LocationResources.ResourceManager, nameof(LocationResources.PlaceViewModelValidatorNumberLength))

            .When(x => !string.IsNullOrWhiteSpace(x.Number) && (x.IsNumberDirty || x.Id is 0));

        RuleFor(x => x.Street)
            .Cascade(CascadeMode.Stop)
            .Length(1, PlaceDomain.MaxStreetLength).WithMessage(string.Format(LocationResources.PlaceViewModelValidatorStreetLength, PlaceDomain.MaxStreetLength))
            .WithError(ErrorCode.NameTooLong, LocationResources.ResourceManager, nameof(LocationResources.PlaceViewModelValidatorStreetLength))

            .When(x => x.IsStreetDirty || x.Id is 0);

        RuleFor(x => x.Postal)
            .Cascade(CascadeMode.Stop)
            .Length(1, PlaceDomain.MaxPostalLength).WithMessage(string.Format(LocationResources.PlaceViewModelValidatorPostalLength, PlaceDomain.MaxPostalLength))
            .WithError(ErrorCode.NameTooLong, LocationResources.ResourceManager, nameof(LocationResources.PlaceViewModelValidatorPostalLength))

            .When(x => x.IsPostalDirty || x.Id is 0);

        RuleFor(x => x.City)
            .Cascade(CascadeMode.Stop)
            .Length(1, PlaceDomain.MaxCityLength).WithMessage(string.Format(LocationResources.PlaceViewModelValidatorCityLength, PlaceDomain.MaxCityLength))
            .WithError(ErrorCode.NameTooLong, LocationResources.ResourceManager, nameof(LocationResources.PlaceViewModelValidatorCityLength))

            .When(x => x.IsCityDirty || x.Id is 0);

        RuleFor(x => x.Country)
            .Cascade(CascadeMode.Stop)
            .Length(1, PlaceDomain.MaxCityLength).WithMessage(string.Format(LocationResources.PlaceViewModelValidatorCountryLength, PlaceDomain.MaxCityLength))
            .WithError(ErrorCode.NameTooLong, LocationResources.ResourceManager, nameof(LocationResources.PlaceViewModelValidatorCountryLength))

            .When(x => x.IsCountryDirty || x.Id is 0);

        RuleFor(x => x.Latitude)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage(LocationResources.PlaceViewModelValidatorLatitudeNotNull)
            .WithError(ErrorCode.LatitudeRequired, LocationResources.ResourceManager, nameof(LocationResources.PlaceViewModelValidatorLatitudeNotNull))

            .When(x => x.IsLatitudeDirty || x.Id is 0);

        RuleFor(x => x.Longitude)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage(LocationResources.PlaceViewModelValidatorLongitudeNotNull)
            .WithError(ErrorCode.LongitudeRequired, LocationResources.ResourceManager, nameof(LocationResources.PlaceViewModelValidatorLongitudeNotNull))

            .When(x => x.IsLongitudeDirty || x.Id is 0);
    }
}