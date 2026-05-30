using Domain.Models.Validation;
using MyExpenses.Application.Dtos.Nominatium;

namespace MyExpenses.Application.Interfaces.IServices;

/// <summary>
/// Represents a service for interacting with the Nominatim geocoding API.
/// </summary>
public interface INominatimService
{
    /// <summary>
    /// Searches for locations near the given latitude and longitude.
    /// </summary>
    /// <param name="latitude">The latitude of the location.</param>
    /// <param name="longitude">The longitude of the location.</param>
    /// <param name="cancellationToken">A token to allow cancellation of the operation.</param>
    /// <returns>A task that represents the asynchronous operation, returning a result containing a list of search results or an error.</returns>
    public Task<Result<IEnumerable<NominatimSearchResultDto>>> SearchAsync(double latitude, double longitude, CancellationToken cancellationToken = default);
}