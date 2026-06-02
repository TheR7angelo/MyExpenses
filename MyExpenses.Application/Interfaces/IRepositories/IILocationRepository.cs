using Domain.Models.Systems;
using Domain.Models.Validation;

namespace MyExpenses.Application.Interfaces.IRepositories;

public interface ILocationRepository
{
    /// <summary>
    /// Retrieves information about a specific place in the system.
    /// </summary>
    /// <param name="placeId">
    /// The unique identifier of the place to retrieve.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests, allowing the operation to be canceled.
    /// </param>
    /// <returns>
    /// A task containing the place information as a <c>PlaceDomain</c> object, or null if no place exists with the given identifier.
    /// </returns>
    public Task<PlaceDomain?> GetPlace(int placeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all places available in the system.
    /// </summary>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests, allowing the operation to be cancelled.
    /// </param>
    /// <returns>
    /// A task containing a result object that encapsulates a collection of places.
    /// </returns>
    public Task<Result<IEnumerable<PlaceDomain>>> GetAllPlaces(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new place in the system.
    /// </summary>
    /// <param name="placeDomain">
    /// The place domain object containing information about the place to create.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests, allowing the operation to be canceled.
    /// </param>
    /// <returns>
    /// A task containing the result of the creation operation as a <c>Result<PlaceDomain></c> object. If successful, it contains the created place; otherwise, it includes an error code and message.
    /// </returns>
    public Task<Result<PlaceDomain>> CreatePlaceAsync(PlaceDomain placeDomain, CancellationToken cancellationToken = default);
}