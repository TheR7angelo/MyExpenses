using Domain.Models.Validation;
using MyExpenses.Application.Interfaces.IServices;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.ViewModels.Accounts;

namespace MyExpenses.Presentation.Services;

public class AccountPresentationService(IAccountService accountService,
    IAccountDtoViewModelMapper viewModelMapper) : IAccountPresentationService
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

    public async Task<Result<AccountTypeViewModel>> AddAccountType(AccountTypeViewModel accountTypeViewModel, CancellationToken cancellationToken = default)
    {
        var accountTypeDto = viewModelMapper.MapToDto(accountTypeViewModel);
        var success = await accountService.CreateAccountTypeAsync(accountTypeDto, cancellationToken);
        return viewModelMapper.MapToViewModel(success);
    }

    public async Task<Result> UpdateAccountTypeName(AccountTypeViewModel accountTypeViewModel, CancellationToken cancellationToken = default)
    {
        var accountTypeDto = viewModelMapper.MapToDto(accountTypeViewModel);
        return await accountService.UpdateAccountTypeName(accountTypeDto, cancellationToken);
    }

    public async Task<Result<CurrencyViewModel>> AddCurrency(CurrencyViewModel newCurrency, CancellationToken cancellationToken = default)
    {
        var currencyDto = viewModelMapper.MapToDto(newCurrency);
        var success = await accountService.CreateCurrencyAsync(currencyDto, cancellationToken);
        return viewModelMapper.MapToViewModel(success);
    }

    public Task<Result> UpdateCurrencySymbol(CurrencyViewModel currencyViewModel, CancellationToken cancellationToken = default)
    {
        var currencyDto = viewModelMapper.MapToDto(currencyViewModel);
        return accountService.UpdateCurrencySymbolAsync(currencyDto, cancellationToken);
    }

    public Task<DeletionResult> DeleteCurrencyAsync(CurrencyViewModel currencyViewModel, CancellationToken cancellationToken = default)
    {
        var currencyDto = viewModelMapper.MapToDto(currencyViewModel);
        return accountService.DeleteCurrencyAsync(currencyDto, cancellationToken);
    }

    public async Task<AccountViewModel?> GetAccount(TotalByAccountViewModel totalByAccountViewModel, CancellationToken cancellationToken = default)
    {
        var accountDto = await accountService.GetAccountAsync(totalByAccountViewModel.Id, cancellationToken);
        return accountDto is null ? null : viewModelMapper.MapToViewModel(accountDto);
    }

    public Task<DeletionResult> DeleteAccountAsync(AccountViewModel accountViewModel, CancellationToken cancellationToken = default)
    {
        var accountDto = viewModelMapper.MapToDto(accountViewModel);
        return accountService.DeleteAccountAsync(accountDto, cancellationToken);
    }

    public Task<Result> UpdateAccount(AccountViewModel accountViewModel, CancellationToken cancellationToken = default)
    {
        var accountDto = viewModelMapper.MapToDto(accountViewModel);
        return accountService.UpdateAccountAsync(accountDto, cancellationToken);
    }

    public async Task<Result<AccountViewModel>> CreateAccount(AccountViewModel accountViewModel, CancellationToken cancellationToken = default)
    {
        var accountDto = viewModelMapper.MapToDto(accountViewModel);
        var success = await accountService.CreateAccount(accountDto, cancellationToken);
        return viewModelMapper.MapToViewModel(success);
    }

    public async Task<TotalByAccountViewModel?> GetTotalByAccountViewModelAsync(AccountViewModel accountViewModel, CancellationToken cancellationToken = default)
    {
        var dto = viewModelMapper.MapToDto(accountViewModel);
        var totalByAccountDto = await accountService.GetTotalByAccountAsync(dto, cancellationToken);
        return totalByAccountDto is null ? null : viewModelMapper.MapToViewModel(totalByAccountDto);
    }
}