using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using CommunityToolkit.Mvvm.ComponentModel;
using Domain.Models.Expenses;
using Domain.Models.Validation;
using MyExpenses.Presentation.Validations.Attributes;
using TheR7angelo.DirtyTracking.Abstractions;

namespace MyExpenses.Presentation.ViewModels.Systems;

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

    // partial void OnLatitudeChanged(double? value)
        // => UpdateGeometry();

    [DirtyTrackedProperty]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RequiredWithCode(ErrorCode.LongitudeRequired, ErrorMessage = "Longitude is required")]
    public partial double? Longitude { get; set; }

    // partial void OnLongitudeChanged(double? value)
        // => UpdateGeometry();

    // [NotMapped]
    // private Geometry? _geometry;
    //
    // [NotMapped]
    // public Geometry? Geometry
    // {
    //     get => _geometry;
    //     set
    //     {
    //         _geometry = value;
    //         if (_geometry is null)
    //         {
    //             Longitude = null;
    //             Latitude = null;
    //         }
    //         else
    //         {
    //             var point = (Point)_geometry;
    //             Longitude = point.X;
    //             Latitude = point.Y;
    //         }
    //     }
    // }

    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial bool IsOpen { get; set; } = true;

    public bool CanBeDeleted { get; set; } = true;

    public DateTime? DateAdded { get; set; } = DateTime.Now;

    public override string ToString()
    {
        // This implementation was chosen based on performance benchmarks.
        // It uses a List<string> to collect non-empty components of the address,
        // which is both fast and memory-efficient for the given use case.
        // Alternative approaches, such as StringBuilder or LINQ, introduced
        // either higher memory allocation or slower execution times.
        // This method achieves the best balance between simplicity, performance,
        // and maintainability.
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        var partAddress = new List<string>();
        if (!string.IsNullOrEmpty(Number)) partAddress.Add(Number);
        if (!string.IsNullOrEmpty(Street)) partAddress.Add(Street);
        if (!string.IsNullOrEmpty(Postal)) partAddress.Add(Postal);
        if (!string.IsNullOrEmpty(City)) partAddress.Add(City);
        if (!string.IsNullOrEmpty(Country)) partAddress.Add(Country);
        return string.Join(", ", partAddress);
    }

    // private void UpdateGeometry()
    // {
    //     if (Longitude.HasValue && Latitude.HasValue)
    //     {
    //         // This implementation was chosen for its clarity and efficiency.
    //         // It ensures that the geometry is updated only when both longitude and
    //         // latitude have valid values, minimizing unnecessary object creation.
    //         // Setting _geometry to null when values are invalid ensures consistent state
    //         // management without introducing additional complexity.
    //         // ReSharper disable once HeapView.ObjectAllocation.Evident
    //         _geometry = new Point(Longitude.Value, Latitude.Value) { SRID = 4326 };
    //     }
    //     else
    //     {
    //         _geometry = null;
    //     }
    // }

    public IEnumerable<DomainValidationResult> GetErrorCodes()
        => GetErrors().OfType<DomainValidationResult>();
}