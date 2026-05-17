using CommunityToolkit.Mvvm.ComponentModel;
using FluentValidation.Results;
using MyExpenses.Presentation.Validations;
using TheR7angelo.DirtyTracking.Abstractions;

namespace MyExpenses.Presentation.ViewModels.Systems;

[DirtyTracking]
public partial class ColorViewModel : BaseViewModel
{
    public int Id { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial string? Name { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial string? HexadecimalColorCode { get; set; }

    public DateTime? DateAdded { get; set; }

    [ObservableProperty]
    public partial bool IsDeleting { get; set; }

    public new void ValidateWithFluent(ValidationResult result)
        => base.ValidateWithFluent(result);
}