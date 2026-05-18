using Domain.Models.Systems;
using MyExpenses.Application.Dtos.Systems;

namespace MyExpenses.Application.Interfaces.Mappings;

public interface ILocationDtoDomainMapper
{
    /// <summary>
    /// Maps a <see cref="PlaceDomain"/> instance to a <see cref="PlaceDto"/> instance.
    /// </summary>
    /// <param name="src">The source <see cref="PlaceDomain"/> object to be mapped.</param>
    /// <returns>A new <see cref="PlaceDto"/> instance mapped from the provided <see cref="PlaceDomain"/>.</returns>
    public PlaceDto MapToDto(PlaceDomain src);

    /// <summary>
    /// Maps a <see cref="PlaceDto"/> instance to a <see cref="PlaceDomain"/> instance.
    /// </summary>
    /// <param name="src">The source <see cref="PlaceDto"/> object to be mapped.</param>
    /// <returns>A new <see cref="PlaceDomain"/> instance mapped from the provided <see cref="PlaceDto"/>.</returns>
    public PlaceDomain MapToDomain(PlaceDto src);
}