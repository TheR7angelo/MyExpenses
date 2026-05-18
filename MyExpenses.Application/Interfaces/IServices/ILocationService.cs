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
}