using Domain.Models.Systems;

namespace MyExpenses.Application.Interfaces.IRepositories;

public interface ISystemRepository
{
    /// <summary>
    /// Retrieves the total count of distinct colors in the system.
    /// </summary>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests, allowing the operation to be cancelled.
    /// </param>
    /// <returns>
    /// A task containing the total count of distinct colors.
    /// </returns>
    public Task<int> GetAllColorCount(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a random color from the system.
    /// </summary>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests, allowing the operation to be cancelled.
    /// </param>
    /// <returns>
    /// A task containing the random color as a <c>ColorDomain</c> object.
    /// </returns>
    public Task<ColorDomain> GetRandomColor(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all available colors from the system.
    /// </summary>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests, allowing the operation to be canceled.
    /// </param>
    /// <returns>
    /// A task containing an enumerable collection of colors as <c>ColorDomain</c> objects.
    /// </returns>
    public Task<IEnumerable<ColorDomain>> GetAllColors(CancellationToken cancellationToken = default);

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
    /// Checks if a given color name is available in the system.
    /// </summary>
    /// <param name="name">
    /// The name of the color to check for availability.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests, allowing the operation to be cancelled.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation, containing a boolean value indicating whether the color name is available.
    /// </returns>
    public Task<bool> IsColorNameAvailableAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a specific hexadecimal color code is available within the system.
    /// </summary>
    /// <param name="hexadecimalCode">
    /// The hexadecimal color code to be checked for availability.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests, allowing the operation to be cancelled.
    /// </param>
    /// <returns>
    /// A task containing a boolean indicating whether the specified hexadecimal color code is available.
    /// </returns>
    public Task<bool> IsColorHexadecimalCodeAvailableAsync(string hexadecimalCode, CancellationToken cancellationToken = default);
}