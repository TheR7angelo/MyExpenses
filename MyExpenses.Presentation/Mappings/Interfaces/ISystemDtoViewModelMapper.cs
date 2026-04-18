using Domain.Models.Systems;
using MyExpenses.Application.Dtos.Systems;
using MyExpenses.Presentation.ViewModels.Systems;

namespace MyExpenses.Presentation.Mappings.Interfaces;

public interface ISystemDtoViewModelMapper
{
    /// <summary>
    /// Maps a color domain model to a color DTO.
    /// </summary>
    /// <param name="src">The color domain model to map from.</param>
    /// <returns>A color DTO containing the mapped data.</returns>
    public ColorDto MapToDto(ColorDomain src);

    /// <summary>
    /// Maps a color view model to a color DTO.
    /// </summary>
    /// <param name="src">The color view model to map from.</param>
    /// <returns>A color DTO containing the mapped data.</returns>
    public ColorDto MapToDto(ColorViewModel src);

    /// <summary>
    /// Maps a color DTO to a color view model.
    /// </summary>
    /// <param name="src">The color DTO to map from.</param>
    /// <returns>A color view model containing the mapped data.</returns>
    public ColorViewModel MapToViewModel(ColorDto src);
}