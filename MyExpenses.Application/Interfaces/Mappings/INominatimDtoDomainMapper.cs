using Domain.Models.Nominatim;
using MyExpenses.Application.Dtos.Nominatium;

namespace MyExpenses.Application.Interfaces.Mappings;

public interface INominatimDtoDomainMapper
{
    /// <summary>
    /// Maps a domain object of type NominatimSearchResultDomain to a DTO of type NominatimSearchResultDto.
    /// </summary>
    /// <param name="domain">The domain object to map.</param>
    /// <returns>A DTO representing the mapped data.</returns>
    public NominatimSearchResultDto MapToDto(NominatimSearchResultDomain domain);

    /// <summary>
    /// Maps a domain object of type NominatimDetailedAddressDomain to a DTO of type NominatimDetailedAddressDto.
    /// </summary>
    /// <param name="domain">The domain object to map.</param>
    /// <returns>A DTO representing the mapped data.</returns>
    public NominatimDetailedAddressDto MapToDto(NominatimDetailedAddressDomain domain);

    /// <summary>
    /// Maps a domain object of type NominatimGeoJsonDomain to a DTO of type NominatiumGeoJsonDto.
    /// </summary>
    /// <param name="domain">The domain object to map.</param>
    /// <returns>A DTO representing the mapped data.</returns>
    public NominatiumGeoJsonDto MapToDto(NominatimGeoJsonDomain domain);
}