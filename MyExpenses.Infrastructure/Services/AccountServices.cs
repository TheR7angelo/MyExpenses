using AutoMapper;
using MyExpenses.Application.Interfaces;
using MyExpenses.Application.Models.Accounts;
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