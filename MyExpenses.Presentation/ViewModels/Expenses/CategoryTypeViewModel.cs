using CommunityToolkit.Mvvm.ComponentModel;
using MyExpenses.Presentation.Validations.Validator;
using MyExpenses.Presentation.ViewModels.Systems;
using TheR7angelo.DirtyTracking.Abstractions;

namespace MyExpenses.Presentation.ViewModels.Expenses;

[DirtyTracking]
public partial class CategoryTypeViewModel : BaseViewModel
{
    public int Id { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial string? Name { get; set; }

    [DirtyTrackedProperty(DisplayMember = nameof(Color.Name))]
    [ObservableProperty]
    public partial ColorViewModel? Color { get; set; }

    public DateTime DateAdded { get; set; } = DateTime.Now;

    [ObservableProperty]
    public partial bool IsDeleting { get; set; }
}