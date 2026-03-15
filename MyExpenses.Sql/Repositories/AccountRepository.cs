using Domain.Models.Accounts;
using Microsoft.EntityFrameworkCore;
using MyExpenses.Application.Interfaces.IRepositories;
using MyExpenses.Sql.Context;
using MyExpenses.Sql.Mappings;

namespace MyExpenses.Sql.Repositories;

public class AccountRepository(DataBaseContext dataBaseContext, IDbContextFactory<DataBaseContext> dbContextFactory) : IAccountRepository
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

    public async Task<IEnumerable<AccountTypeDomain>> GetAllAccountTypeAsync(CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var accountTypes = await context.TAccountTypes
            .AsNoTracking()
            .ProjectToDomain()
            .ToListAsync(cancellationToken);

        return accountTypes;
    }

    public async Task<IEnumerable<CurrencyDomain>> GetAllCurrencyAsync(CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var currencies = await context.TCurrencies
            .AsNoTracking()
            .ProjectToDomain()
            .ToListAsync(cancellationToken);

        return currencies;
    }
}