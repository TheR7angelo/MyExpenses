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

    /// <summary>
    /// Maps a system settings view model to a system settings DTO.
    /// </summary>
    /// <param name="appSettingsDto">The system settings view model to map from.</param>
    /// <returns>A system settings DTO containing the mapped data.</returns>
    public AppSettingsViewModel MapToViewModel(AppSettingsDto appSettingsDto);

    /// <summary>
    /// Maps an application settings view model to a DTO.
    /// </summary>
    /// <param name="appSettingsViewModel">The application settings view model to map from.</param>
    /// <returns>An application settings DTO containing the mapped data.</returns>
    public AppSettingsDto MapToDto(AppSettingsViewModel appSettingsViewModel);

    /// <summary>
    /// Maps a system DTO to a system view model.
    /// </summary>
    /// <param name="systemDto">The system DTO to map from.</param>
    /// <returns>A system view model containing the mapped data.</returns>
    public SystemViewModel MapToViewModel(SystemDto systemDto);

    /// <summary>
    /// Maps a System view model to a System DTO.
    /// </summary>
    /// <param name="systemViewModel">The System view model to map from.</param>
    /// <returns>A System DTO containing the mapped data.</returns>
    public SystemDto MapToDto(SystemViewModel systemViewModel);

    /// <summary>
    /// Maps a system DTO to a system view model.
    /// </summary>
    /// <param name="interfaceDto">The system DTO to map from.</param>
    /// <returns>A system view model containing the mapped data.</returns>
    public InterfaceViewModel MapToViewModel(InterfaceDto interfaceDto);

    /// <summary>
    /// Maps a color view model to a color DTO.
    /// </summary>
    /// <param name="interfaceViewModel">The color view model to map from.</param>
    /// <returns>A color DTO containing the mapped data.</returns>
    public InterfaceDto MapToDto(InterfaceViewModel interfaceViewModel);

    /// <summary>
    /// Maps a theme DTO to a theme view model.
    /// </summary>
    /// <param name="themeDto">The theme DTO to map from.</param>
    /// <returns>A theme view model containing the mapped data.</returns>
    public ThemeViewModel MapToViewModel(ThemeDto themeDto);

    /// <summary>
    /// Maps a theme view model to a theme DTO.
    /// </summary>
    /// <param name="themeViewModel">The theme view model to map from.</param>
    /// <returns>A theme DTO containing the mapped data.</returns>
    public ThemeDto MapToDto(ThemeViewModel themeViewModel);

    /// <summary>
    /// Maps a clock DTO to a clock view model.
    /// </summary>
    /// <param name="clockDto">The clock DTO to map from.</param>
    /// <returns>A clock view model containing the mapped data.</returns>
    public ClockViewModel MapToViewModel(ClockDto clockDto);

    /// <summary>
    /// Maps a clock view model to a clock DTO.
    /// </summary>
    /// <param name="clockViewModel">The clock view model to map from.</param>
    /// <returns>A clock DTO containing the mapped data.</returns>
    public ClockDto MapToDto(ClockViewModel clockViewModel);
}