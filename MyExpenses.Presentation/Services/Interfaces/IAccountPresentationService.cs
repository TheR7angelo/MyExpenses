using MyExpenses.Presentation.ViewModels.Accounts;

namespace MyExpenses.Presentation.Services.Interfaces;

public interface IAccountPresentationService
{
    public Task<IEnumerable<AccountViewModel>> GetAllAccountViewModelAsync(CancellationToken cancellationToken = default);

    public Task<IEnumerable<TotalByAccountViewModel>> GetAllTotalByAccountViewModelAsync(CancellationToken cancellationToken = default);

    public Task<IEnumerable<AccountTypeViewModel>> GetAllAccountTypeViewModelAsync(CancellationToken cancellationToken = default);

    public Task<IEnumerable<CurrencyViewModel>> GetAllCurrencyViewModelAsync(CancellationToken cancellationToken = default);
}