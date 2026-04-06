using Domain.Models.Accounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyExpenses.Application.Interfaces.IRepositories;
using MyExpenses.Sql.Context;

namespace MyExpenses.Sql.Repositories;

public class ExpenseRepository(IDbContextFactory<DataBaseContext> dbContextFactory,
    ILogger<ExpenseRepository> logger) : IExpenseRepository
{
    public async Task<int> GetAllBankTransactionCountAsync(AccountDomain account, CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Loading bank transaction count for account with id {AccountId}", account.Id);
        var bankTransactionCount = await context.TBankTransfers
            .CountAsync(h => h.FromAccountFk == account.Id || h.ToAccountFk == account.Id, cancellationToken);

        logger.LogInformation("Loaded {Count} bank transaction count for account with id {AccountId}", bankTransactionCount, account.Id);

        return bankTransactionCount;
    }

    public async Task<int> GetAllRecursiveExpenseCountAsync(AccountDomain account, CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Loading recursive expense count for account with id {AccountId}", account.Id);
        var recursiveExpenseCount = await context.TRecursiveExpenses
            .CountAsync(e => e.AccountFk == account.Id, cancellationToken);

        logger.LogInformation("Loaded {Count} recursive expense count for account with id {AccountId}", recursiveExpenseCount, account.Id);

        return recursiveExpenseCount;
    }

    public async Task<int[]> GetAllExpenseIdAsync(int[] accountIds, CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Loading all expenses for accounts with ids {@AccountIds}", accountIds);
        var result = await context.THistories.Where(e => ((IEnumerable<int>)accountIds).Contains(e.AccountFk)).Select(e => e.Id).ToArrayAsync(cancellationToken);

        logger.LogInformation("Loaded {Count} expenses for accounts", result.Length);
        return result;
    }

    public async Task<int[]> GetAllBankTransferIdsAsync(int[] accountIds, CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Loading all bank transfers for accounts with ids {@AccountIds}", accountIds);

        var result = await context.TBankTransfers.Where(e => ((IEnumerable<int>)accountIds).Contains(e.FromAccountFk) || ((IEnumerable<int>)accountIds).Contains(e.ToAccountFk)).Select(e => e.Id).ToArrayAsync(cancellationToken);

        logger.LogInformation("Loaded {Count} bank transfers for accounts", result.Length);

        return result;
    }

    public async Task<int[]> GetAllRecurringTransactionIdsAsync(int[] accountIds, CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Loading all recurring transactions for accounts with ids {@AccountIds}", accountIds);

        var result = await context.TRecursiveExpenses.Where(e => ((IEnumerable<int>)accountIds).Contains(e.AccountFk)).Select(e => e.Id).ToArrayAsync(cancellationToken);

        logger.LogInformation("Loaded {Count} recurring transactions for accounts", result.Length);

        return result;
    }
}