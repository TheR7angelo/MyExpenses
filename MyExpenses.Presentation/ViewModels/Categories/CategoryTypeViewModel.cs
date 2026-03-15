using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using MyExpenses.Presentation.ViewModels.Systems;

namespace MyExpenses.Presentation.ViewModels.Categories;

public partial class CategoryTypeViewModel : ObservableValidator
{
    [ObservableProperty]
    public partial int Id { get; set; }

    [ObservableProperty]
    [Required(ErrorMessage = "Category name is required")]
    [MaxLength(55, ErrorMessage = "The maximum length of the category name is 55 characters")]
    public partial string? Name { get; set; }

    [ObservableProperty]
    public partial ColorViewModel? Color { get; set; }

    [ObservableProperty]
    public partial DateTime? DateAdded { get; set; }
}