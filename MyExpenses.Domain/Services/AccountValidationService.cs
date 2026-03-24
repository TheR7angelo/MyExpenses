using Domain.Interfaces;
using Domain.Models.Accounts;

namespace Domain.Services;

public class AccountValidationService : IAccountValidationService
{
    public Task<bool> IsAccountNameValid(AccountDomain accountDomain, CancellationToken cancellationToken = default)
    {
        try
        {
            return Task.FromResult(!string.IsNullOrWhiteSpace(accountDomain.Name) && accountDomain.Name.Length <= AccountDomain.MaxNameLength);
        }
        catch (Exception exception)
        {
            return Task.FromException<bool>(exception);
        }
    }
}