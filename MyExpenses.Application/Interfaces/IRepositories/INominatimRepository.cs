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
}