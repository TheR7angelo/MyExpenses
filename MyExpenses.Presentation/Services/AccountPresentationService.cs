using Domain.Models.Dependencies;
using Domain.Models.Validation;
using MyExpenses.Application.Interfaces.IServices;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.ViewModels.Accounts;

namespace MyExpenses.Presentation.Services;

public class AccountPresentationService(IAccountService accountService, IAccountDtoViewModelMapper viewModelMapper) : IAccountPresentationService
{
    public async Task<IEnumerable<AccountViewModel>> GetAllAccountViewModelAsync(CancellationToken cancellationToken = default)
    {
        var accounts = await accountService.GetAllAccountAsync(cancellationToken);
        return accounts.Select(viewModelMapper.MapToViewModel);
    }

    public async Task<IEnumerable<TotalByAccountViewModel>> GetAllTotalByAccountViewModelAsync(CancellationToken cancellationToken = default)
    {
        var totalByAccountDto = await accountService.GetAllTotalByAccountAsync(cancellationToken);
        return totalByAccountDto.Select(viewModelMapper.MapToViewModel);
    }

    public async Task<IEnumerable<AccountTypeViewModel>> GetAllAccountTypeViewModelAsync(CancellationToken cancellationToken = default)
    {
        var accountTypes = await accountService.GetAllAccountTypeAsync(cancellationToken);
        return accountTypes.Select(viewModelMapper.MapToViewModel);
    }

    public async Task<IEnumerable<CurrencyViewModel>> GetAllCurrencyViewModelAsync(CancellationToken cancellationToken = default)
    {
        var currencies = await accountService.GetAllCurrencyAsync(cancellationToken);
        return currencies.Select(viewModelMapper.MapToViewModel);
    }

    public async Task<DeletionResult> DeleteAccountTypeAsync(AccountTypeViewModel accountTypeViewModel, CancellationToken cancellationToken = default)
    {
        var accountTypeDto = viewModelMapper.MapToDto(accountTypeViewModel);
        return await accountService.DeleteAccountTypeAsync(accountTypeDto, cancellationToken);
    }

    public async Task<IEnumerable<DeletionDependency>> GetAllDependenciesAsync(AccountTypeViewModel accountTypeViewModel,
        CancellationToken cancellationToken = default)
    {
        var accountTypeDto = viewModelMapper.MapToDto(accountTypeViewModel);
        return await accountService.GetAllDependenciesAsync(accountTypeDto, cancellationToken);
    }

    public async Task<Result> AddAccountType(AccountTypeViewModel newAccountType, CancellationToken cancellationToken = default)
    {
        var accountTypeDto = viewModelMapper.MapToDto(newAccountType);
        return await accountService.AddAccountTypeAsync(accountTypeDto, cancellationToken);
    }

    // public async Task<AccountViewModel> AddOrEditAsync(AccountTypeViewModel accountViewModel, CancellationToken cancellationToken = default)
    // {
    //     var accountType = viewModelMapper.MapToDto(accountViewModel);
    //     return await accountService.AddOrEditAsync(accountType, cancellationToken);
    // }
}