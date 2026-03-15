using Domain.Models.Accounts;

namespace MyExpenses.Application.Interfaces.IRepositories;

public interface IAccountRepository
{
    public Task<IEnumerable<TotalByAccountDomain>> GetTotalByAccountAsync(CancellationToken cancellationToken = default);

    public Task<IEnumerable<string>> GetAllAccountNames(CancellationToken cancellationToken = default);

    public Task<IEnumerable<AccountDomain>> GetAllAccountAsync(CancellationToken cancellationToken = default);

    public Task<IEnumerable<AccountTypeDomain>> GetAllAccountTypeAsync(CancellationToken cancellationToken = default);

    public Task<IEnumerable<CurrencyDomain>> GetAllCurrencyAsync(CancellationToken cancellationToken = default);
}