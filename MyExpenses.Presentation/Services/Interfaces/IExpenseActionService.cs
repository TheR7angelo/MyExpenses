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
}