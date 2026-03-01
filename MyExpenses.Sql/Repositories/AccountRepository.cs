using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Models.Accounts;
using Microsoft.EntityFrameworkCore;
using MyExpenses.Application.Interfaces;
using MyExpenses.Infrastructure.Repositories;
using MyExpenses.Sql.Context;

namespace MyExpenses.Sql.Repositories;

public class AccountRepository(DataBaseContext dataBaseContext, IMapper mapper) : IAccountRepository
{
    public async Task<IEnumerable<TotalByAccountDomain>> GetTotalByAccountAsync()
    {
        var totalByAccounts = await dataBaseContext.VTotalByAccounts
            .AsNoTracking()
            .ProjectTo<TotalByAccountDomain>(mapper.ConfigurationProvider)
            .ToListAsync();

        return totalByAccounts;
    }
}