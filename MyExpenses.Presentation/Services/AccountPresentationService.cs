using MyExpenses.Application.Interfaces;
using MyExpenses.Application.Interfaces.IServices;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.ViewModels.Accounts;

namespace MyExpenses.Presentation.Services;

public class AccountPresentationService(IAccountServices accountServices, IAccountDtoViewModelMapper viewModelMapper) : IAccountPresentationService
{
    public async Task<IEnumerable<AccountViewModel>> GetAllAccountViewModelAsync(CancellationToken cancellationToken = default)
    {
        var accounts = await accountServices.GetAllAccountAsync(cancellationToken);
        return accounts.Select(viewModelMapper.MapToViewModel);
    }

    public async Task<IEnumerable<TotalByAccountViewModel>> GetAllTotalByAccountViewModelAsync(CancellationToken cancellationToken = default)
    {
        var totalByAccountDto = await accountServices.GetAllTotalByAccountAsync(cancellationToken);
        return totalByAccountDto.Select(viewModelMapper.MapToViewModel);
    }
}