using MyExpenses.Application.Dtos.Nominatium;
using MyExpenses.Presentation.ViewModels.Locations;

namespace MyExpenses.Presentation.Mappings.Interfaces;

/// <summary>
/// Interface for mapping between Nominatim DTOs and ViewModel classes.
/// This mapper is used to transform data transfer objects (DTOs) received from the Nominatim API into view models suitable for presentation in the user interface.
/// Implementations of this interface should provide methods to map specific DTO types to their corresponding ViewModel types.
/// </summary>
public interface INominatimDtoViewModelMapper
{
    /// <summary>
    /// Maps a Nominatim search result DTO to its corresponding ViewModel.
    /// Transforms the data from the Nominatim API into a ViewModel format suitable for presentation in the user interface.
    /// </summary>
    /// <param name="src">The source Nominatim search result DTO to be mapped.</param>
    /// <returns>A new instance of NominatimSearchResultViewModel populated with the data from the source DTO.</returns>
    public NominatimSearchResultViewModel MapToViewModel(NominatimSearchResultDto src);

    /// <summary>
    /// Maps a Nominatim detailed address DTO to its corresponding ViewModel.
    /// Transforms the data from the Nominatim API into a ViewModel format suitable for presentation in the user interface.
    /// </summary>
    /// <param name="src">The source Nominatim detailed address DTO to be mapped.</param>
    /// <returns>A new instance of NominatimDetailedAddressViewModel populated with the data from the source DTO.</returns>
    public NominatimDetailedAddressViewModel MapToViewModel(NominatimDetailedAddressDto src);

    /// <summary>
    /// Maps a Nominatim GeoJSON DTO to its corresponding ViewModel.
    /// Transforms the data from the Nominatim API into a ViewModel format suitable for presentation in the user interface.
    /// </summary>
    /// <param name="src">The source Nominatim GeoJSON DTO to be mapped.</param>
    /// <returns>A new instance of NominatimGeoJsonViewModel populated with the data from the source DTO.</returns>
    public NominatimGeoJsonViewModel MapToViewModel(NominatimGeoJsonDto src);
}