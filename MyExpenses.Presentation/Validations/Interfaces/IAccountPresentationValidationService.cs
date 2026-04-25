using MyExpenses.Presentation.ViewModels.Accounts;

namespace MyExpenses.Presentation.Validations.Interfaces;

public interface IAccountPresentationValidationService
{
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