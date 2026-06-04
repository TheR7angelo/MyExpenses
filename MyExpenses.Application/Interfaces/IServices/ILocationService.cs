using Domain.Models.Validation;
using MyExpenses.Application.Dtos.Systems;

namespace MyExpenses.Application.Interfaces.IServices;

public interface ILocationService
{
    /// <summary>
    /// Retrieves details of a specific place based on the provided identifier.
    /// </summary>
    /// <param name="placeId">The unique identifier of the place to retrieve.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="PlaceDto"/> object with details of the place.</returns>
    public Task<PlaceDto?> GetPlace(int placeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a collection of all places available in the system, including details such as name, address, and geographical location.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Result"/> object with an enumerable collection of <see cref="PlaceDto"/> objects.</returns>
    public Task<Result<IEnumerable<PlaceDto>>> GetAllPlaces(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new place asynchronously.
    /// </summary>
    /// <param name="placeDto">The data transfer object containing details of the place to create.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Result{PlaceDto}"/> object with the created place or an error if the creation fails.</returns>
    public Task<Result<PlaceDto>> CreatePlaceAsync(PlaceDto placeDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing place with the provided details.
    /// </summary>
    /// <param name="placeDto">The updated place information.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Result{PlaceDto}"/> object indicating the success or failure of the update operation and containing the updated place details if successful.</returns>
    public Task<Result<PlaceDto>> UpdatePlaceAsync(PlaceDto placeDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a specified place and handles its associated deletion dependencies.
    /// </summary>
    /// <param name="placeDto">The data transfer object representing the place to be deleted.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="DeletionResult"/> indicating the outcome of the delete operation, including details of dependencies removed, if any.</returns>
    public Task<DeletionResult> DeletePlaceAsync(PlaceDto placeDto, CancellationToken cancellationToken);
}