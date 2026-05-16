using MyExpenses.Presentation.ViewModels.Expenses;

namespace MyExpenses.Presentation.Services.Interfaces;

public interface IExpenseActionService
{
    /// <summary>
    /// Validates a bank transfer using the provided bank transfer details and historical data.
    /// </summary>
    /// <param name="bankTransferViewModel">The view model containing details about the bank transfer, including accounts, value, date, and reasons for the transfer.</param>
    /// <param name="historyViewModel">The view model containing historical data that can be used to validate the bank transfer.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task<bool> ValidateBankTransfer(BankTransferViewModel bankTransferViewModel,
        HistoryViewModel historyViewModel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a bank transfer using the provided bank transfer details and historical data.
    /// </summary>
    /// <param name="bankTransferViewModel">The view model containing details about the bank transfer, including accounts, value, date, and reasons for the transfer.</param>
    /// <param name="historyViewModel">The view model containing historical data associated with the bank transfer.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task CreateBankTransfer(BankTransferViewModel bankTransferViewModel, HistoryViewModel historyViewModel,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a category type based on the given view model.
    /// </summary>
    /// <param name="categoryTypeViewModel">The view model containing the details of the category type to be created, including name, color, and other attributes.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests during the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task<bool> CreateCategoryType(CategoryTypeViewModel categoryTypeViewModel,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing category type with the specified details in the provided view model.
    /// </summary>
    /// <param name="categoryTypeViewModel">The view model containing the updated details of the category type, including name and color.</param>
    /// <param name="cancellationToken">The cancellation token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation, returning a boolean indicating whether the update was successful.</returns>
    public Task<bool> UpdateCategoryType(CategoryTypeViewModel categoryTypeViewModel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a category type based on the provided category type view model.
    /// </summary>
    /// <param name="categoryTypeViewModel">
    /// The view model containing details of the category type to be deleted.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to observe for cancellation requests during the asynchronous operation.
    /// </param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task<bool> DeleteCategoryType(CategoryTypeViewModel categoryTypeViewModel, CancellationToken cancellationToken = default);
}