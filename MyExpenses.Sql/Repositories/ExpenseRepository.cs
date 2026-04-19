using Domain.Models.Accounts;
using Domain.Models.Categories;
using Domain.Models.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyExpenses.Application.Interfaces.IRepositories;
using MyExpenses.SharedUtils;
using MyExpenses.Sql.Context;
using MyExpenses.Sql.Mappings;

namespace MyExpenses.Sql.Repositories;

public class ExpenseRepository(IDbContextFactory<DataBaseContext> dbContextFactory,
    ILogger<ExpenseRepository> logger) : IExpenseRepository
{
    public async Task<int> GetAllExpenseCountAsync(AccountDomain accountDomain, CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Loading all expenses for account with id {AccountId}", accountDomain.Id);
        var expenses = await context.THistories
            .Where(e => e.AccountFk == accountDomain.Id)
            .CountAsync(cancellationToken);

        logger.LogInformation("Loaded {Count} expenses for account with id {AccountId}", expenses, accountDomain.Id);

        return expenses;
    }

    public async Task<int> GetAllExpenseCountAsync(CategoryTypeDomain categoryTypeDomain, CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Loading all expenses for category type with id {CategoryTypeId}", categoryTypeDomain.Id);
        var expenses = await context.THistories
            .Where(e => e.CategoryTypeFk == categoryTypeDomain.Id)
            .CountAsync(cancellationToken);

        logger.LogInformation("Loaded {Count} expenses for category type with id {CategoryTypeId}", expenses, categoryTypeDomain.Id);

        return expenses;
    }

    public async Task<int> GetAllBankTransactionCountAsync(CategoryTypeDomain categoryTypeDomain,
        CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Loading bank transaction count for category type with id {CategoryTypeId}", categoryTypeDomain.Id);

        var bankTransactionCount = await context.THistories
            .AsNoTracking()
            .Where(h => h.CategoryTypeFk == categoryTypeDomain.Id && h.BankTransferFk != null)
            .Select(h => h.BankTransferFk)
            .Distinct()
            .CountAsync(cancellationToken);

        logger.LogInformation("Loaded {Count} bank transaction count for category type with id {CategoryTypeId}", bankTransactionCount, categoryTypeDomain.Id);

        return bankTransactionCount;
    }

    public async Task<int> GetAllRecursiveExpenseCountAsync(CategoryTypeDomain categoryTypeDomain,
        CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Loading recursive expense count for category type with id {CategoryTypeId}", categoryTypeDomain.Id);
        var recursiveExpenseCount = await context.TRecursiveExpenses
            .CountAsync(e => e.CategoryTypeFk == categoryTypeDomain.Id, cancellationToken);

        logger.LogInformation("Loaded {Count} recursive expense count for category type with id {CategoryType}", recursiveExpenseCount, categoryTypeDomain.Id);

        return recursiveExpenseCount;
    }

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

    public async Task<IEnumerable<CategoryTypeDomain>> GetAllCategoryTypesAsync(CancellationToken cancellationToken = default)
    {
        await using var dataBaseContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await dataBaseContext.TCategoryTypes
            .AsNoTracking()
            .Include(s => s.ColorFkNavigation)
            .ProjectToDomain()
            .ToListAsync(cancellationToken);
    }

    public async Task<Result> AddCategoryTypeAsync(CategoryTypeDomain categoryTypeDomain, CancellationToken cancellationToken = default)
    {
        var categoryType = categoryTypeDomain.MapToEntity();

        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Adding category type with name {CategoryTypeName}", categoryType.Name);

        var json = categoryType.ToJson();
        logger.LogInformation("Category type json: {Json}", json);

        try
        {
            context.TCategoryTypes.Add(categoryType);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Category type with name {CategoryTypeName} was successfully added", categoryType.Name);
            return Result.Success("Category type was successfully added");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to add category type with name {CategoryTypeName}", categoryType.Name);
            return Result.Failure(ErrorCode.DatabaseError, "Failed to add category type");
        }
    }
}