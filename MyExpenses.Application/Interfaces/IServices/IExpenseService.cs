using Domain.Models.Validation;
using MyExpenses.Application.Dtos.Expenses;

namespace MyExpenses.Application.Interfaces.IServices;

public interface IExpenseService
{
    /// <summary>
    /// Retrieves all category types available in the system.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="CategoryTypeDto"/> objects.</returns>
    public Task<IEnumerable<CategoryTypeDto>> GetAllCategoryTypesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new category type to the system.
    /// </summary>
    /// <param name="categoryTypeDto">The data transfer object representing the category type to be added, including its name, color, and other relevant details.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Result{T}"/> object with the created category type details encapsulated in a <see cref="CategoryTypeDto"/>.</returns>
    public Task<Result<CategoryTypeDto>> CreateCategoryTypeAsync(CategoryTypeDto categoryTypeDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a specified category type from the system.
    /// </summary>
    /// <param name="categoryTypeDto">An object representing the category type to be deleted.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="DeletionResult"/> indicating the outcome of the operation, including any deleted dependencies.</returns>
    public Task<DeletionResult> DeleteCategoryTypeAsync(CategoryTypeDto categoryTypeDto,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the name of an existing category type in the system.
    /// </summary>
    /// <param name="categoryTypeDto">An object containing the updated details of the category type, including the new name.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Result"/> indicating the success or failure of the operation.</returns>
    public Task<Result> UpdateCategoryTypeNameAsync(CategoryTypeDto categoryTypeDto,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new expense entry in the system.
    /// </summary>
    /// <param name="historyDto">An object containing details of the expense to be created.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Result{T}"/> object with the created expense details encapsulated in a <see cref="HistoryDto"/>.</returns>
    public Task<Result<HistoryDto>> CreateExpenseAsync(HistoryDto historyDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all available modes of payment in the system.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="ModePaymentDto"/> object representing the collection of all modes of payment.</returns>
    public Task<IEnumerable<ModePaymentDto>> GetAllModePaymentAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the mode of payment details for a specified ID.
    /// </summary>
    /// <param name="modePaymentId">The unique identifier of the mode of payment.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="ModePaymentDto"/> object representing the mode of payment information, or null if not found.</returns>
    public Task<ModePaymentDto?> GetModePaymentByIdAsync(int modePaymentId, CancellationToken cancellationToken = default);
}