using MyExpenses.Application.Dtos.Systems;
using MyExpenses.Presentation.ViewModels.Systems;

namespace MyExpenses.Presentation.Mappings.Interfaces;

public interface ISystemDtoViewModelMapper
{
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

    /// <summary>
    /// Creates a deep clone of the provided <see cref="ColorViewModel"/> instance.
    /// </summary>
    /// <param name="src">The instance of <see cref="ColorViewModel"/> to be cloned.</param>
    /// <returns>A new <see cref="ColorViewModel"/> instance with the same property values as the source object.</returns>
    public ColorViewModel Clone(ColorViewModel src);

    /// <summary>
    /// Maps a PlaceViewModel instance to a PlaceDto instance.
    /// </summary>
    /// <param name="src">The PlaceViewModel to map from.</param>
    /// <returns>A PlaceDto containing the mapped data.</returns>
    public PlaceDto MapToDto(PlaceViewModel src);

    /// <summary>
    /// Maps a place DTO to a place view model.
    /// </summary>
    /// <param name="src">The place DTO to map from.</param>
    /// <returns>A place view model containing the mapped data.</returns>
    public PlaceViewModel MapToViewModel(PlaceDto src);

    /// <summary>
    /// Maps a RecursiveFrequencyViewModel instance to a RecursiveFrequencyDto instance.
    /// </summary>
    /// <param name="src">The RecursiveFrequencyViewModel instance to map from.</param>
    /// <returns>A RecursiveFrequencyDto containing the mapped data.</returns>
    public RecursiveFrequencyDto MapToDto(RecursiveFrequencyViewModel src);

    /// <summary>
    /// Maps a recursive frequency DTO to a recursive frequency view model.
    /// </summary>
    /// <param name="src">The recursive frequency DTO to map from.</param>
    /// <returns>A recursive frequency view model containing the mapped data.</returns>
    public RecursiveFrequencyViewModel MapToViewModel(RecursiveFrequencyDto src);
}