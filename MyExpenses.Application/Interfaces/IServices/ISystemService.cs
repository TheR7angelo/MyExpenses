using Domain.Models.Dependencies;
using Domain.Models.Validation;
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
    /// Retrieves a collection of dependencies associated with the specified color.
    /// Each dependency includes details such as label, count, and category.
    /// </summary>
    /// <param name="colorDto">The color data transfer object used to identify dependencies.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Result{T}"/> object holding an enumerable collection of <see cref="DeletionDependency"/> objects.</returns>
    public Task<Result<IEnumerable<DeletionDependency>>> GetAllDependenciesAsync(ColorDto colorDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all dependencies for a specific place. The type of place is determined by its DTO passed as an argument.
    /// </summary>
    /// <param name="placeDto">The place's details in the form of a Data Transfer Object (DTO).</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of DeletionDependency objects representing all dependencies related to the place specified by its DTO.</returns>
    public Task<Result<IEnumerable<DeletionDependency>>> GetAllDependenciesAsync(PlaceDto placeDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a collection of dependencies associated with the specified mode payment.
    /// </summary>
    /// <param name="modePaymentDto">The mode payment data transfer object used to identify dependencies.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="DeletionDependency"/> objects.</returns>
    public Task<Result<IEnumerable<DeletionDependency>>> GetAllDependenciesAsync(ModePaymentDto modePaymentDto, CancellationToken cancellationToken = default);

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
    /// Checks if the specified color name is available for use.
    /// </summary>
    /// <param name="name">The name of the color to validate for availability.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the color name is available.</returns>z
    public Task<bool> IsColorNameAvailableAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a given color hexadecimal code is available for use.
    /// </summary>
    /// <param name="hexadecimalCode">The hexadecimal code of the color to be validated.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result indicates whether the color hexadecimal code is available.</returns>
    public Task<bool> IsColorHexadecimalCodeAvailableAsync(string hexadecimalCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new color entry using the provided color data transfer object.
    /// </summary>
    /// <param name="colorDto">The data transfer object containing details of the color to be added, including name and hexadecimal code.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Result{T}"/> object with the created <see cref="ColorDto"/>.</returns>
    public Task<Result<ColorDto>> CreateColorAsync(ColorDto colorDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the details of an existing color in the system based on the provided color data transfer object.
    /// </summary>
    /// <param name="colorDto">The data transfer object containing updated color details, such as name and hexadecimal color code.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Result{ColorDto}"/> indicating the success or failure of the update operation.</returns>
    public Task<Result<ColorDto>> UpdateColorAsync(ColorDto colorDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a specified color and handles its associated deletion dependencies.
    /// </summary>
    /// <param name="colorDto">The data transfer object representing the color to be deleted.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="DeletionResult"/> indicating the outcome of the delete operation, including details of dependencies removed, if any.</returns>
    public Task<DeletionResult> DeleteColorAsync(ColorDto colorDto, CancellationToken cancellationToken = default);
}