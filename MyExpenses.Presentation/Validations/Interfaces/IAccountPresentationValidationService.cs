using MyExpenses.Presentation.ViewModels.Accounts;

namespace MyExpenses.Presentation.Validations.Interfaces;

public interface IAccountPresentationValidationService
{
    /// <summary>
    /// Validates the given account to ensure it meets the necessary criteria for the application's requirements.
    /// </summary>
    /// <param name="accountViewModel">The account view model containing account details to be validated.</param>
    /// <param name="cancellationToken">A cancellation token to observe while awaiting the task to complete.</param>
    /// <returns>A task that represents the asynchronous validation operation. The task result is a boolean indicating whether the account is valid.</returns>
    public Task<bool> IsAccountValid(AccountViewModel accountViewModel, CancellationToken cancellationToken = default);
}