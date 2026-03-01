using AutoMapper;
using MyExpenses.Application.Dtos.Accounts;
using MyExpenses.Application.Interfaces;
using MyExpenses.Infrastructure.Repositories;

namespace MyExpenses.Infrastructure.Services;

public class AccountServices(IAccountRepository accountRepository, IMapper mapper) : IAccountServices
{
    public async Task<IEnumerable<TotalByAccountDto>> GetAllTotalByAccountAsync()
    {
        var totalByAccountDomain = await accountRepository.GetTotalByAccountAsync();
        var z = mapper.Map<IEnumerable<TotalByAccountDto>>(totalByAccountDomain);
        return z;
    }
}