using MyExpenses.Application.Dtos.Accounts;
using MyExpenses.Application.Interfaces;
using MyExpenses.Application.Interfaces.IRepositories;
using MyExpenses.Application.Interfaces.IServices;
using MyExpenses.Application.Interfaces.Mappings;

namespace MyExpenses.Infrastructure.Services;

public class AccountServices(IAccountRepository accountRepository, IAccountDtoDomainMapper mapper)
    : IAccountServices
{
    public async Task<IEnumerable<TotalByAccountDto>> GetAllTotalByAccountAsync(CancellationToken cancellationToken = default)
    {
        var totalByAccountDomain = await accountRepository.GetTotalByAccountAsync(cancellationToken);
        return totalByAccountDomain.Select(mapper.MapToDto);
    }

    public Task<IEnumerable<string>> GetAllAccountNames(CancellationToken cancellationToken = default)
    {
        var accountNames = accountRepository.GetAllAccountNames(cancellationToken);
        return accountNames;
    }

    public async Task<IEnumerable<AccountDto>> GetAllAccountAsync(CancellationToken cancellationToken = default)
    {
        var accounts = await accountRepository.GetAllAccountAsync(cancellationToken);
        return accounts.Select(mapper.MapToDto);
    }
}