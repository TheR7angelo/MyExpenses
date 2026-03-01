using Domain.Models.Accounts;

namespace MyExpenses.Infrastructure.Repositories;

public interface IAccountRepository
{
    public Task<IEnumerable<TotalByAccountDomain>> GetTotalByAccountAsync();
}