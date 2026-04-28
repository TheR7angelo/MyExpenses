using Domain.Models.Validation;
using MyExpenses.Application.Dtos.Accounts;
using MyExpenses.Application.Interfaces.IRepositories;
using MyExpenses.Application.Interfaces.IServices;
using MyExpenses.Application.Interfaces.Mappings;

namespace MyExpenses.Infrastructure.Services;

public class AccountService(IAccountRepository accountRepository,
    IAccountDtoDomainMapper mapperAccount)
    : IAccountService
{
    public async Task<IEnumerable<TotalByAccountDto>> GetAllTotalByAccountAsync(CancellationToken cancellationToken = default)
    {
        var totalByAccountDomain = await accountRepository.GetTotalByAccountAsync(cancellationToken);
        return totalByAccountDomain.Select(mapperAccount.MapToDto);
    }

    public async Task<IEnumerable<AccountDto>> GetAllAccountAsync(CancellationToken cancellationToken = default)
    {
        var accounts = await accountRepository.GetAllAccountAsync(cancellationToken);
        return accounts.Select(mapperAccount.MapToDto);
    }

    public async Task<AccountDto?> GetAccountAsync(int id, CancellationToken cancellationToken = default)
    {
        var account = await accountRepository.GetAccountAsync(id, cancellationToken);
        return account is null ? null : mapperAccount.MapToDto(account);
    }

    public async Task<IEnumerable<AccountTypeDto>> GetAllAccountTypeAsync(CancellationToken cancellationToken = default)
    {
        var accountTypes = await accountRepository.GetAllAccountTypeAsync(cancellationToken);
        return accountTypes.Select(mapperAccount.MapToDto);
    }

    public async Task<IEnumerable<CurrencyDto>> GetAllCurrencyAsync(CancellationToken cancellationToken = default)
    {
        var currencies = await accountRepository.GetAllCurrencyAsync(cancellationToken);
        return currencies.Select(mapperAccount.MapToDto);
    }

    public async Task<DeletionResult> DeleteAccountTypeAsync(AccountTypeDto accountTypeDto, CancellationToken cancellationToken = default)
    {
        var accountType = mapperAccount.MapToDomain(accountTypeDto);
        return await accountRepository.DeleteAccountTypeAsync(accountType, cancellationToken);
    }

    public async Task<Result> AddAccountTypeAsync(AccountTypeDto accountTypeDto, CancellationToken cancellationToken = default)
    {
        var accountType = mapperAccount.MapToDomain(accountTypeDto);
        return await accountRepository.AddAccountTypeAsync(accountType, cancellationToken);
    }

    public async Task<Result> UpdateAccountTypeName(AccountTypeDto accountTypeDto, CancellationToken cancellationToken = default)
    {
        var accountType = mapperAccount.MapToDomain(accountTypeDto);
        return await accountRepository.UpdateAccountTypeName(accountType, cancellationToken);
    }

    public Task<Result> AddCurrencyAsync(CurrencyDto currencyDto, CancellationToken cancellationToken)
    {
        var currencyDomain = mapperAccount.MapToDomain(currencyDto);
        return accountRepository.AddCurrencyAsync(currencyDomain, cancellationToken);
    }

    public Task<Result> UpdateCurrencySymbolAsync(CurrencyDto currencyDto, CancellationToken cancellationToken = default)
    {
        var currencyDomain = mapperAccount.MapToDomain(currencyDto);
        return accountRepository.UpdateCurrencySymbolAsync(currencyDomain, cancellationToken);
    }

    public Task<DeletionResult> DeleteCurrencyAsync(CurrencyDto currencyDto, CancellationToken cancellationToken = default)
    {
        var currencyDomain = mapperAccount.MapToDomain(currencyDto);
        return accountRepository.DeleteCurrencyAsync(currencyDomain, cancellationToken);
    }

    public Task<DeletionResult> DeleteAccountAsync(AccountDto accountDto, CancellationToken cancellationToken = default)
    {
        var accountDomain = mapperAccount.MapToDomain(accountDto);
        return accountRepository.DeleteAccountAsync(accountDomain, cancellationToken);
    }

    public Task<Result> UpdateAccountAsync(AccountDto accountDto, CancellationToken cancellationToken = default)
    {
        var accountDomain = mapperAccount.MapToDomain(accountDto);
        return accountRepository.UpdateAccountAsync(accountDomain, cancellationToken);
    }
}