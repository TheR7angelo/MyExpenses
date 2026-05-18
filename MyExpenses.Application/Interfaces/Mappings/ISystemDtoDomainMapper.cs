using Domain.Models.Systems;
using MyExpenses.Application.Dtos.Systems;

namespace MyExpenses.Application.Interfaces.Mappings;

public interface ISystemDtoDomainMapper
{
    /// <summary>
    /// Maps a <see cref="ColorDto"/> instance to a <see cref="ColorDomain"/> instance.
    /// </summary>
    /// <param name="src">The source <see cref="ColorDto"/> object to be mapped.</param>
    /// <returns>A new <see cref="ColorDomain"/> instance mapped from the provided <see cref="ColorDto"/>.</returns>
    public ColorDomain MapToDomain(ColorDto src);

    /// <summary>
    /// Maps a <see cref="ColorDomain"/> instance to a <see cref="ColorDto"/> instance.
    /// </summary>
    /// <param name="src">The source <see cref="ColorDomain"/> object to be mapped.</param>
    /// <returns>A new <see cref="ColorDto"/> instance mapped from the provided <see cref="ColorDomain"/>.</returns>
    public ColorDto MapToDto(ColorDomain src);

    /// <summary>
    /// Maps a <see cref="RecursiveFrequencyDomain"/> instance to a <see cref="RecursiveFrequencyDto"/> instance.
    /// </summary>
    /// <param name="src">The source <see cref="RecursiveFrequencyDomain"/> object to be mapped.</param>
    /// <returns>A new <see cref="RecursiveFrequencyDto"/> instance mapped from the provided <see cref="RecursiveFrequencyDomain"/>.</returns>
    public RecursiveFrequencyDto MapToDto(RecursiveFrequencyDomain src);

    /// <summary>
    /// Maps a <see cref="RecursiveFrequencyDto"/> instance to a <see cref="RecursiveFrequencyDomain"/> instance.
    /// </summary>
    /// <param name="src">The source <see cref="RecursiveFrequencyDto"/> object to be mapped.</param>
    /// <returns>A new <see cref="RecursiveFrequencyDomain"/> instance mapped from the provided <see cref="RecursiveFrequencyDto"/>.</returns>
    public RecursiveFrequencyDomain MapToDomain(RecursiveFrequencyDto src);
}