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

    /// <summary>
    /// Merges the properties of the source color view model into the destination color view model.
    /// </summary>
    /// <param name="src">The source <see cref="ColorViewModel"/> to merge from.</param>
    /// <param name="dst">The destination <see cref="ColorViewModel"/> to merge into.</param>
    public void Merge(ColorViewModel src, ColorViewModel dst);
}