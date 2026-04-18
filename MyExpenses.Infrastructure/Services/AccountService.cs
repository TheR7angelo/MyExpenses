using Domain.Models.Validation;
using MyExpenses.Application.Dtos.Accounts;
using MyExpenses.Application.Dtos.Categories;
using MyExpenses.Application.Interfaces.IRepositories;
using MyExpenses.Application.Interfaces.IServices;
using MyExpenses.Application.Interfaces.Mappings;

namespace MyExpenses.Infrastructure.Services;

public class AccountService(IAccountRepository accountRepository, IExpenseRepository expenseRepository, IExpenseRepository categoryRepository,
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

    public async Task<Result> AddCategoryTypeAsync(CategoryTypeDto categoryTypeDto, CancellationToken cancellationToken = default)
    {
        var accountType = mapperAccount.MapToDomain(categoryTypeDto);
        return await categoryRepository.AddCategoryTypeAsync(accountType, cancellationToken);
    }

        // public async Task<AccountDto> AddOrEditAsync(AccountDto accountDto, CancellationToken cancellationToken = default)
    // {
    //
    // }
}