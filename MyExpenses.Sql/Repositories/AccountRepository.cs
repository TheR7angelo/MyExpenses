using Domain.Models.Accounts;
using Domain.Models.Dependencies;
using Domain.Models.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyExpenses.Application.Interfaces.IRepositories;
using MyExpenses.SharedUtils;
using MyExpenses.Sql.Context;
using MyExpenses.Sql.Mappings;

namespace MyExpenses.Sql.Repositories;

public class AccountRepository(DataBaseContext dataBaseContext, IDbContextFactory<DataBaseContext> dbContextFactory,
    IExpenseRepository expenseRepository,
    ILogger<AccountRepository> logger) : IAccountRepository
{
    public async Task<IEnumerable<TotalByAccountDomain>> GetAllTotalByAccountAsync(CancellationToken cancellationToken = default)
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

    public async Task<IEnumerable<AccountDomain>> GetAllAccountAsync(CurrencyDomain currencyDomain, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Loading all accounts with currency {CurrencySymbol}", currencyDomain.Symbol);

        var accounts = await dataBaseContext.TAccounts
            .AsNoTracking()
            .Where(s => s.CurrencyFk == currencyDomain.Id)
            .ProjectToDomain()
            .ToArrayAsync(cancellationToken);

        logger.LogInformation("Loaded {Count} account", accounts.Length);

        return accounts;
    }

    public async Task<AccountDomain?> GetAccountAsync(int id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Loading account with id {AccountId}", id);

        var account = await dataBaseContext.TAccounts
            .Include(s => s.AccountTypeFkNavigation)
            .Include(s => s.CurrencyFkNavigation)
            .AsNoTracking()
            .ProjectToDomain()
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        logger.LogInformation("Loaded account");

        return account;
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

            var result = new Dictionary<DependencyType, int[]>
            {
                { DependencyType.Account, accountIds },
                { DependencyType.Expense, expenseIds },
                { DependencyType.BankTransfer, bankTransferIds },
                { DependencyType.RecurringExpense, recurringExpenseIds }
            };
            return DeletionResult.Success("Account type was successfully deleted", result);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to delete account type with id {AccountTypeId}", accountType.Id);
            return DeletionResult.Failure(ErrorCode.DatabaseError, $"Failed to delete account type: {e.Message}");
        }
    }

    public async Task<Result<AccountTypeDomain>> CreateAccountTypeAsync(AccountTypeDomain accountTypeDomain, CancellationToken cancellationToken = default)
    {
        var accountType = accountTypeDomain.MapToEntity();

        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Adding account type with name {AccountTypeName}", accountType.Name);

        var json = accountType.ToJson();
        logger.LogInformation("Account type json: {Json}", json);

        try
        {
            context.TAccountTypes.Add(accountType);
            await context.SaveChangesAsync(cancellationToken);

            accountTypeDomain = accountType.MapToDomain();

            logger.LogInformation("Account type with name {AccountTypeName} was successfully added", accountType.Name);
            return Result<AccountTypeDomain>.Success(accountTypeDomain, "Account type was successfully added");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to add account type with name {AccountTypeName}", accountType.Name);
            return Result<AccountTypeDomain>.Failure(ErrorCode.DatabaseError, "Failed to add account type");
        }
    }

    public async Task<Result> UpdateAccountTypeName(AccountTypeDomain accountTypeDomain, CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Updating account type (ID={AccountTypeId}) with name {AccountTypeName}", accountTypeDomain.Id, accountTypeDomain.Name);

        var json = accountTypeDomain.ToJson();
        logger.LogInformation("Account type json: {Json}", json);

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

    public async Task<Result<CurrencyDomain>> CreateCurrencyAsync(CurrencyDomain currencyDomain, CancellationToken cancellationToken = default)
    {
        var currency = currencyDomain.MapToEntity();

        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Adding currency with symbol {CurrencySymbol}", currency.Symbol);

        var json = currency.ToJson();
        logger.LogInformation("Currency json: {Json}", json);
        logger.Log(LogLevel.Information, "Currency json: {Json}", json);

        try
        {
            context.TCurrencies.Add(currency);
            await context.SaveChangesAsync(cancellationToken);

            currencyDomain = currency.MapToDomain();

            logger.LogInformation("Currency with symbol {CurrencySymbol} was successfully added", currency.Symbol);
            return Result<CurrencyDomain>.Success(currencyDomain, "Currency was successfully added");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to add currency with symbol {CurrencySymbol}", currency.Symbol);
            return Result<CurrencyDomain>.Failure(ErrorCode.DatabaseError, "Failed to add currency");
        }
    }

    public async Task<Result> UpdateCurrencySymbolAsync(CurrencyDomain currencyDomain, CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Updating currency (ID={CurrencyId}) with symbol {CurrencySymbol}", currencyDomain.Id, currencyDomain.Symbol);

        var json = currencyDomain.ToJson();
        logger.LogInformation("Currency json: {Json}", json);

        try
        {
            var updatedCurrency = await context.TCurrencies.FirstOrDefaultAsync(s => s.Id == currencyDomain.Id, cancellationToken);
            if (updatedCurrency is null)
            {
                return Result.Failure(ErrorCode.NotFound, "Currency not found");
            }

            updatedCurrency.Symbol = currencyDomain.Symbol;
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Currency (ID={CurrencyId}) with symbol {CurrencySymbol} was successfully updated", currencyDomain.Id, currencyDomain.Symbol);
            return Result.Success("Currency was successfully updated");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to update currency (ID={CurrencyId}) with symbol {CurrencySymbol}", currencyDomain.Id, currencyDomain.Symbol);
            return Result.Failure(ErrorCode.DatabaseError, "Failed to update currency");
        }
    }

    public async Task<DeletionResult> DeleteCurrencyAsync(CurrencyDomain currencyDomain, CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Deleting currency with symbol {CurrencySymbol} and id {CurrencyId}", currencyDomain.Symbol, currencyDomain.Id);

        var currencyEntity = await
            context.TCurrencies.FirstOrDefaultAsync(s => s.Id == currencyDomain.Id,
                cancellationToken: cancellationToken);

        if (currencyEntity is null && currencyDomain.Id is 0)
        {
            logger.LogWarning("Currency with id {CurrencyId} was not found", currencyDomain.Id);

            return DeletionResult.Success("Currency was not persisted, so nothing had to be deleted");
        }

        if (currencyEntity is null) {
            logger.LogWarning("Currency with id {CurrencyId} was not found", currencyDomain.Id);
            return DeletionResult.Failure(ErrorCode.NotFound, $"The currency with id {currencyDomain.Id} was not found");
        }

        try
        {
            var accountIds = await GetAllAccountIdAsync(currencyDomain, cancellationToken);

            var expenseIdsTask = expenseRepository.GetAllExpenseIdAsync(accountIds, cancellationToken);
            var bankTransferIdsTask = expenseRepository.GetAllBankTransferIdsAsync(accountIds, cancellationToken);
            var recurringExpenseIdsTask = expenseRepository.GetAllRecurringTransactionIdsAsync(accountIds, cancellationToken);

            await Task.WhenAll(expenseIdsTask, bankTransferIdsTask, recurringExpenseIdsTask);

            var expenseIds = expenseIdsTask.Result;
            var bankTransferIds = bankTransferIdsTask.Result;
            var recurringExpenseIds = recurringExpenseIdsTask.Result;

            if (accountIds.Length > 0) logger.LogWarning("Currency has associated accounts {@AccountIds}", accountIds);
            if (expenseIds.Length > 0) logger.LogWarning("Currency has associated expenses {@ExpenseIds}", expenseIds);
            if (bankTransferIds.Length > 0) logger.LogWarning("Currency has associated bank transfers {@BankTransferIds}", bankTransferIds);
            if (recurringExpenseIds.Length > 0) logger.LogWarning("Currency has associated recurring expenses {@RecurringExpenses}", recurringExpenseIds);

            logger.LogInformation("Deleting currency with id {CurrencyId}", currencyDomain.Id);
            context.TCurrencies.Remove(currencyEntity);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Currency with id {CurrencyId} was successfully deleted", currencyDomain.Id);

            var result = new Dictionary<DependencyType, int[]>
            {
                { DependencyType.Account, accountIds },
                { DependencyType.Expense, expenseIds },
                { DependencyType.BankTransfer, bankTransferIds },
                { DependencyType.RecurringExpense, recurringExpenseIds }
            };
            return DeletionResult.Success("Currency was successfully deleted", result);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to delete currency with id {CurrencyId}", currencyDomain.Id);
            return DeletionResult.Failure(ErrorCode.DatabaseError, $"Failed to delete currency: {e.Message}");
        }
    }

    public async Task<DeletionResult> DeleteAccountAsync(AccountDomain accountDomain, CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Deleting account with name {AccountName} and id {AccountId}", accountDomain.Name, accountDomain.Id);

        var accountEntity = await
            context.TAccounts.FirstOrDefaultAsync(s => s.Id == accountDomain.Id,
                cancellationToken: cancellationToken);

        if (accountEntity is null && accountDomain.Id is 0)
        {
            logger.LogWarning("Account with id {AccountId} was not found", accountDomain.Id);

            return DeletionResult.Success("Account was not persisted, so nothing had to be deleted");
        }

        if (accountEntity is null) {
            logger.LogWarning("Account with id {AccountId} was not found", accountDomain.Id);
            return DeletionResult.Failure(ErrorCode.NotFound, $"The account with id {accountDomain.Id} was not found");
        }

        try
        {
            var expenseIdsTask = expenseRepository.GetAllExpenseIdAsync(accountDomain, cancellationToken);
            var bankTransferIdsTask = expenseRepository.GetAllBankTransferIdsAsync(accountDomain, cancellationToken);
            var recurringExpenseIdsTask = expenseRepository.GetAllRecurringTransactionIdsAsync(accountDomain, cancellationToken);

            await Task.WhenAll(expenseIdsTask, bankTransferIdsTask, recurringExpenseIdsTask);

            var expenseIds = expenseIdsTask.Result;
            var bankTransferIds = bankTransferIdsTask.Result;
            var recurringExpenseIds = recurringExpenseIdsTask.Result;

            if (expenseIds.Length > 0) logger.LogWarning("Account has associated expenses {@ExpenseIds}", expenseIds);
            if (bankTransferIds.Length > 0) logger.LogWarning("Account has associated bank transfers {@BankTransferIds}", bankTransferIds);
            if (recurringExpenseIds.Length > 0) logger.LogWarning("Account has associated recurring expenses {@RecurringExpenses}", recurringExpenseIds);

            logger.LogInformation("Deleting account with id {AccountId}", accountDomain.Id);
            context.TAccounts.Remove(accountEntity);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Account with id {AccountId} was successfully deleted", accountDomain.Id);

            var result = new Dictionary<DependencyType, int[]>
            {
                { DependencyType.Account, [accountDomain.Id] },
                { DependencyType.Expense, expenseIds },
                { DependencyType.BankTransfer, bankTransferIds },
                { DependencyType.RecurringExpense, recurringExpenseIds }
            };
            return DeletionResult.Success("Account was successfully deleted", result);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to delete account with id {AccountId}", accountDomain.Id);
            return DeletionResult.Failure(ErrorCode.DatabaseError, $"Failed to delete account: {e.Message}");
        }
    }

    public async Task<Result> UpdateAccountAsync(AccountDomain accountDomain, CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Updating account (ID={AccountId}) with name {AccountName}", accountDomain.Id, accountDomain.Name);

        var json = accountDomain.ToJson();
        logger.LogInformation("Account json: {Json}", json);

        try
        {
            var updatedAccount = await context.TAccounts.FirstOrDefaultAsync(s => s.Id == accountDomain.Id, cancellationToken);
            if (updatedAccount is null)
            {
                return Result.Failure(ErrorCode.NotFound, "Account not found");
            }

            var entity = accountDomain.MapToEntity();
            entity.Merge(updatedAccount);

            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Account (ID={AccountId}) with name {AccountName} was successfully updated", accountDomain.Id, accountDomain.Name);
            return Result.Success("Account was successfully updated");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to update account (ID={AccountId}) with name {AccountName}", accountDomain.Id, accountDomain.Name);
            return Result.Failure(ErrorCode.DatabaseError, "Failed to update account");
        }
    }

    public async Task<Result<AccountDomain>> CreateAccount(AccountDomain accountDomain, CancellationToken cancellationToken = default)
    {
        var account = accountDomain.MapToEntity();

        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Adding account with name {AccountName}", accountDomain.Name);

        var json = account.ToJson();
        logger.LogInformation("Account json: {Json}", json);
        logger.Log(LogLevel.Information, "Account json: {Json}", json);

        try
        {
            context.TAccounts.Add(account);
            await context.SaveChangesAsync(cancellationToken);

            accountDomain = context.TAccounts
                .AsNoTracking()
                .Include(s => s.CurrencyFkNavigation)
                .Include(s => s.AccountTypeFkNavigation)
                .First(s => s.Id == account.Id)
                .MapToDomain();

            logger.LogInformation("Account with name {AccountName} was successfully added", accountDomain.Name);
            return Result<AccountDomain>.Success(accountDomain, "Account was successfully added");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to add account with name {AccountName}", accountDomain.Name);
            return Result<AccountDomain>.Failure(ErrorCode.DatabaseError, "Failed to add account");
        }
    }

    public async Task<TotalByAccountDomain?> GetTotalByAccountAsync(AccountDomain accountDomain, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Loading total by account with name {AccountName} and id {AccountId}", accountDomain.Name, accountDomain.Id);
        var totalByAccount = await dataBaseContext.VTotalByAccounts
            .AsNoTracking()
            .ProjectToDomain()
            .FirstOrDefaultAsync(s => s.Id == accountDomain.Id, cancellationToken);

        if (totalByAccount is null)
        {
            logger.LogWarning("Total by account with id {AccountId} not found", accountDomain.Id);
            return null;
        }

        logger.LogInformation("Loaded total by account with id {AccountId}", totalByAccount.Id);
        return totalByAccount;
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

    private async Task<int[]> GetAllAccountIdAsync(CurrencyDomain currencyDomain, CancellationToken cancellationToken = default)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        logger.LogInformation("Loading all accounts with currency {CurrencyName}", currencyDomain.Symbol);

        var result = await context.TAccounts.Where(s => s.CurrencyFk == currencyDomain.Id).Select(s => s.Id).ToArrayAsync(cancellationToken);

        logger.LogInformation("Loaded {Count} account", result.Length);

        return result;
    }
}