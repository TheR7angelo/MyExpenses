using Domain.Models.Validation;
using Mapsui;
using MyExpenses.Presentation.ViewModels.Locations;

namespace MyExpenses.Presentation.Services.Interfaces;

public interface INominatimPresentationService
{
    /// <summary>
    /// Performs an asynchronous search using the Nominatim API.
    /// </summary>
    /// <param name="point">The geographical point to search around.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A Result object containing a collection of NominatimSearchResultViewModel if successful, or an error code and message if not.</returns>
    public Task<Result<IEnumerable<NominatimSearchResultViewModel>>> SearchAsync(MPoint point, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for locations based on the provided latitude and longitude.
    /// </summary>
    /// <param name="latitude">The latitude of the location to search for.</param>
    /// <param name="longitude">The longitude of the location to search for.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A result containing an enumerable of NominatimSearchResultViewModel objects or an error if the operation fails.</returns>
    public Task<Result<IEnumerable<NominatimSearchResultViewModel>>> SearchAsync(double latitude, double longitude, CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs an asynchronous search for locations based on the provided address.
    /// </summary>
    /// <param name="address">The address to search for.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous search operation, returning a Result containing a collection of NominatimSearchResultViewModel objects.</returns>
    public Task<Result<IEnumerable<NominatimSearchResultViewModel>>> SearchAsync(string address, CancellationToken cancellationToken = default);
}