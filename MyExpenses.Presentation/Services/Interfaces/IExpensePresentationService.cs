using Domain.Models;
using Domain.Models.Validation;
using MyExpenses.Presentation.ViewModels.Accounts;
using MyExpenses.Presentation.ViewModels.Expenses;

namespace MyExpenses.Presentation.Services.Interfaces;

public interface IExpensePresentationService
{
    /// <summary>
    /// Retrieves a list of all category type view models.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a collection of category type view models or an error if the operation fails.</returns>
    public Task<Result<IEnumerable<CategoryTypeViewModel>>> GetAllCategoryTypeViewModelAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new category type.
    /// </summary>
    /// <param name="newCategoryType">The category type to be added, containing its details such as name and color.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the outcome of the addition operation.</returns>
    public Task<Result<CategoryTypeViewModel>> CreateCategoryType(CategoryTypeViewModel newCategoryType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the provided category type name is available for use.
    /// </summary>
    /// <param name="input">The category type name to check for availability.</param>
    /// <param name="cancellationToken">A cancellation token to observe while awaiting the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is a boolean indicating whether the category type name is available.</returns>
    public Task<bool> IsCategoryTypeNameAvailableAsync(string input, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a specified category type asynchronously.
    /// </summary>
    /// <param name="categoryTypeViewModel">The category type view model to be deleted.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <return>A task representing the asynchronous operation. The task result contains the outcome of the deletion process.</return>
    public Task<DeletionResult> DeleteCategoryTypeAsync(CategoryTypeViewModel categoryTypeViewModel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the name of a category type.
    /// </summary>
    /// <param name="categoryTypeViewModel">The category type view model containing the updated name.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <return>A task representing the asynchronous operation. The task result contains the result of the update operation.</return>
    public Task<Result> UpdateCategoryTypeName(CategoryTypeViewModel categoryTypeViewModel,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new expense entry based on the provided history view model.
    /// </summary>
    /// <param name="historyViewModel">The view model containing details of the expense to be created.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <return>A task representing the asynchronous operation. The task result contains the created expense wrapped in a Result object.</return>
    public Task<Result<HistoryViewModel>> CreateExpense(HistoryViewModel historyViewModel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing expense.
    /// </summary>
    /// <param name="historyViewModel">The history view model containing the updated expense data.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a validation result indicating whether the update was successful or not.</returns>
    public Task<Result<HistoryViewModel>> UpdateExpense(HistoryViewModel historyViewModel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a history entry.
    /// </summary>
    /// <param name="historyViewModel">The history entry to delete.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a deletion result or an error if the operation fails.</returns>
    public Task<DeletionResult> DeleteHistory(HistoryViewModel historyViewModel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of all mode payment view models.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a collection of mode payment view models or an error if the operation fails.</returns>
    public Task<Result<IEnumerable<ModePaymentViewModel>>> GetAllModePaymentViewModelAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the mode of payment view model based on the specified mode payment ID.
    /// </summary>
    /// <param name="modePaymentId">The unique identifier of the mode payment to retrieve.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <return>A task representing the asynchronous operation. The task result contains the mode of payment view model if found, or null if it does not exist.</return>
    public Task<ModePaymentViewModel?> GetModePaymentViewModel(int modePaymentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Merges the specified source bank transfer view model into the destination history view model.
    /// </summary>
    /// <param name="src">The source bank transfer view model containing information to be merged.</param>
    /// <param name="dst">The destination history view model to which the data will be merged.</param>
    public void Merge(BankTransferViewModel src, HistoryViewModel dst);

    /// <summary>
    /// Creates a bank transfer by associating a bank transfer view model with a history view model.
    /// </summary>
    /// <param name="bankTransferViewModel">The bank transfer details to be used for the operation.</param>
    /// <param name="historyViewModel">The history details associated with the bank transfer.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a <see cref="Result{T}"/> object,
    /// which holds the created bank transfer and history view models on success.</returns>
    public Task<Result<(BankTransferViewModel bankTransferViewModel, IEnumerable<HistoryViewModel> historyViewModel)>>
        CreateBankTransferAsync(BankTransferViewModel bankTransferViewModel, HistoryViewModel historyViewModel,
            CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new mode of payment.
    /// </summary>
    /// <param name="newModePayment">The model view of the new mode of payment.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the result of the operation including the created mode payment if successful.</returns>
    public Task<Result<ModePaymentViewModel>> CreateModePayment(ModePaymentViewModel newModePayment, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a mode payment.
    /// </summary>
    /// <param name="modePaymentViewModel">The mode payment view model to update.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a result indicating success or failure of the operation and any associated error code or message.</returns>
    public Task<Result<ModePaymentViewModel>> UpdateModePayment(ModePaymentViewModel modePaymentViewModel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a mode payment asynchronously.
    /// </summary>
    /// <param name="modePaymentViewModel">The mode payment view model to delete.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a deletion result.</returns>
    public Task<DeletionResult> DeleteModePaymentAsync(ModePaymentViewModel modePaymentViewModel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of all active recurrences for the specified year and month.
    /// </summary>
    /// <param name="year">The year to retrieve active recurrences for.</param>
    /// <param name="month">The month to retrieve active recurrences for.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a collection of recursive expense view models or an error if the operation fails.</returns>
    public Task<Result<IEnumerable<RecursiveExpenseViewModel>>> GetAllActiveRecurrences(int year, int month, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of all expense years.
    /// </summary>
    /// <param name="sortOrder">The sort order for the expense years. Use SortOrder.Ascending for ascending order and SortOrder.Descending for descending order.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a collection of expense years or an error if the operation fails.</returns>
    public Task<Result<IEnumerable<int>>> GetAllExpenseYear(SortOrder sortOrder, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of expenses for a specific account.
    /// </summary>
    /// <param name="accountId">The ID of the account.</param>
    /// <param name="selectedYear">Optional. The year to filter expenses.</param>
    /// <param name="selectedMonth">Optional. The month to filter expenses.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a collection of expense view models or an error if the operation fails.</returns>
    public Task<Result<IEnumerable<HistoryViewModel>>> GetAllExpenses(int accountId, int? selectedYear = null,
        int? selectedMonth = null, CancellationToken cancellationToken = default);
}