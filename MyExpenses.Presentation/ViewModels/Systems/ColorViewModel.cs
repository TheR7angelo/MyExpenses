using CommunityToolkit.Mvvm.ComponentModel;
using Domain.Models.Systems;
using Domain.Models.Validation;
using MyExpenses.Presentation.Validations.Attributes;
using TheR7angelo.DirtyTracking.Abstractions;

namespace MyExpenses.Presentation.ViewModels.Systems;

[DirtyTracking]
public partial class ColorViewModel : ObservableValidator
{
    public int Id { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RequiredWithCode(ErrorCode.NameRequired, ErrorMessage = "Color name is required")]
    [MaxLengthWithCode(ColorDomain.MaxNameLength, ErrorCode.NameTooLong, ErrorMessage = "Color name cannot exceed 55 characters")]
    public partial string? Name { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RequiredWithCode(ErrorCode.HexadecimalColorCodeRequired, ErrorMessage = "Hexadecimal color code is required")]
    [MaxLengthWithCode(ColorDomain.MaxHexadecimalColorCodeLength, ErrorCode.HexadecimalColorCodeTooLong, ErrorMessage = "Hexadecimal color code cannot exceed 9 characters")]
    public partial string? HexadecimalColorCode { get; set; }

    public DateTime? DateAdded { get; init; }
}