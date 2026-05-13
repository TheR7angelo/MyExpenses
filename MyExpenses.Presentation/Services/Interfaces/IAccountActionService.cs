using MyExpenses.Presentation.ViewModels.Accounts;
using MyExpenses.Presentation.ViewModels.Expenses;

namespace MyExpenses.Presentation.Services.Interfaces;

public interface IAccountActionService
{
    /// <summary>
    /// Manages the action to perform on a category type based on the given historical view model data.
    /// </summary>
    /// <param name="historyViewModel">The view model containing historical data, including details about the category type and associated entities.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests during the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task ManageCategoryTypeAction(HistoryViewModel historyViewModel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a category type based on the given input string.
    /// </summary>
    /// <param name="input">The input string representing the data required to create the category type.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests during the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task CreateCategoryType(string input, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a category type based on the data provided in the view model and additional input.
    /// </summary>
    /// <param name="categoryTypeViewModel">The view model containing the details of the category type to be updated, including name, color, and other attributes.</param>
    /// <param name="input">Additional input or metadata required to process the update operation.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests during the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous update operation.</returns>
    public Task UpdateCategoryType(CategoryTypeViewModel categoryTypeViewModel, string input, CancellationToken cancellationToken = default);

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
    public Task DeleteCategoryType(CategoryTypeViewModel categoryTypeViewModel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Manages the action to perform on an account type based on the given account view model state.
    /// </summary>
    /// <param name="accountViewModel">The view model representing the account data and its current state.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task ManageAccountTypeAction(AccountViewModel accountViewModel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Manages the action to perform on an account type based on the given account view model state.
    /// </summary>
    /// <param name="accountTypeViewModel">The view model representing the account type data.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>

    public Task ManageAccountTypeAction(AccountTypeViewModel? accountTypeViewModel = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new account type based on the provided account type view model.
    /// </summary>
    /// <param name="input">The name of the account type to be created.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task CreateAccountType(string input, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the details of an existing account type based on the provided view model and input values.
    /// </summary>
    /// <param name="accountTypeViewModel">The view model containing the account type's current data and modifications.</param>
    /// <param name="input">A string input containing additional data or updates for the account type.</param>
    /// <param name="cancellationToken">A token that can be used to propagate notification that the operation should be canceled.</param>
    /// <returns>A task representing the completion of the asynchronous update operation.</returns>
    public Task UpdateAccountType(AccountTypeViewModel accountTypeViewModel, string input, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an existing account type based on the given account type view model.
    /// </summary>
    /// <param name="accountTypeViewModel">The view model containing the account type information to be deleted.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests during the operation.</param>
    /// <returns>A task representing the asynchronous delete operation.</returns>
    public Task DeleteAccountType(AccountTypeViewModel accountTypeViewModel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Manages the action to perform on a currency based on the provided account view model data.
    /// </summary>
    /// <param name="accountViewModel">The view model containing account-related data required for managing the currency action.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests during the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task ManageCurrencyAction(AccountViewModel accountViewModel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new currency based on the provided input data.
    /// </summary>
    /// <param name="input">The input string containing the information required to create the currency.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests during the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task CreateCurrency(string input, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the details of a currency based on the provided currency view model and the given input.
    /// </summary>
    /// <param name="currencyViewModel">The view model containing the data for the currency to be updated.</param>
    /// <param name="input">A string containing additional information or parameters required for the update operation.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests during the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task UpdateCurrency(CurrencyViewModel currencyViewModel, string input, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a specified currency based on the provided view model data.
    /// </summary>
    /// <param name="currencyViewModel">The view model containing details of the currency to be deleted, including its identifier and related properties.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests during the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task DeleteCurrency(CurrencyViewModel currencyViewModel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an account based on the provided account view model data.
    /// </summary>
    /// <param name="accountViewModel">The view model containing data for the account that needs to be deleted.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests during the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task DeleteAccount(AccountViewModel accountViewModel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Edits the details of an existing account based on the provided account view model.
    /// </summary>
    /// <param name="accountViewModel">The view model containing details of the account to be edited.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests during the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation, returning a boolean indicating success or failure of the edit operation.</returns>
    public Task<bool> UpdateAccount(AccountViewModel accountViewModel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new account based on the provided account information and historical data.
    /// </summary>
    /// <param name="accountViewModel">The view model containing details and configurations for the account to be added.</param>
    /// <param name="historyViewModel">The view model containing historical data relevant to the account addition.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests during the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation, returning a boolean indicating the success or failure of the account addition.</returns>
    public Task<bool> CreateAccount(AccountViewModel accountViewModel, HistoryViewModel historyViewModel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new account based on the provided account view model.
    /// </summary>
    /// <param name="accountViewModel">The view model containing details of the account to be added, such as name, balance, and account type.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests during the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation, containing a boolean value indicating the success or failure of the operation.</returns>
    public Task<bool> CreateAccount(AccountViewModel accountViewModel, CancellationToken cancellationToken = default);
}