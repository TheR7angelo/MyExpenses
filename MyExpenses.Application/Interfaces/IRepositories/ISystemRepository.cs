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
}