using Domain.Models;
using Domain.Models.Accounts;
using Domain.Models.Dependencies;
using Domain.Models.Expenses;
using Domain.Models.Systems;
using Domain.Models.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyExpenses.Application.Interfaces.IRepositories;
using MyExpenses.Models.Sql.Bases.Tables;
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

    public async Task<int> GetAllExpenseCountAsync(PlaceDomain placeDomain, CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Loading all expenses for place with id {PlaceId}", placeDomain.Id);
        var expenses = await context.THistories
            .Where(e => e.PlaceFk == placeDomain.Id)
            .CountAsync(cancellationToken);

        logger.LogInformation("Loaded {Count} expenses for place with id {PlaceId}", expenses, placeDomain.Id);

        return expenses;
    }

    public async Task<int> GetAllExpenseCountAsync(ModePaymentDomain modePaymentDomain, CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Loading all expenses for mode payment with id {ModePaymentId}", modePaymentDomain.Id);
        var expenses = await context.THistories
            .Where(e => e.ModePaymentFk == modePaymentDomain.Id)
            .CountAsync(cancellationToken);

        logger.LogInformation("Loaded {Count} expenses for mode payment with id {ModePaymentId}", expenses, modePaymentDomain.Id);

        return expenses;
    }

    public async Task<int> GetAllBankTransactionCountAsync(ModePaymentDomain modePaymentDomain, CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Loading bank transaction count for mode payment with id {ModePaymentId}", modePaymentDomain.Id);

        var bankTransactionCount = await context.THistories
            .AsNoTracking()
            .Where(h => h.ModePaymentFk == modePaymentDomain.Id && h.BankTransferFk != null)
            .Select(h => h.BankTransferFk)
            .Distinct()
            .CountAsync(cancellationToken);

        logger.LogInformation("Loaded {Count} bank transaction count for mode payment with id {ModePaymentId}", bankTransactionCount, modePaymentDomain.Id);

        return bankTransactionCount;
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

    public async Task<int> GetAllBankTransactionCountAsync(PlaceDomain placeDomain, CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Loading bank transaction count for place with id {PlaceId}", placeDomain.Id);

        var bankTransactionCount = await context.THistories
            .AsNoTracking()
            .Where(h => h.PlaceFk == placeDomain.Id && h.BankTransferFk != null)
            .Select(h => h.BankTransferFk)
            .Distinct()
            .CountAsync(cancellationToken);

        logger.LogInformation("Loaded {Count} bank transaction count for place with id {PlaceId}", bankTransactionCount, placeDomain.Id);

        return bankTransactionCount;
    }

    public async Task<Result<int>> GetAllBankTransactionCountAsync(HistoryDomain historyDomain, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

            logger.LogInformation("Loading bank transaction count for expense with id {HistoryId}", historyDomain.Id);

            var bankTransactionCount = await context.THistories
                .AsNoTracking()
                .Where(h => h.Id == historyDomain.Id && h.BankTransferFk != null)
                .Select(h => h.BankTransferFk)
                .Distinct()
                .CountAsync(cancellationToken);

            logger.LogInformation("Loaded {Count} bank transaction count for expense with id {HistoryId}", bankTransactionCount, historyDomain.Id);

            return Result<int>.Success(bankTransactionCount);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while getting bank transaction count");
            return Result<int>.Failure(ErrorCode.DatabaseError, "Error while getting bank transaction count. Please try again later.");
        }
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

    public async Task<int[]> GetAllBankTransferIdsAsync(AccountDomain accountDomain, CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Loading all bank transfers for accounts with ids {AccountIds}", accountDomain.Id);

        var result = await context.TBankTransfers.Where(e => e.FromAccountFk == accountDomain.Id || e.ToAccountFk == accountDomain.Id)
            .Select(e => e.Id).ToArrayAsync(cancellationToken);

        logger.LogInformation("Loaded {Count} bank transfers for account", result.Length);

        return result;
    }

    public async Task<Result<int[]>> GetAllBankTransferIdsAsync(PlaceDomain placeDomain, CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Loading all bank transfers for place with ids {PlaceId}", placeDomain.Id);

        var result = await context.THistories
            .Where(e => e.PlaceFk == placeDomain.Id && e.BankTransferFk != null)
            .Select(e => (int)e.BankTransferFk!)
            .Distinct().ToArrayAsync(cancellationToken);

        logger.LogInformation("Loaded {Count} bank transfers for place", result.Length);

        return Result<int[]>.Success(result);
    }

    public async Task<Result<int[]>> GetAllBankTransferIdsAsync(ModePaymentDomain modePaymentDomain, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

            logger.LogInformation("Loading all bank transfers for mode payment with ids {ModePaymentId}",
                modePaymentDomain.Id);

            var result = await context.THistories
                .Where(e => e.ModePaymentFk == modePaymentDomain.Id && e.BankTransferFk != null)
                .Select(e => (int)e.BankTransferFk!)
                .Distinct().ToArrayAsync(cancellationToken);

            logger.LogInformation("Loaded {Count} bank transfers for mode payment", result.Length);

            return Result<int[]>.Success(result);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while loading all bank transfers for mode payment with id {ModePaymentId}", modePaymentDomain.Id);
            return Result<int[]>.Failure(ErrorCode.DatabaseError, $"Failed to load bank transfer ids: {e.Message}");
        }
    }

    public async Task<int[]> GetAllRecurringTransactionIdsAsync(AccountDomain accountDomain, CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Loading recursive expense count for category type with id {AccountId}", accountDomain.Id);
        var recursiveExpenseCount = await context.TRecursiveExpenses
            .Where(e => e.AccountFk == accountDomain.Id)
            .Select(s => s.Id)
            .ToArrayAsync(cancellationToken);

        logger.LogInformation("Loaded {Count} recursive expense count for account type with id {AccountId}", recursiveExpenseCount, accountDomain.Id);

        return recursiveExpenseCount;
    }

    public async Task<Result<int[]>> GetAllRecurringTransactionIdsAsync(PlaceDomain placeDomain, CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Loading recursive expense count for place with id {PlaceId}", placeDomain.Id);
        var recursiveExpenseCount = await context.TRecursiveExpenses
            .Where(e => e.PlaceFk == placeDomain.Id)
            .Select(s => s.Id)
            .ToArrayAsync(cancellationToken);

        logger.LogInformation("Loaded {Count} recursive expense count for place with id {PlaceId}", recursiveExpenseCount, placeDomain.Id);

        return Result<int[]>.Success(recursiveExpenseCount);
    }

    public async Task<int> GetAllRecursiveExpenseCountAsync(CategoryTypeDomain categoryTypeDomain, CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Loading recursive expense count for category type with id {CategoryTypeId}", categoryTypeDomain.Id);
        var recursiveExpenseCount = await context.TRecursiveExpenses
            .CountAsync(e => e.CategoryTypeFk == categoryTypeDomain.Id, cancellationToken);

        logger.LogInformation("Loaded {Count} recursive expense count for category type with id {CategoryType}", recursiveExpenseCount, categoryTypeDomain.Id);

        return recursiveExpenseCount;
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

    public async Task<int> GetAllRecursiveExpenseCountAsync(PlaceDomain placeDomain, CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Loading recursive expense count for place with id {PlaceId}", placeDomain.Id);
        var recursiveExpenseCount = await context.TRecursiveExpenses
            .CountAsync(e => e.PlaceFk == placeDomain.Id, cancellationToken);

        logger.LogInformation("Loaded {Count} recursive expense count for place with id {PlaceId}", recursiveExpenseCount, placeDomain.Id);

        return recursiveExpenseCount;
    }

    public async Task<int> GetAllRecursiveExpenseCountAsync(ModePaymentDomain modePaymentDomain,
        CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Loading recursive expense count for mode payment with id {ModePaymentId}", modePaymentDomain.Id);
        var recursiveExpenseCount = await context.TRecursiveExpenses
            .CountAsync(e => e.ModePaymentFk == modePaymentDomain.Id, cancellationToken);

        logger.LogInformation("Loaded {Count} recursive expense count for mode payment with id {ModePaymentId}", recursiveExpenseCount, modePaymentDomain.Id);

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

    public async Task<int[]> GetAllExpenseIdAsync(AccountDomain accountDomain, CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Loading all expenses for account with id {AccountId}", accountDomain.Id);
        var result = await context.THistories.Where(e => e.AccountFk == accountDomain.Id).Select(e => e.Id).ToArrayAsync(cancellationToken);

        logger.LogInformation("Loaded {Count} expenses for account", result.Length);
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

    public async Task<Result<int[]>> GetAllExpenseIdAsync(PlaceDomain placeDomain, CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Loading all expenses for place with id {PlaceId}", placeDomain.Id);
        var result = await context.THistories.Where(e => e.PlaceFk == placeDomain.Id).Select(e => e.Id).ToArrayAsync(cancellationToken);

        logger.LogInformation("Loaded {Count} expenses for place", result.Length);
        return Result<int[]>.Success(result);
    }

    public async Task<Result<int[]>> GetAllExpenseIdAsync(ModePaymentDomain modePaymentDomain, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

            logger.LogInformation("Loading all expenses for mode payment with id {ModePaymentId}",
                modePaymentDomain.Id);
            var result = await context.THistories.Where(e => e.ModePaymentFk == modePaymentDomain.Id).Select(e => e.Id)
                .ToArrayAsync(cancellationToken);

            logger.LogInformation("Loaded {Count} expenses for mode payment", result.Length);
            return Result<int[]>.Success(result);
        }
        catch (Exception e)
        {
            return Result<int[]>.Failure(ErrorCode.DatabaseError, $"Failed to load expenses for mode payment: {e.Message}");
        }
    }

    public async Task<Result<int[]>> GetAllExpenseIdAsync(BankTransferDomain bankTransferDomain, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

            logger.LogInformation("Loading all expenses for bank transfer with id {BankTransferId}",
                bankTransferDomain.Id);
            var result = await context.THistories.Where(e => e.BankTransferFk == bankTransferDomain.Id).Select(e => e.Id)
                .ToArrayAsync(cancellationToken);

            logger.LogInformation("Loaded {Count} expenses for bank transfer", result.Length);
            return Result<int[]>.Success(result);
        }
        catch (Exception e)
        {
            return Result<int[]>.Failure(ErrorCode.DatabaseError, $"Failed to load expenses for mode payment: {e.Message}");
        }
    }

    public async Task<int[]> GetAllBankTransferIdsAsync(int[] accountIds, CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Loading all bank transfers for accounts with ids {@AccountIds}", accountIds);

        var result = await context.TBankTransfers.Where(e => ((IEnumerable<int>)accountIds).Contains(e.FromAccountFk) || ((IEnumerable<int>)accountIds)
            .Contains(e.ToAccountFk))
            .Select(e => e.Id)
            .Distinct()
            .ToArrayAsync(cancellationToken);

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

    public async Task<Result<int[]>> GetAllRecurringTransactionIdsAsync(ModePaymentDomain modePaymentDomain, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

            logger.LogInformation("Loading all recurring transactions for mode payment with id {ModePaymentId}",
                modePaymentDomain.Id);

            var result = await context.TRecursiveExpenses
                .Where(e => e.ModePaymentFk == modePaymentDomain.Id)
                .Select(e => e.Id).ToArrayAsync(cancellationToken);

            logger.LogInformation("Loaded {Count} recurring transactions for mode payment", result.Length);

            return Result<int[]>.Success(result);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to load recurring transactions for mode payment with id {ModePaymentId}", modePaymentDomain.Id);
            return Result<int[]>.Failure(ErrorCode.DatabaseError, $"Failed to load recurring transactions for mode payment: {e.Message}");
        }
    }

    public async Task<Result<IEnumerable<CategoryTypeDomain>>> GetAllCategoryTypesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("Loading all category types");

            await using var dataBaseContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

            var results = await dataBaseContext.TCategoryTypes
                .AsNoTracking()
                .Include(s => s.ColorFkNavigation)
                .ProjectToDomain()
                .ToListAsync(cancellationToken);

            logger.LogInformation("Loaded {Count} category types", results.Count);

            return Result<IEnumerable<CategoryTypeDomain>>.Success(results);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to load all category types");
            return Result<IEnumerable<CategoryTypeDomain>>.Failure(ErrorCode.DatabaseError, $"Failed to load all category types: {e.Message}");
        }
    }

    public async Task<Result<CategoryTypeDomain>> CreateCategoryTypeAsync(CategoryTypeDomain categoryTypeDomain, CancellationToken cancellationToken = default)
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

            categoryTypeDomain = context.TCategoryTypes
                .AsNoTracking()
                .Include(s => s.ColorFkNavigation)
                .First(s => s.Id == categoryType.Id)
                .MapToDomain();

            logger.LogInformation("Category type with name {CategoryTypeName} was successfully added", categoryType.Name);
            return Result<CategoryTypeDomain>.Success(categoryTypeDomain, "Category type was successfully added");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to add category type with name {CategoryTypeName}", categoryType.Name);
            return Result<CategoryTypeDomain>.Failure(ErrorCode.DatabaseError, "Failed to add category type");
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

    public async Task<Result> UpdateCategoryTypeNameAsync(CategoryTypeDomain categoryType, CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Updating category type (ID={AccountTypeId}) with name {AccountTypeName}", categoryType.Id, categoryType.Name);

        var json = categoryType.ToJson();
        logger.LogInformation("Category type json: {Json}", json);

        try
        {
            var updatedCategoryType = await context.TCategoryTypes.FirstOrDefaultAsync(s => s.Id == categoryType.Id, cancellationToken);
            if (updatedCategoryType is null)
            {
                return Result.Failure(ErrorCode.NotFound, "Category type not found");
            }

            if (categoryType.Name is null)
            {
                return Result.Failure(ErrorCode.NameRequired, "Category type name cannot be null");
            }

            updatedCategoryType.Name = categoryType.Name;
            // updatedCategoryType.ColorFk = categoryType.Color.MapToEntity().Id;
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Category type (ID={CategoryTypeId}) with name {AccountTypeName} was successfully updated", categoryType.Id, categoryType.Name);
            return Result.Success("Category type was successfully updated");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to update category type (ID={CategoryTypeId}) with name {CategoryTypeName}", categoryType.Id, categoryType.Name);
            return Result.Failure(ErrorCode.DatabaseError, "Failed to update account type");
        }
    }

    public async Task<Result<HistoryDomain>> CreateExpenseAsync(HistoryDomain historyDomain, CancellationToken cancellationToken = default)
    {
        var history = historyDomain.MapToEntity();

        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Adding expense with description {HistoryDescription}", historyDomain.Description);

        var json = historyDomain.ToJson();
        logger.LogInformation("Expense json: {Json}", json);

        try
        {
            context.THistories.Add(history);
            await context.SaveChangesAsync(cancellationToken);

            historyDomain = context.THistories
                .AsNoTracking()
                .Include(s => s.AccountFkNavigation).ThenInclude(s => s.AccountTypeFkNavigation)
                .Include(s => s.AccountFkNavigation).ThenInclude(s => s.CurrencyFkNavigation)
                .Include(s => s.CategoryTypeFkNavigation).ThenInclude(s => s.ColorFkNavigation)
                .Include(s => s.ModePaymentFkNavigation)
                .Include(s => s.PlaceFkNavigation)
                .Include(s => s.RecursiveExpenseFkNavigation).ThenInclude(s => s!.AccountFkNavigation).ThenInclude(s => s!.AccountTypeFkNavigation)
                .Include(s => s.RecursiveExpenseFkNavigation).ThenInclude(s => s!.CategoryTypeFkNavigation).ThenInclude(s => s!.ColorFkNavigation)
                .Include(s => s.RecursiveExpenseFkNavigation).ThenInclude(s => s!.FrequencyFkNavigation)
                .Include(s => s.RecursiveExpenseFkNavigation).ThenInclude(s => s!.ModePaymentFkNavigation)
                .Include(s => s.RecursiveExpenseFkNavigation).ThenInclude(s => s!.PlaceFkNavigation)
                .Include(s => s.BankTransferFkNavigation).ThenInclude(s => s!.FromAccountFkNavigation).ThenInclude(s => s!.AccountTypeFkNavigation)
                .Include(s => s.BankTransferFkNavigation).ThenInclude(s => s!.ToAccountFkNavigation).ThenInclude(s => s!.AccountTypeFkNavigation)
                .First(s => s.Id == history.Id)
                .MapToDomain();

            logger.LogInformation("Expense with description {HistoryDescription} was successfully added", historyDomain.Description);
            return Result<HistoryDomain>.Success(historyDomain, "Expense was successfully added");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to add expense with description {HistoryDescription}", historyDomain.Description);
            return Result<HistoryDomain>.Failure(ErrorCode.DatabaseError, "Failed to add expense");
        }
    }

    public async Task<Result<HistoryDomain>> UpdateExpenseAsync(HistoryDomain domain, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Updating expense with id {HistoryId}", domain.Id);

        try
        {
            if (domain.BankTransfer is not null)
            {
                logger.LogInformation("Bank transfer was provided, updating bank transfer instead of expense");
                return await UpdateBankTransfer(domain, cancellationToken);
            }

            await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

            logger.LogInformation("Updating expense (ID={HistoryId}) with description {HistoryDescription}", domain.Id, domain.Description);

            var json = domain.ToJson();
            logger.LogDebug("Expense: {Json}", json);

            var updatedHistory = await context.THistories.FirstOrDefaultAsync(s => s.Id == domain.Id, cancellationToken);
            if (updatedHistory is null)
            {
                return Result<HistoryDomain>.Failure(ErrorCode.NotFound, "Expense was not found");
            }

            var entity = domain.MapToEntity();
            entity.Merge(updatedHistory);

            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Expense (ID={HistoryId}) with description {HistoryDescription} was successfully updated", domain.Id, domain.Description);
            return Result<HistoryDomain>.Success(domain, "Expense was successfully updated");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to update expense with description {HistoryDescription}", domain.Description);
            return Result<HistoryDomain>.Failure(ErrorCode.DatabaseError, "Failed to update expense");
        }
    }

    public async Task<DeletionResult> DeleteHistoryAsync(HistoryDomain historyDomain, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Deleting expense with id {HistoryId}", historyDomain.Id);

        try
        {
            if (historyDomain.BankTransfer is not null)
            {
                logger.LogInformation("Bank transfer was provided, deleting bank transfer instead of expense");
                return await DeleteBankTransfer(historyDomain, cancellationToken);
            }

            await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

            logger.LogInformation("Deleting expense (ID={HistoryId}) with description {HistoryDescription}", historyDomain.Id, historyDomain.Description);

            var expenseEntity = await context.THistories.FirstOrDefaultAsync(s => s.Id == historyDomain.Id, cancellationToken);

            if (expenseEntity is null)
            {
                logger.LogWarning("Expense with id {HistoryId} not found", historyDomain.Id);
                return DeletionResult.Success($"Expense {historyDomain.Description} was successfully deleted");
            }

            if (expenseEntity.Id is 0)
            {
                logger.LogWarning("Expense with id {HistoryId} was not found", historyDomain.Id);
                return DeletionResult.Success("Expense was not persisted, so nothing had to be deleted");
            }

            context.THistories.Remove(expenseEntity);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Expense (ID={HistoryId}) with description {HistoryDescription} was successfully deleted", historyDomain.Id, historyDomain.Description);

            var result = new Dictionary<DependencyType, int[]>
            {
                { DependencyType.Expense, [historyDomain.Id] }
            };

            return DeletionResult.Success($"Expense {historyDomain.Description} was successfully deleted", result);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to delete expense with id {HistoryId}", historyDomain.Id);
            return DeletionResult.Failure(ErrorCode.DatabaseError, $"Failed to delete expense: {e.Message}");
        }
    }

    public async Task<Result<IEnumerable<ModePaymentDomain>>> GetAllModePaymentAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

            logger.LogInformation("Loading all mode payment");
            var modePayments = await context.TModePayments
                .ProjectToDomain()
                .ToListAsync(cancellationToken);

            logger.LogInformation("Loaded {Count} mode payment", modePayments.Count);

            return Result<IEnumerable<ModePaymentDomain>>.Success(modePayments, "Mode payments loaded successfully");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to load mode payments");
            return Result<IEnumerable<ModePaymentDomain>>.Failure(ErrorCode.DatabaseError, "Failed to load mode payments");
        }
    }

    public async Task<ModePaymentDomain?> GetModePaymentByIdAsync(int modePaymentId, CancellationToken cancellationToken = default)
    {
        await using var dataBaseContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await dataBaseContext.TModePayments
            .AsNoTracking()
            .ProjectToDomain()
            .FirstOrDefaultAsync(s => s.Id == modePaymentId, cancellationToken);
    }

    public async Task<Result<(BankTransferDomain bankTransfer, IEnumerable<HistoryDomain> historiesDomain)>> CreateBankTransferAsync(BankTransferDomain bankTransferDomain, HistoryDomain historyDomain,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Creating bank transfer with raison {BankTransfer}", bankTransferDomain.MainReason);

        var fromHistory = FormateBankTransferHistory(bankTransferDomain.FromAccount, historyDomain, -1);
        var toHistory = FormateBankTransferHistory(bankTransferDomain.ToAccount, historyDomain);

        var bankTransferEntity = bankTransferDomain.MapToEntity();
        bankTransferEntity.THistories = new List<THistory> { fromHistory, toHistory };

        await using var dataBaseContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        dataBaseContext.TBankTransfers.Add(bankTransferEntity);
        await dataBaseContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Bank transfer with raison {BankTransfer} was successfully created", bankTransferDomain.MainReason);

        bankTransferEntity = dataBaseContext.TBankTransfers
            .Include(s => s.FromAccountFkNavigation).ThenInclude(s => s!.AccountTypeFkNavigation)
            .Include(s => s.FromAccountFkNavigation).ThenInclude(s => s!.CurrencyFkNavigation)
            .Include(s => s.ToAccountFkNavigation).ThenInclude(s => s!.AccountTypeFkNavigation)
            .Include(s => s.ToAccountFkNavigation).ThenInclude(s => s!.CurrencyFkNavigation)
            .Include(s => s.THistories!).ThenInclude(s => s.AccountFkNavigation).ThenInclude(s => s.AccountTypeFkNavigation)
            .Include(s => s.THistories!).ThenInclude(s => s.AccountFkNavigation).ThenInclude(s => s.CurrencyFkNavigation)
            .Include(s => s.THistories!).ThenInclude(s => s.CategoryTypeFkNavigation).ThenInclude(s => s.ColorFkNavigation)
            .Include(s => s.THistories!).ThenInclude(s => s.ModePaymentFkNavigation)
            .Include(s => s.THistories!).ThenInclude(s => s.PlaceFkNavigation)
            .AsNoTracking()
            .First(s => s.Id == bankTransferEntity.Id);

        bankTransferDomain = bankTransferEntity.MapToDomain();
        if (bankTransferEntity.THistories is null || bankTransferEntity.THistories.Count is 0 || bankTransferEntity.THistories.Count != 2)
        {
            logger.LogWarning("Failed to create bank transfer because the associated histories were not found for bank transfer with id {BankTransferId}", bankTransferEntity.Id);
            return Result<(BankTransferDomain, IEnumerable<HistoryDomain>)>.Failure(ErrorCode.NotFound, "Failed to create bank transfer because the associated histories were not found");
        }

        var historiesDomain = bankTransferEntity.THistories
            .Select(s => s.MapToDomain())
            .OrderBy(s => s.Value).ToList();

        return Result<(BankTransferDomain, IEnumerable<HistoryDomain>)>.Success((bankTransferDomain, historiesDomain));
    }

    public async Task<Result<HistoryDomain>> UpdateBankTransfer(HistoryDomain domain, CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("Updating bank transfer with id {BankTransferId}", domain.BankTransfer!.Id);

            await using var dataBaseContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

            var bankEntity = await dataBaseContext.TBankTransfers
                .Include(s => s.THistories)
                .FirstOrDefaultAsync(s => s.Id == domain.BankTransfer.Id, cancellationToken: cancellationToken);

            if (bankEntity is null)
            {
                logger.LogError("Failed to find bank transfer with id {BankTransferId}", domain.BankTransfer.Id);
                return Result<HistoryDomain>.Failure(ErrorCode.NotFound, "Failed find corresponding bank transfer for history update");
            }

            logger.LogInformation("Found bank transfer with id {BankTransferId}", bankEntity.Id);

            var entityEdited = bankEntity.THistories!.First(s => s.Id == domain.Id);
            var entityToEdit = bankEntity.THistories!.First(s => s.Id != domain.Id);

            bankEntity.Date = domain.Date;
            bankEntity.MainReason = domain.Description?.Length > 100
                ? domain.Description[..100]
                : domain.Description;

            bankEntity.FromAccountFk = domain.Value < 0 ? entityEdited.AccountFk : entityToEdit.AccountFk;
            bankEntity.ToAccountFk = bankEntity.FromAccountFk == entityEdited.AccountFk ? entityToEdit.AccountFk : entityEdited.AccountFk;
            bankEntity.Value = Math.Abs(entityEdited.Value ?? 0d);

            var history = domain.MapToEntity();
            history.Id = entityEdited.Id;
            history.AccountFk = entityEdited.AccountFk;
            history.Value = bankEntity.FromAccountFk == entityEdited.AccountFk ? -1 * Math.Abs(domain.Value) : domain.Value;
            history.Merge(entityEdited);

            var historyDestination = domain.MapToEntity();
            historyDestination.Id = entityToEdit.Id;
            historyDestination.AccountFk = entityToEdit.AccountFk;
            historyDestination.Value = -entityEdited.Value;
            historyDestination.Merge(entityToEdit);

            await dataBaseContext.SaveChangesAsync(cancellationToken);

            domain = await dataBaseContext.THistories
                .Include(s => s.AccountFkNavigation).ThenInclude(s => s.AccountTypeFkNavigation)
                .Include(s => s.AccountFkNavigation).ThenInclude(s => s.CurrencyFkNavigation)
                .Include(s => s.CategoryTypeFkNavigation).ThenInclude(s => s.ColorFkNavigation)
                .Include(s => s.ModePaymentFkNavigation)
                .Include(s => s.BankTransferFkNavigation).ThenInclude(s => s!.FromAccountFkNavigation).ThenInclude(s => s!.AccountTypeFkNavigation)
                .Include(s => s.BankTransferFkNavigation).ThenInclude(s => s!.ToAccountFkNavigation).ThenInclude(s => s!.AccountTypeFkNavigation)
                .Include(s => s.RecursiveExpenseFkNavigation).ThenInclude(s => s!.CategoryTypeFkNavigation).ThenInclude(s => s!.ColorFkNavigation)
                .Include(s => s.RecursiveExpenseFkNavigation).ThenInclude(s => s!.AccountFkNavigation).ThenInclude(s => s!.CurrencyFkNavigation)
                .Include(s => s.PlaceFkNavigation)
                .ProjectToDomain()
                .FirstAsync(s => s.Id == domain.Id, cancellationToken: cancellationToken);

            return Result<HistoryDomain>.Success(domain, "Bank transfer successfully updated with expense data");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to update bank transfer with expense {HistoryId}", domain.Id);
            return Result<HistoryDomain>.Failure(ErrorCode.DatabaseError, "Failed to update bank transfer");
        }
    }

    public async Task<DeletionResult> DeleteBankTransfer(HistoryDomain historyDomain, CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var bankTransferDomain = historyDomain.BankTransfer!;
        logger.LogInformation("Deleting bank transfer with main raison {BankTransferMainReason} and id {BankTransferId}", bankTransferDomain.MainReason, bankTransferDomain.Id);

        var bankTransferEntity = await
            context.TBankTransfers.FirstOrDefaultAsync(s => s.Id == bankTransferDomain.Id, cancellationToken);

        if (bankTransferEntity is null)
        {
            logger.LogWarning("Bank transfer with id {BankTransferId} was not found", bankTransferDomain.Id);
            return DeletionResult.Failure(ErrorCode.NotFound, $"The bank transfer with id {bankTransferDomain.Id} was not found");
        }

        if (bankTransferEntity.Id is 0)
        {
            logger.LogWarning("Bank transfer with id {BankTransferId} was not found", bankTransferDomain.Id);
            return DeletionResult.Success("Bank transfer was not persisted, so nothing had to be deleted");
        }

        try
        {
            var expenseIds = await GetAllExpenseIdAsync(bankTransferDomain, cancellationToken);

            if (expenseIds is { IsSuccess: true, Value.Length: > 0 }) logger.LogWarning("Bank transfer has associated expenses {@ExpenseIds}", expenseIds.Value);

            logger.LogInformation("Deleting bank transfer with id {BankTransferId}", bankTransferDomain.Id);
            context.TBankTransfers.Remove(bankTransferEntity);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Bank transfer with id {BankTransferId} was successfully deleted", bankTransferDomain.Id);

            var result = new Dictionary<DependencyType, int[]>
            {
                { DependencyType.BankTransfer, [ bankTransferEntity.Id ] },
                { DependencyType.Expense, expenseIds.Value ?? [] },
            };
            return DeletionResult.Success("Bank transfer was successfully deleted", result);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to delete bank transfer with id {BankTransferId}", bankTransferDomain.Id);
            return DeletionResult.Failure(ErrorCode.DatabaseError, $"Failed to delete bank transfer: {e.Message}");
        }
    }



    public async Task<Result<IEnumerable<CategoryTypeDomain>>> GetAllByColorAsync(ColorDomain colorDomain, CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("Loading all category types with color {ColorName}", colorDomain.Name);

            await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

            var categoryTypes = await context.TCategoryTypes
                .Include(s => s.ColorFkNavigation)
                .Where(ct => ct.ColorFk == colorDomain.Id)
                .Select(ct => ct.MapToDomain())
                .ToListAsync(cancellationToken);

            logger.LogInformation("Loaded {Count} category types with color {ColorName}", categoryTypes.Count, colorDomain.Name);

            return Result<IEnumerable<CategoryTypeDomain>>.Success(categoryTypes);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to load category types with color {ColorName}", colorDomain.Name);
            return Result<IEnumerable<CategoryTypeDomain>>.Failure(ErrorCode.DatabaseError, "Failed to load category types by color");
        }
    }

    public async Task<Result<int[]>> GetAllExpenseIdAsync(CategoryTypeDomain[] categoryTypeDomain, CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("Loading all expenses for category types {@CategoryTypes}", categoryTypeDomain);

            await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

            var expenseIds = await context.THistories
                .Where(e => categoryTypeDomain.Select(s => s.Id).Contains((int)e.CategoryTypeFk!))
                .Select(e => e.Id)
                .ToListAsync(cancellationToken);

            logger.LogInformation("Loaded {Count} expense IDs for category types", expenseIds.Count);

            return Result<int[]>.Success(expenseIds.ToArray());
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to load expenses for category types {@CategoryTypes}", categoryTypeDomain);
            return Result<int[]>.Failure(ErrorCode.DatabaseError, "Failed to load expenses for category types");
        }
    }

    public async Task<Result<int[]>> GetAllBankTransferIdsAsync(CategoryTypeDomain[] categoryTypeDomain, CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("Loading all bank transfers for category types {@CategoryTypes}", categoryTypeDomain);

            await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

            var result = await context.THistories.Where(e => e.BankTransferFk != null && categoryTypeDomain.Select(s => s.Id).Contains((int)e.CategoryTypeFk!))
                .Select(e => e.Id)
                .Distinct()
                .ToArrayAsync(cancellationToken);

            logger.LogInformation("Loaded {Count} bank transfer IDs for category types", result.Length);

            return Result<int[]>.Success(result);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to load bank transfers for category types {@CategoryTypes}", categoryTypeDomain);
            return Result<int[]>.Failure(ErrorCode.DatabaseError, "Failed to load bank transfers for category types");
        }
    }

    public async Task<Result<int[]>> GetAllRecurringTransactionIdsAsync(CategoryTypeDomain[] categoryTypeDomain,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Loading all recurring expense for category types {@CategoryTypes}", categoryTypeDomain);

        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var result = await context.TRecursiveExpenses.Where(e => categoryTypeDomain.Select(s => s.Id).Contains((int)e.CategoryTypeFk!))
            .Select(e => e.Id)
            .Distinct()
            .ToArrayAsync(cancellationToken);

        logger.LogInformation("Loaded {Count} recurring expense IDs for category types", result.Length);

        return Result<int[]>.Success(result);
    }

    public async Task<Result<ModePaymentDomain>> CreateModePaymentAsync(ModePaymentDomain modePaymentDomain, CancellationToken cancellationToken = default)
    {
        var modePayment = modePaymentDomain.MapToEntity();

        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Adding mode payment with name {ModePaymentName}", modePayment.Name);

        var json = modePayment.ToJson();
        logger.LogInformation("Mode payment json: {Json}", json);
        logger.Log(LogLevel.Information, "Mode payment json: {Json}", json);

        try
        {
            context.TModePayments.Add(modePayment);
            await context.SaveChangesAsync(cancellationToken);

            modePaymentDomain = modePayment.MapToDomain();

            logger.LogInformation("Mode payment with name {ModePaymentName} was successfully added", modePayment.Name);
            return Result<ModePaymentDomain>.Success(modePaymentDomain, "Mode payment was successfully added");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to add mode payment with name {ModePaymentName}", modePayment.Name);
            return Result<ModePaymentDomain>.Failure(ErrorCode.DatabaseError, "Failed to add mode payment");
        }
    }

    public async Task<Result<ModePaymentDomain>> UpdateModePaymentAsync(ModePaymentDomain modePaymentDomain, CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Updating mode payment (ID={ModePaymentId}) with name {ModePaymentName}", modePaymentDomain.Id, modePaymentDomain.Name);

        var json = modePaymentDomain.ToJson();
        logger.LogInformation("Mode payment json: {Json}", json);

        try
        {
            var modePayment = await context.TModePayments.FirstOrDefaultAsync(s => s.Id == modePaymentDomain.Id, cancellationToken);
            if (modePayment is null)
            {
                return Result<ModePaymentDomain>.Failure(ErrorCode.NotFound, "Mode payment not found");
            }

            modePayment.Name = modePaymentDomain.Name;
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Mode payment (ID={ModePaymentId}) with name {ModePaymentName} was successfully updated", modePaymentDomain.Id, modePaymentDomain.Name);
            return Result<ModePaymentDomain>.Success(modePaymentDomain, "Mode payment was successfully updated");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to update mode payment (ID={ModePaymentId}) with name {ModePaymentName}", modePaymentDomain.Id, modePaymentDomain.Name);
            return Result<ModePaymentDomain>.Failure(ErrorCode.DatabaseError, "Failed to update mode payment");
        }
    }

    public async Task<DeletionResult> DeleteModePaymentAsync(ModePaymentDomain modePaymentDomain, CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Deleting mode payment with name {ModePaymentName} and id {ModePaymentId}", modePaymentDomain.Name, modePaymentDomain.Id);

        var modePaymentEntity = await
            context.TModePayments.FirstOrDefaultAsync(s => s.Id == modePaymentDomain.Id,
                cancellationToken: cancellationToken);

        if (modePaymentEntity is null && modePaymentDomain.Id is 0)
        {
            logger.LogWarning("Mode payment with id {ModePaymentId} was not found", modePaymentDomain.Id);
            return DeletionResult.Success("Mode payment was not persisted, so nothing had to be deleted");
        }

        if (modePaymentEntity is null) {
            logger.LogWarning("Mode payment with id {ModePaymentId} was not found", modePaymentDomain.Id);
            return DeletionResult.Failure(ErrorCode.NotFound, $"The mode payment with id {modePaymentDomain.Id} was not found");
        }

        try
        {
            var expenseIdsTask = GetAllExpenseIdAsync(modePaymentDomain, cancellationToken);
            var bankTransferIdsTask = GetAllBankTransferIdsAsync(modePaymentDomain, cancellationToken);
            var recurringExpenseIdsTask = GetAllRecurringTransactionIdsAsync(modePaymentDomain, cancellationToken);

            await Task.WhenAll(expenseIdsTask, bankTransferIdsTask, recurringExpenseIdsTask);

            var expenseIds = expenseIdsTask.Result;
            var bankTransferIds = bankTransferIdsTask.Result;
            var recurringExpenseIds = recurringExpenseIdsTask.Result;

            if (!expenseIds.IsSuccess || !bankTransferIds.IsSuccess || !recurringExpenseIds.IsSuccess)
            {
                return DeletionResult.Failure(ErrorCode.DatabaseError, "Failed to load dependencies for deletion");
            }

            if (expenseIds.Value!.Length > 0) logger.LogWarning("Category type has associated expenses {@ExpenseIds}", expenseIds);
            if (bankTransferIds.Value!.Length > 0) logger.LogWarning("Category type has associated bank transfers {@BankTransferIds}", bankTransferIds);
            if (recurringExpenseIds.Value!.Length > 0) logger.LogWarning("Category type has associated recurring expenses {@RecurringExpenses}", recurringExpenseIds);

            logger.LogInformation("Deleting mode payment with id {ModePaymentId}", modePaymentDomain.Id);
            context.TModePayments.Remove(modePaymentEntity);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Mode payment with id {ModePaymentId} was successfully deleted", modePaymentDomain.Id);

            var result = new Dictionary<DependencyType, int[]>
            {
                { DependencyType.ModePayment, [modePaymentDomain.Id] },
                { DependencyType.Expense, expenseIds.Value! },
                { DependencyType.BankTransfer, bankTransferIds.Value! },
                { DependencyType.RecurringExpense, recurringExpenseIds.Value! }
            };
            return DeletionResult.Success("Mode payment was successfully deleted", result);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to delete mode payment with id {ModePaymentId}", modePaymentDomain.Id);
            return DeletionResult.Failure(ErrorCode.DatabaseError, $"Failed to delete mode payment: {e.Message}");
        }
    }

    public async Task<Result<IEnumerable<RecursiveExpenseDomain>>> GetAllActiveRecurrences(int year, int month, CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("Loading all recurring expense with year={Year} and month={Month}", year, month);

            await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

            var result = await context.TRecursiveExpenses
                    .Where(s => !s.ForceDeactivate)
                    .Where(s => s.IsActive)
                    .Where(s => s.NextDueDate.Year <= year && s.NextDueDate.Month <= month)
                    .ProjectToDomain()
                    .ToArrayAsync(cancellationToken);

            logger.LogInformation("Loaded {Count} recurring expense", result.Length);

            return Result<IEnumerable<RecursiveExpenseDomain>>.Success(result);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to load recurring expense");
            return Result<IEnumerable<RecursiveExpenseDomain>>.Failure(ErrorCode.DatabaseError, "Failed to load recurring expense");
        }
    }

    public async Task<Result<IEnumerable<int>>> GetAllExpenseYear(SortOrder sortOrder,
        CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("Year-round loading available for all existing expenses");

            await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

            var query = context.THistories
                .Where(s => s.Date.HasValue)
                .Select(s => s.Date!.Value.Year)
                .Distinct();

            query = sortOrder switch
            {
                SortOrder.Ascending => query.OrderBy(x => x),
                SortOrder.Descending => query.OrderByDescending(x => x),
                _ => query
            };

            var result = await query.ToArrayAsync(cancellationToken);

            logger.LogInformation("Loaded {Count} year", result.Length);

            return Result<IEnumerable<int>>.Success(result);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to load year");
            return Result<IEnumerable<int>>.Failure(ErrorCode.DatabaseError, "Failed to load year");
        }
    }

    private static THistory FormateBankTransferHistory(AccountDomain accountDomain, HistoryDomain historyDomain, int multiplier = 1)
    {
        return new THistory
        {
            AccountFk = accountDomain.Id,
            Description = historyDomain.Description,
            CategoryTypeFk = historyDomain.CategoryType.Id,
            ModePaymentFk = historyDomain.ModePayment.Id,
            Value = multiplier * historyDomain.Value,
            Date = historyDomain.Date,
            PlaceFk = historyDomain.Place.Id,
            IsPointed = historyDomain.IsPointed,
            DateAdded = historyDomain.DateAdded,
            DatePointed = historyDomain.DatePointed
        };
    }
}