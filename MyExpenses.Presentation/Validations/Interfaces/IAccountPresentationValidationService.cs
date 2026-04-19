using MyExpenses.Presentation.ViewModels.Accounts;

namespace MyExpenses.Presentation.Validations.Interfaces;

public interface IAccountPresentationValidationService
{
    // /// <summary>
    // /// Validates the given account to ensure it meets the necessary criteria for the application's requirements.
    // /// </summary>
    // /// <param name="accountViewModel">The account view model containing account details to be validated.</param>
    // /// <param name="cancellationToken">A cancellation token to observe while awaiting the task to complete.</param>
    // /// <returns>A task that represents the asynchronous validation operation. The task result is a boolean indicating whether the account is valid.</returns>
    // public Task<Result> IsAccountValid(AccountViewModel accountViewModel, CancellationToken cancellationToken = default);

    // /// <summary>
    // /// Validates the provided account type to ensure it meets the application's requirements for account types.
    // /// </summary>
    // /// <param name="accountTypeViewModel">The account type view model containing details of the account type to be validated.</param>
    // /// <param name="cancellationToken">A cancellation token to observe while awaiting the task to complete.</param>
    // /// <returns>A task representing the asynchronous validation operation. The task result is a boolean indicating whether the account type is valid.</returns>
    // public Task<Result> IsAccountTypeValid(AccountTypeViewModel accountTypeViewModel,
    //     CancellationToken cancellationToken = default);

    // /// <summary>
    // /// Validates the provided account type to ensure it meets the application's requirements for account types.
    // /// </summary>
    // /// <param name="accountTypeViewModel">The account type view model containing details of the account type to be validated.</param>
    // /// <param name="cancellationToken">A cancellation token to observe while awaiting the task to complete.</param>
    // /// <returns>A task that represents the asynchronous validation operation. The task result is a Result indicating the success or failure of the validation.</returns>
    // public Task<Result> IsAccountTypeValid(AccountTypeViewModel accountTypeViewModel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the provided account type name is available for use.
    /// </summary>
    /// <param name="input">The account type name to check for availability.</param>
    /// <param name="accountTypeViewModel">The account type view model containing additional account type details.</param>
    /// <param name="cancellationToken">A cancellation token to observe while awaiting the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is a boolean indicating whether the account type name is available.</returns>
    public Task<bool> IsAccountTypeNameAvailableAsync(string input, AccountTypeViewModel accountTypeViewModel,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the provided account type name is available for use.
    /// </summary>
    /// <param name="input">The account type name to check for availability.</param>
    /// <param name="cancellationToken">A cancellation token to observe while awaiting the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is a boolean indicating whether the account type name is available.</returns>
    public Task<bool> IsAccountTypeNameAvailableAsync(string input, CancellationToken cancellationToken = default);
}