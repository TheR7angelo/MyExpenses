using Domain.Models.Accounts;

namespace Domain.Interfaces;

public interface IAccountDomainValidationService
{
    /// <summary>
    /// Validates if the account name in the provided account data is valid based on predefined criteria.
    /// </summary>
    /// <param name="accountDomain">The account data to validate, which contains the account name and other details.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result contains true if the account name is valid; otherwise, false.
    /// </returns>
    public Task<bool> IsAccountNameValid(AccountDomain accountDomain, CancellationToken cancellationToken = default);
}