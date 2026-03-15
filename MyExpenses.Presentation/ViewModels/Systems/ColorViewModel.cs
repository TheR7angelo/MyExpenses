using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using Domain.Models.Systems;

namespace MyExpenses.Presentation.ViewModels.Systems;

public class ColorViewModel : ObservableValidator
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Color name is required")]
    [MaxLength(ColorDomain.MaxNameLength, ErrorMessage = "Account name cannot exceed 55 characters")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Hexadecimal color code is required")]
    [MaxLength(ColorDomain.MaxHexadecimalColorCodeLength, ErrorMessage = "Hexadecimal color code cannot exceed 9 characters")]
    public string? HexadecimalColorCode { get; set; }

    public DateTime? DateAdded { get; init; }
}