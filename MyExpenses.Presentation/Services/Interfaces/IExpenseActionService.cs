using Domain.Models.Validation;
using MyExpenses.Presentation.ViewModels.Expenses;

namespace MyExpenses.Presentation.Services.Interfaces;

public interface IExpenseActionService
{
    /// <summary>
    /// Creates an expense using the provided historical data.
    /// </summary>
    /// <param name="historyViewModel">The view model containing historical data that can be used to create the expense.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation. The result indicates whether the expense was successfully created.</returns>```csharp
    public Task<bool> CreateExpense(HistoryViewModel historyViewModel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an expense using the provided historical data.
    /// </summary>
    /// <param name="historyViewModel">The view model containing historical data that can be used to update the expense.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation. The result indicates whether the expense was successfully updated.</returns>
    public Task<bool> UpdateExpense(HistoryViewModel historyViewModel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the history using the provided historical data.
    /// </summary>
    /// <param name="historyViewModel">The view model containing historical data that can be used to delete the history.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation. The result indicates whether the history was successfully deleted.</returns>
    public Task<bool> DeleteHistory(HistoryViewModel historyViewModel, CancellationToken cancellationToken = default);

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

    /// <summary>
    /// Manages the payment mode action using the provided mode payment view model.
    /// </summary>
    /// <param name="modePaymentViewModel">The view model containing details about the payment mode, including ID, name, and whether it can be deleted.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation. The result is a boolean indicating success or failure of the operation.</returns>
    public Task ManageModePaymentAction(ModePaymentViewModel? modePaymentViewModel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a mode payment based on the provided input and cancellation token.
    /// </summary>
    /// <param name="input">The input data required to create the mode payment.</param>
    /// <param name="cancellationToken">A token to allow for cancellation of the operation.</param>
    /// <returns>A task representing the asynchronous operation, which will be true if successful.</returns>
    public Task CreateModePayment(string input, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the mode payment using the provided mode payment view model and input.
    /// </summary>
    /// <param name="modePaymentViewModel">The view model containing details about the mode of payment.</param>
    /// <param name="input">Additional input related to the update operation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation. The result is a boolean indicating whether the update was successful.</returns>
    public Task UpdateModePayment(ModePaymentViewModel modePaymentViewModel, string input, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a mode of payment.
    /// </summary>
    /// <param name="modePaymentViewModel">The view model containing details about the mode of payment to be deleted.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation. Returns true if the deletion was successful, otherwise false.</returns>
    public Task DeleteModePayment(ModePaymentViewModel modePaymentViewModel, CancellationToken cancellationToken = default);
}