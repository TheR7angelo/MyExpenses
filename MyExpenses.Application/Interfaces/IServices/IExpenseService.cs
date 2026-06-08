using Domain.Models.Validation;
using MyExpenses.Application.Dtos.Expenses;

namespace MyExpenses.Application.Interfaces.IServices;

public interface IExpenseService
{
    /// <summary>
    /// Retrieves a list of all category types from the system.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Result{T}"/> object with a collection of <see cref="CategoryTypeDto"/> representing all category types.</returns>
    public Task<Result<IEnumerable<CategoryTypeDto>>> GetAllCategoryTypesAsync(CancellationToken cancellationToken = default);

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
    /// Updates an existing expense in the system.
    /// </summary>
    /// <param name="historyDto">The updated expense data transfer object.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Result{HistoryDto}"/> object representing the updated expense or an error if the update failed.</returns>
    public Task<Result<HistoryDto>> UpdateExpenseAsync(HistoryDto historyDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of all mode payment types from the system.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Result{T}"/> object with a collection of <see cref="ModePaymentDto"/> representing all mode payment types.</returns>
    public Task<Result<IEnumerable<ModePaymentDto>>> GetAllModePaymentAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the mode of payment details for a specified ID.
    /// </summary>
    /// <param name="modePaymentId">The unique identifier of the mode of payment.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="ModePaymentDto"/> object representing the mode of payment information, or null if not found.</returns>
    public Task<ModePaymentDto?> GetModePaymentByIdAsync(int modePaymentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Initiates the creation of a bank transfer, along with the associated expense history entries.
    /// </summary>
    /// <param name="bankTransferDto">The data object containing details of the bank transfer, including accounts, value, and reason.</param>
    /// <param name="historyDto">The data object representing the expense history entry related to the bank transfer.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains a <see cref="Result{T}"/> of a tuple consisting of the created <see cref="BankTransferDto"/> and an enumerable collection of <see cref="HistoryDto"/> objects.</returns>
    public Task<Result<(BankTransferDto bankTransfer, IEnumerable<HistoryDto> historyDtos)>> CreateBankTransferAsync(
        BankTransferDto bankTransferDto, HistoryDto historyDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new mode of payment.
    /// </summary>
    /// <param name="modePaymentDto">The mode payment to create.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Result{ModePaymentDto}"/> object indicating success or failure of the operation.</returns>
    public Task<Result<ModePaymentDto>> CreateModePaymentAsync(ModePaymentDto modePaymentDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing mode of payment.
    /// </summary>
    /// <param name="modePaymentDto">The mode of payment to update.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Result{ModePaymentDto}"/> indicating success or failure and containing the updated mode of payment.</returns>
    public Task<Result<ModePaymentDto>> UpdateModePaymentAsync(ModePaymentDto modePaymentDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a mode of payment asynchronously.
    /// </summary>
    /// <param name="modePaymentDto">The mode payment to delete.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a deletion result indicating whether the deletion was successful and any deleted items.</returns>
    public Task<DeletionResult> DeleteModePaymentAsync(ModePaymentDto modePaymentDto, CancellationToken cancellationToken = default);
}