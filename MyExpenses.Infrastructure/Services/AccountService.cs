using MyExpenses.Application.Dtos.Accounts;
using MyExpenses.Application.Interfaces;
using MyExpenses.Application.Interfaces.IRepositories;
using MyExpenses.Application.Interfaces.IServices;
using MyExpenses.Application.Interfaces.Mappings;

namespace MyExpenses.Infrastructure.Services;

public class AccountService(IAccountRepository accountRepository, IAccountDtoDomainMapper mapper)
    : IAccountService
{
    public async Task<IEnumerable<TotalByAccountDto>> GetAllTotalByAccountAsync(CancellationToken cancellationToken = default)
    {
        var totalByAccountDomain = await accountRepository.GetTotalByAccountAsync(cancellationToken);
        return totalByAccountDomain.Select(mapper.MapToDto);
    }

    public async Task<IEnumerable<AccountDto>> GetAllAccountAsync(CancellationToken cancellationToken = default)
    {
        var accounts = await accountRepository.GetAllAccountAsync(cancellationToken);
        return accounts.Select(mapper.MapToDto);
    }

    public async Task<IEnumerable<AccountTypeDto>> GetAllAccountTypeAsync(CancellationToken cancellationToken = default)
    {
        var accountTypes = await accountRepository.GetAllAccountTypeAsync(cancellationToken);
        return accountTypes.Select(mapper.MapToDto);
    }

    public async Task<IEnumerable<CurrencyDto>> GetAllCurrencyAsync(CancellationToken cancellationToken = default)
    {
        var currencies = await accountRepository.GetAllCurrencyAsync(cancellationToken);
        return currencies.Select(mapper.MapToDto);
    }

    // public async Task<AccountDto> AddOrEditAsync(AccountDto accountDto, CancellationToken cancellationToken = default)
    // {
    //
    // }
}