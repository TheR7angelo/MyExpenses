using CommunityToolkit.Mvvm.ComponentModel;
using MyExpenses.Presentation.Validations;
using TheR7angelo.DirtyTracking.Abstractions;

namespace MyExpenses.Presentation.ViewModels.Accounts;

[DirtyTracking]
public partial class AccountTypeViewModel : BaseViewModel
{
    [ObservableProperty]
    public partial int Id { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial string? Name { get; set; }

    public DateTime? DateAdded { get; set; }

    [ObservableProperty]
    public partial bool IsDeleting { get; set; }
}