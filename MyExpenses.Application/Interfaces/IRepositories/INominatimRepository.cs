using Domain.Models.Nominatim;
using Domain.Models.Validation;

namespace MyExpenses.Application.Interfaces.IRepositories;

/// <summary>
/// Interface for repository operations related to nominatium data.
/// </summary>
public interface INominatimRepository
{
    /// <summary>
    /// Searches for locations near a given latitude and longitude.
    /// </summary>
    /// <param name="latitude">The latitude coordinate.</param>
    /// <param name="longitude">The longitude coordinate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation, returning a Result containing an IEnumerable of NominatimSearchResultDomain objects or an error if the operation fails.</returns>
    public Task<Result<IEnumerable<NominatimSearchResultDomain>>> SearchAsync(double latitude, double longitude, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for locations based on the provided address.
    /// </summary>
    /// <param name="address">The address to search for.</param>
    /// <param name="cancellationToken">A token to allow for cancellation of the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The result is a Result containing an IEnumerable of NominatimSearchResultDomain if successful, or a failure message if not.</returns>
    public Task<Result<IEnumerable<NominatimSearchResultDomain>>> SearchAsync(string address, CancellationToken cancellationToken = default);
}