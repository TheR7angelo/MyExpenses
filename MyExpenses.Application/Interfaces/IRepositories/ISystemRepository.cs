using Domain.Models.Systems;
using Domain.Models.Validation;

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

    /// <summary>
    /// Creates a new color entry in the system.
    /// </summary>
    /// <param name="colorDomain">
    /// The domain model representing the color to be created.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests, allowing the operation to be cancelled.
    /// </param>
    /// <returns>
    /// A task containing a result object that includes the created color domain entity if successful.
    /// </returns>
    public Task<Result<ColorDomain>> CreateColorAsync(ColorDomain colorDomain, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing color in the system.
    /// </summary>
    /// <param name="colorDomain">
    /// The color domain model containing the updated color information.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests, allowing the operation to be cancelled.
    /// </param>
    /// <returns>
    /// A task containing the result of the update operation. The result indicates whether the update was successful
    /// and may include the updated color domain model or an error message.
    /// </returns>
    public Task<Result<ColorDomain>> UpdateColorAsync(ColorDomain colorDomain, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the specified color from the system.
    /// </summary>
    /// <param name="colorDomain">
    /// The color to be deleted, represented as a <see cref="ColorDomain"/> object.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests, allowing the operation to be cancelled.
    /// </param>
    /// <returns>
    /// A task containing a <see cref="DeletionResult"/> object which indicates the outcome of the deletion,
    /// including any dependencies affected.
    /// </returns>
    public Task<DeletionResult> DeleteColorAsync(ColorDomain colorDomain, CancellationToken cancellationToken = default);
}