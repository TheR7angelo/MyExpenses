using MyExpenses.Application.Dtos.Accounts;
using MyExpenses.Application.Interfaces;
using MyExpenses.Application.Mappings.Interfaces;
using MyExpenses.Application.ViewModels.Accounts;
using MyExpenses.Infrastructure.Repositories;

namespace MyExpenses.Infrastructure.Services;

public class AccountServices(IAccountRepository accountRepository, IAccountDtoDomainMapper mapper, IAccountDtoViewModelMapper viewModelMapper) : IAccountServices
{
    public async Task<IEnumerable<TotalByAccountDto>> GetAllTotalByAccountAsync(CancellationToken cancellationToken = default)
    {
        var totalByAccountDomain = await accountRepository.GetTotalByAccountAsync(cancellationToken);
        return totalByAccountDomain.Select(mapper.MapToDto);
    }

    public async Task<IEnumerable<TotalByAccountViewModel>> GetAllTotalByAccountViewModelAsync(CancellationToken cancellationToken = default)
    {
        var totalByAccountDto = await GetAllTotalByAccountAsync(cancellationToken);
        return totalByAccountDto.Select(viewModelMapper.MapToViewModel);
    }
}