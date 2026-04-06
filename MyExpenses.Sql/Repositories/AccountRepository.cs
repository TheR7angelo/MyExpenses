using Domain.Models.Accounts;
using Domain.Models.Dependencies;
using Domain.Models.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyExpenses.Application.Interfaces.IRepositories;
using MyExpenses.Sql.Context;
using MyExpenses.Sql.Mappings;

namespace MyExpenses.Sql.Repositories;

public class AccountRepository(DataBaseContext dataBaseContext, IDbContextFactory<DataBaseContext> dbContextFactory,
    IExpenseRepository expenseRepository,
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

    public async Task<IEnumerable<AccountDomain>> GetAllAccountAsync(AccountTypeDomain accountType, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Loading all accounts with account type {AccountTypeName}", accountType.Name);

        var accounts = await dataBaseContext.TAccounts
            .AsNoTracking()
            .Include(s => s.AccountTypeFkNavigation)
            .Where(s => s.AccountTypeFk == accountType.Id)
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

    public async Task<DeletionResult> DeleteAccountTypeAsync(AccountTypeDomain accountType, CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Deleting account type with name {AccountTypeName} and id {AccountTypeId}", accountType.Name, accountType.Id);

        var accountTypeEntity = await
            context.TAccountTypes.FirstOrDefaultAsync(s => s.Id == accountType.Id,
                cancellationToken: cancellationToken);

        if (accountTypeEntity is null && accountType.Id is 0)
        {
            logger.LogWarning("Account type with id {AccountTypeId} was not found", accountType.Id);

            return DeletionResult.Success("Account type was not persisted, so nothing had to be deleted");
        }

        if (accountTypeEntity is null) {
            logger.LogWarning("Account type with id {AccountTypeId} was not found", accountType.Id);
            return DeletionResult.Failure(ErrorCode.AccountTypeNotFound, $"The accountType with id {accountType.Id} was not found");
        }

        try
        {
            var accountIds = await GetAllAccountIdAsync(accountType, cancellationToken);

            var expenseIdsTask = expenseRepository.GetAllExpenseIdAsync(accountIds, cancellationToken);
            var bankTransferIdsTask = expenseRepository.GetAllBankTransferIdsAsync(accountIds, cancellationToken);
            var recurringExpenseIdsTask = expenseRepository.GetAllRecurringTransactionIdsAsync(accountIds, cancellationToken);

            await Task.WhenAll(expenseIdsTask, bankTransferIdsTask, recurringExpenseIdsTask);

            var expenseIds = expenseIdsTask.Result;
            var bankTransferIds = bankTransferIdsTask.Result;
            var recurringExpenseIds = recurringExpenseIdsTask.Result;

            if (accountIds.Length > 0) logger.LogWarning("Account type has associated accounts {@AccountIds}", accountIds);
            if (expenseIds.Length > 0) logger.LogWarning("Account type has associated expenses {@ExpenseIds}", expenseIds);
            if (bankTransferIds.Length > 0) logger.LogWarning("Account type has associated bank transfers {@BankTransferIds}", bankTransferIds);
            if (recurringExpenseIds.Length > 0) logger.LogWarning("Account type has associated recurring expenses {@RecurringExpenses}", recurringExpenseIds);

            logger.LogInformation("Deleting account type with id {AccountTypeId}", accountType.Id);
            context.TAccountTypes.Remove(accountTypeEntity);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Account type with id {AccountTypeId} was successfully deleted", accountType.Id);

            var result = new Dictionary<EntityType, int[]>
            {
                { EntityType.Account, accountIds },
                { EntityType.Expense, expenseIds },
                { EntityType.BankTransfer, bankTransferIds },
                { EntityType.RecurringExpense, recurringExpenseIds }
            };
            return DeletionResult.Success("Account type was successfully deleted", result);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to delete account type with id {AccountTypeId}", accountType.Id);
            return DeletionResult.Failure(ErrorCode.DatabaseError, $"Failed to delete account type: {e.Message}");
        }
    }

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

    public async Task<Result> AddAccountTypeAsync(AccountTypeDomain accountTypeDomain, CancellationToken cancellationToken = default)
    {
        var accountType = accountTypeDomain.MapToEntity();

        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Adding account type with name {AccountTypeName}", accountType.Name);

        try
        {
            context.TAccountTypes.Add(accountType);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Account type with name {AccountTypeName} was successfully added", accountType.Name);
            return Result.Success("Account type was successfully added");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to add account type with name {AccountTypeName}", accountType.Name);
            return Result.Failure(ErrorCode.DatabaseError, "Failed to add account type");
        }
    }

    public async Task<Result> UpdateAccountTypeName(AccountTypeDomain accountTypeDomain, CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Updating account type (ID={AccountTypeId}) with name {AccountTypeName}", accountTypeDomain.Id, accountTypeDomain.Name);

        try
        {
            var updatedAccountType = await context.TAccountTypes.FirstOrDefaultAsync(s => s.Id == accountTypeDomain.Id, cancellationToken);
            if (updatedAccountType is null)
            {
                return Result.Failure(ErrorCode.NotFound, "Account type not found");
            }

            updatedAccountType.Name = accountTypeDomain.Name;
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Account type (ID={AccountTypeId}) with name {AccountTypeName} was successfully updated", accountTypeDomain.Id, accountTypeDomain.Name);
            return Result.Success("Account type was successfully updated");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to update account type (ID={AccountTypeId}) with name {AccountTypeName}", accountTypeDomain.Id, accountTypeDomain.Name);
            return Result.Failure(ErrorCode.DatabaseError, "Failed to update account type");
        }
    }

    private async Task<int[]> GetAllAccountIdAsync(AccountTypeDomain accountType,
        CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Loading all accounts with account type {AccountTypeName}", accountType.Name);

        var result = await context.TAccounts.Where(s => s.AccountTypeFk == accountType.Id).Select(s => s.Id).ToArrayAsync(cancellationToken);

        logger.LogInformation("Loaded {Count} account", result.Length);

        return result;
    }
}