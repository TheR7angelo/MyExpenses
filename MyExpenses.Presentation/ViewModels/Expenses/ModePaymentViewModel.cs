using CommunityToolkit.Mvvm.ComponentModel;
using MyExpenses.Presentation.Validations.Validator;
using TheR7angelo.DirtyTracking.Abstractions;

namespace MyExpenses.Presentation.ViewModels.Expenses;

[DirtyTracking]
public partial class ModePaymentViewModel : BaseViewModel
{
    public int Id { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial string? Name { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial bool CanBeDeleted { get; set; } = true;

    public DateTime? DateAdded { get; set; } = DateTime.Now;

    [ObservableProperty]
    public partial bool IsDeleting { get; set; }
}