using Domain.Models.Accounts;
using Microsoft.EntityFrameworkCore;
using MyExpenses.Infrastructure.Repositories;
using MyExpenses.Sql.Context;
using MyExpenses.Sql.Mappings;

namespace MyExpenses.Sql.Repositories;

public class AccountRepository(DataBaseContext dataBaseContext) : IAccountRepository
{
    public async Task<IEnumerable<TotalByAccountDomain>> GetTotalByAccountAsync(CancellationToken cancellationToken = default)
    {
        var totalByAccounts = await dataBaseContext.VTotalByAccounts
            .AsNoTracking()
            .ProjectToDto()
            .ToListAsync(cancellationToken);

        return totalByAccounts;
    }
}