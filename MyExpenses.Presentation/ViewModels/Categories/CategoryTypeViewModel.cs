using CommunityToolkit.Mvvm.ComponentModel;
using Domain.Models.Categories;
using Domain.Models.Validation;
using MyExpenses.Presentation.Validations.Attributes;
using MyExpenses.Presentation.ViewModels.Systems;
using TheR7angelo.DirtyTracking.Abstractions;

namespace MyExpenses.Presentation.ViewModels.Categories;

[DirtyTracking]
public partial class CategoryTypeViewModel : ObservableValidator
{
    [ObservableProperty]
    public partial int Id { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RequiredWithCode(ErrorCode.NameRequired, ErrorMessage = "Category name is required")]
    [MaxLengthWithCode(CategoryTypeDomain.MaxNameLength, ErrorCode.NameTooLong, ErrorMessage = "Category name cannot exceed 55 characters")]
    public partial string? Name { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RequiredWithCode(ErrorCode.ColorRequired, ErrorMessage = "Category color is required")]
    public partial ColorViewModel? Color { get; set; }

    [ObservableProperty]
    public partial DateTime? DateAdded { get; set; }

    public IEnumerable<DomainValidationResult> GetErrorCodes()
        => GetErrors().OfType<DomainValidationResult>();
}