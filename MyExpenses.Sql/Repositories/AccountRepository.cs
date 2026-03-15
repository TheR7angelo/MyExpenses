using Domain.Models.Accounts;
using Microsoft.EntityFrameworkCore;
using MyExpenses.Application.Interfaces.IRepositories;
using MyExpenses.Sql.Context;
using MyExpenses.Sql.Mappings;

namespace MyExpenses.Sql.Repositories;

public class AccountRepository(DataBaseContext dataBaseContext) : IAccountRepository
{
    public async Task<IEnumerable<TotalByAccountDomain>> GetTotalByAccountAsync(CancellationToken cancellationToken = default)
    {
        var totalByAccounts = await dataBaseContext.VTotalByAccounts
            .AsNoTracking()
            .ProjectToDomain()
            .ToListAsync(cancellationToken);

        return totalByAccounts;
    }

    public async Task<IEnumerable<string>> GetAllAccountNames(CancellationToken cancellationToken = default)
    {
        var accountNames = await dataBaseContext.TAccounts
            .AsNoTracking()
            .Select(s => s.Name)
            .ToListAsync(cancellationToken);

        return accountNames;
    }

    public async Task<IEnumerable<AccountDomain>> GetAllAccountAsync(CancellationToken cancellationToken = default)
    {
        var accounts = await dataBaseContext.TAccounts
            .AsNoTracking()
            .Include(s => s.CurrencyFkNavigation)
            .Include(s => s.AccountTypeFkNavigation)
            .ProjectToDomain()
            .ToListAsync(cancellationToken);

        return accounts;
    }
}