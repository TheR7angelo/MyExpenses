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
        // TODO add check inside db
        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage(LocationResources.PlaceViewModelValidatorNameRequired)
            .WithError(ErrorCode.NameRequired, LocationResources.ResourceManager, nameof(LocationResources.PlaceViewModelValidatorNameRequired))
            .Length(1, PlaceDomain.MaxNameLength).WithMessage(LocationResources.PlaceViewModelValidatorNameLength)
            .WithError(ErrorCode.NameTooLong, LocationResources.ResourceManager, nameof(LocationResources.PlaceViewModelValidatorNameLength))

            .When(x => x.IsNameDirty || x.Id is 0);

        RuleFor(x => x.Number)
            .Cascade(CascadeMode.Stop)
            .Length(0, PlaceDomain.MaxNumberLength).WithMessage(LocationResources.PlaceViewModelValidatorNumberLength)
            .WithError(ErrorCode.NameTooLong, LocationResources.ResourceManager, nameof(LocationResources.PlaceViewModelValidatorNumberLength))

            .When(x => x.IsNumberDirty || x.Id is 0);

        RuleFor(x => x.Street)
            .Cascade(CascadeMode.Stop)
            .Length(1, PlaceDomain.MaxStreetLength).WithMessage(LocationResources.PlaceViewModelValidatorStreetLength)
            .WithError(ErrorCode.NameTooLong, LocationResources.ResourceManager, nameof(LocationResources.PlaceViewModelValidatorStreetLength))

            .When(x => x.IsStreetDirty || x.Id is 0);

        RuleFor(x => x.Postal)
            .Cascade(CascadeMode.Stop)
            .Length(1, PlaceDomain.MaxPostalLength).WithMessage(LocationResources.PlaceViewModelValidatorPostalLength)
            .WithError(ErrorCode.NameTooLong, LocationResources.ResourceManager, nameof(LocationResources.PlaceViewModelValidatorPostalLength))

            .When(x => x.IsPostalDirty || x.Id is 0);

        RuleFor(x => x.City)
            .Cascade(CascadeMode.Stop)
            .Length(1, PlaceDomain.MaxCityLength).WithMessage(LocationResources.PlaceViewModelValidatorCityLength)
            .WithError(ErrorCode.NameTooLong, LocationResources.ResourceManager, nameof(LocationResources.PlaceViewModelValidatorCityLength))

            .When(x => x.IsCityDirty || x.Id is 0);

        RuleFor(x => x.Country)
            .Cascade(CascadeMode.Stop)
            .Length(1, PlaceDomain.MaxCityLength).WithMessage(LocationResources.PlaceViewModelValidatorCountryLength)
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