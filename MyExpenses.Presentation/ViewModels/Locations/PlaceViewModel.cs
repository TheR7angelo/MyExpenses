using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using Domain.Models.Systems;
using Domain.Models.Validation;
using MyExpenses.Presentation.Validations.Attributes;
using TheR7angelo.DirtyTracking.Abstractions;

namespace MyExpenses.Presentation.ViewModels.Locations;

[DirtyTracking]
public partial class PlaceViewModel : ObservableValidator
{
    public int Id { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RequiredWithCode(ErrorCode.NameRequired, ErrorMessage = "Name is required")]
    [MaxLengthWithCode(PlaceDomain.MaxNameLength, ErrorCode.NameTooLong, ErrorMessage = "Name cannot exceed 155 characters")]
    public partial string? Name { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [MaxLengthWithCode(PlaceDomain.MaxNumberLength, ErrorCode.NumberTooLong, ErrorMessage = "Number cannot exceed 20 characters")]
    public partial string? Number { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [MaxLengthWithCode(PlaceDomain.MaxStreetLength, ErrorCode.StreetTooLong, ErrorMessage = "Street cannot exceed 155 characters")]
    public partial string? Street { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [MaxLengthWithCode(PlaceDomain.MaxPostalLength, ErrorCode.PostalTooLong, ErrorMessage = "Postal cannot exceed 10 characters")]
    public partial string? Postal { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [MaxLengthWithCode(PlaceDomain.MaxCityLength, ErrorCode.CityTooLong, ErrorMessage = "City cannot exceed 100 characters")]
    public partial string? City { get; set; }

    [Column("country")]
    [DirtyTrackedProperty]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [MaxLengthWithCode(PlaceDomain.MaxCountryLength, ErrorCode.CountryTooLong, ErrorMessage = "Country cannot exceed 55 characters")]
    public partial string? Country { get; set; }

    [Column("latitude")]
    [DirtyTrackedProperty]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RequiredWithCode(ErrorCode.LatitudeRequired, ErrorMessage = "Latitude is required")]
    public partial double? Latitude { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RequiredWithCode(ErrorCode.LongitudeRequired, ErrorMessage = "Longitude is required")]
    public partial double? Longitude { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial bool IsOpen { get; set; } = true;

    public bool CanBeDeleted { get; set; } = true;

    public DateTime? DateAdded { get; set; } = DateTime.Now;

    public string GetAddress()
    {
        var sb = new StringBuilder(5);

        Append(Number);
        Append(Street);
        Append(Postal);
        Append(City);
        Append(Country);

        return sb.ToString();

        void Append(string? value)
        {
            if (string.IsNullOrEmpty(value)) return;
            if (sb.Length > 0) sb.Append(", ");
            sb.Append(value);
        }
    }

    public IEnumerable<DomainValidationResult> GetErrorCodes()
        => GetErrors().OfType<DomainValidationResult>();
}