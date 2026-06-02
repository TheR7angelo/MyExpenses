using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentValidation.Results;
using MyExpenses.Presentation.Validations.Validator;
using TheR7angelo.DirtyTracking.Abstractions;

namespace MyExpenses.Presentation.ViewModels.Locations;

[DirtyTracking]
public partial class PlaceViewModel : BaseViewModel
{
    public int Id { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial string? Name { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial string? Number { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial string? Street { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial string? Postal { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial string? City { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial string? Country { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial double? Latitude { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
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

    public new void ValidateWithFluent(ValidationResult result)
        => base.ValidateWithFluent(result);
}