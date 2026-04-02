using Domain.Models.Accounts;
using Domain.Models.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyExpenses.Application.Interfaces.IRepositories;
using MyExpenses.Sql.Context;
using MyExpenses.Sql.Mappings;

namespace MyExpenses.Sql.Repositories;

public class AccountRepository(DataBaseContext dataBaseContext, IDbContextFactory<DataBaseContext> dbContextFactory,
    ILogger<AccountRepository> logger) : IAccountRepository
{
    public async Task<IEnumerable<TotalByAccountDomain>> GetTotalByAccountAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Loading all totals by account");
        var totalByAccounts = await dataBaseContext.VTotalByAccounts
            .AsNoTracking()
            .ProjectToDomain()
            .ToArrayAsync(cancellationToken);

        logger.LogInformation("Loaded {Count} total by account", totalByAccounts.Length);
        return totalByAccounts;
    }

    public async Task<IEnumerable<string>> GetAllAccountNames(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Loading all account names");
        var accountNames = await dataBaseContext.TAccounts
            .AsNoTracking()
            .Select(s => s.Name)
            .ToArrayAsync(cancellationToken);

        logger.LogInformation("Loaded {Count} account names", accountNames.Length);
        return accountNames;
    }

    public async Task<IEnumerable<AccountDomain>> GetAllAccountAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Loading all accounts");
        var accounts = await dataBaseContext.TAccounts
            .AsNoTracking()
            .Include(s => s.CurrencyFkNavigation)
            .Include(s => s.AccountTypeFkNavigation)
            .ProjectToDomain()
            .ToArrayAsync(cancellationToken);

        logger.LogInformation("Loaded {Count} account", accounts.Length);
        return accounts;
    }

    public async Task<IEnumerable<AccountTypeDomain>> GetAllAccountTypeAsync(CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Loading all account types");
        var accountTypes = await context.TAccountTypes
            .AsNoTracking()
            .ProjectToDomain()
            .ToArrayAsync(cancellationToken);

        logger.LogInformation("Loaded {Count} account type", accountTypes.Length);
        return accountTypes;
    }

    public async Task<IEnumerable<CurrencyDomain>> GetAllCurrencyAsync(CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Loading all currencies");
        var currencies = await context.TCurrencies
            .AsNoTracking()
            .ProjectToDomain()
            .ToArrayAsync(cancellationToken);

        logger.LogInformation("Loaded {Count} currency", currencies.Length);
        return currencies;
    }

    public async Task<Result> DeleteAccountTypeAsync(AccountTypeDomain accountType, CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Deleting account type with name {AccountTypeName} and id {AccountTypeId}", accountType.Name, accountType.Id);
        var accountTypeEntity = await
            context.TAccountTypes.FirstOrDefaultAsync(s => s.Id == accountType.Id,
                cancellationToken: cancellationToken);
        if (accountTypeEntity is null)
        {
            logger.LogWarning("Account type with id {AccountTypeId} was not found", accountType.Id);
            return Result.Failure(ErrorCode.AccountTypeNotFound, $"The accountType with id {accountType.Id} was not found");
        }

        try
        {
            logger.LogInformation("Deleting account type with id {AccountTypeId}", accountType.Id);
            context.TAccountTypes.Remove(accountTypeEntity);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Account type with id {AccountTypeId} was successfully deleted", accountType.Id);
            return Result.Success("Account type was successfully deleted");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to delete account type with id {AccountTypeId}", accountType.Id);
            return Result.Failure(ErrorCode.DatabaseError, $"Failed to delete account type: {e.Message}");
        }
    }
}