using Domain.Models.Dependencies;
using MyExpenses.Application.Dtos.Accounts;
using MyExpenses.Application.Dtos.Expenses;
using MyExpenses.Application.Dtos.Systems;

namespace MyExpenses.Application.Interfaces.IServices;

public interface ISystemService
{
    /// <summary>
    /// Retrieves a collection of dependencies associated with the specified account type. Each dependency includes details such as label, count, and category.
    /// </summary>
    /// <param name="accountTypeDto">The account type data transfer object used to identify dependencies.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="DeletionDependency"/> objects.</returns>
    public Task<IEnumerable<DeletionDependency>> GetAllDependenciesAsync(AccountTypeDto accountTypeDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a collection of dependencies associated with the specified category type. Each dependency includes details such as count and category.
    /// </summary>
    /// <param name="categoryTypeDto">The category type data transfer object used to identify dependencies.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="DeletionDependency"/> objects.</returns>
    public Task<IEnumerable<DeletionDependency>> GetAllDependenciesAsync(CategoryTypeDto categoryTypeDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a collection of dependencies associated with the specified currency type. Each dependency includes details such as count and category.
    /// </summary>
    /// <param name="currencyDto">The currency type data transfer object used to identify dependencies.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="DeletionDependency"/> objects.</returns>
    public Task<IEnumerable<DeletionDependency>> GetAllDependenciesAsync(CurrencyDto currencyDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all deletion dependencies for the given account.
    /// </summary>
    /// <param name="accountDto">The account data transfer object for which dependencies are to be retrieved.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of deletion dependencies associated with the account.</returns>
    public Task<IEnumerable<DeletionDependency>> GetAllDependenciesAsync(AccountDto accountDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a randomly selected color, including its name and hexadecimal color code.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="ColorDto"/> object with details about the color.</returns>
    public Task<ColorDto> GetRandomColor(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a collection of all available colors in the system. Each color includes details such as its name, hexadecimal color code, and the date it was added.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="ColorDto"/> objects.</returns>
    public Task<IEnumerable<ColorDto>> GetAllColors(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves details of a specific place based on the provided identifier.
    /// </summary>
    /// <param name="placeId">The unique identifier of the place to retrieve.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="PlaceDto"/> object with details of the place.</returns>
    public Task<PlaceDto?> GetPlace(int placeId, CancellationToken cancellationToken = default);
}