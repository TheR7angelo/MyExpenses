using MyExpenses.Application.Dtos.Accounts;
using MyExpenses.Application.ViewModels.Accounts;

namespace MyExpenses.Application.Interfaces;

public interface IAccountServices
{
    public Task<IEnumerable<TotalByAccountDto>> GetAllTotalByAccountAsync(CancellationToken cancellationToken = default);

    public Task<IEnumerable<TotalByAccountViewModel>> GetAllTotalByAccountViewModelAsync(CancellationToken cancellationToken = default);
}