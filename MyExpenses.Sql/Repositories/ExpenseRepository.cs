using Domain.Models.Accounts;
using Domain.Models.Categories;
using Domain.Models.Dependencies;
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

    public async Task<int[]> GetAllExpenseIdAsync(CategoryTypeDomain categoryTypeDomain, CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Loading all expenses for category type with id {CategoryType}", categoryTypeDomain.Id);
        var result = await context.THistories.Where(e => e.CategoryTypeFk == categoryTypeDomain.Id).Select(e => e.Id).ToArrayAsync(cancellationToken);

        logger.LogInformation("Loaded {Count} expenses for category type", result.Length);
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

    public async Task<int[]> GetAllBankTransferIdsAsync(CategoryTypeDomain categoryTypeDomain, CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Loading all bank transfers for category type with id {CategoryTypeId}", categoryTypeDomain.Id);

        var result = await context.THistories
            .AsNoTracking()
            .Where(h => h.CategoryTypeFk == categoryTypeDomain.Id && h.BankTransferFk != null)
            .Select(h => (int)h.BankTransferFk!)
            .Distinct()
            .ToArrayAsync(cancellationToken);

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

    public async Task<int[]> GetAllRecurringTransactionIdsAsync(CategoryTypeDomain categoryTypeDomain,
        CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Loading all recurring transactions for category type with id {CategoryTypeId}", categoryTypeDomain.Id);

        var result = await context.TRecursiveExpenses
            .Where(e => e.CategoryTypeFk == categoryTypeDomain.Id)
            .Select(e => e.Id).ToArrayAsync(cancellationToken);

        logger.LogInformation("Loaded {Count} recurring transactions for category type", result.Length);

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

    public async Task<DeletionResult> DeleteCategoryTypeAsync(CategoryTypeDomain categoryType, CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Deleting category type with name {CategoryTypeName} and id {CategoryTypeId}", categoryType.Name, categoryType.Id);

        var categoryTypeEntity = await
            context.TCategoryTypes.FirstOrDefaultAsync(s => s.Id == categoryType.Id,
                cancellationToken: cancellationToken);

        if (categoryTypeEntity is null && categoryType.Id is 0)
        {
            logger.LogWarning("Category type with id {CategoryTypeId} was not found", categoryType.Id);
            return DeletionResult.Success("Category type was not persisted, so nothing had to be deleted");
        }

        if (categoryTypeEntity is null) {
            logger.LogWarning("Category type with id {CategoryTypeId} was not found", categoryType.Id);
            return DeletionResult.Failure(ErrorCode.CategoryTypeNotFound, $"The categoryType with id {categoryType.Id} was not found");
        }

        try
        {
            var expenseIdsTask = GetAllExpenseIdAsync(categoryType, cancellationToken);
            var bankTransferIdsTask = GetAllBankTransferIdsAsync(categoryType, cancellationToken);
            var recurringExpenseIdsTask = GetAllRecurringTransactionIdsAsync(categoryType, cancellationToken);

            await Task.WhenAll(expenseIdsTask, bankTransferIdsTask, recurringExpenseIdsTask);

            var expenseIds = expenseIdsTask.Result;
            var bankTransferIds = bankTransferIdsTask.Result;
            var recurringExpenseIds = recurringExpenseIdsTask.Result;

            if (expenseIds.Length > 0) logger.LogWarning("Category type has associated expenses {@ExpenseIds}", expenseIds);
            if (bankTransferIds.Length > 0) logger.LogWarning("Category type has associated bank transfers {@BankTransferIds}", bankTransferIds);
            if (recurringExpenseIds.Length > 0) logger.LogWarning("Category type has associated recurring expenses {@RecurringExpenses}", recurringExpenseIds);

            logger.LogInformation("Deleting category type with id {CategoryTypeId}", categoryType.Id);
            context.TCategoryTypes.Remove(categoryTypeEntity);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Category type with id {CategoryTypeId} was successfully deleted", categoryType.Id);

            var result = new Dictionary<DependencyType, int[]>
            {
                { DependencyType.Expense, expenseIds },
                { DependencyType.BankTransfer, bankTransferIds },
                { DependencyType.RecurringExpense, recurringExpenseIds }
            };
            return DeletionResult.Success("Category type was successfully deleted", result);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to delete category type with id {CategoryTypeId}", categoryType.Id);
            return DeletionResult.Failure(ErrorCode.DatabaseError, $"Failed to delete category type: {e.Message}");
        }
    }
}