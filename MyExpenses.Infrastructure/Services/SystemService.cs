using Domain.Models.Dependencies;
using Microsoft.Extensions.Logging;
using MyExpenses.Application.Dtos.Accounts;
using MyExpenses.Application.Interfaces.IRepositories;
using MyExpenses.Application.Interfaces.IServices;
using MyExpenses.Application.Interfaces.Mappings;

namespace MyExpenses.Infrastructure.Services;

public class SystemService(IAccountDtoDomainMapper mapperAccount,
    ILogger<SystemService> logger,
    IAccountRepository accountRepository, IExpenseRepository expenseRepository, IExpenseRepository categoryRepository) : ISystemService
{
    public async Task<IEnumerable<DeletionDependency>> GetAllDependenciesAsync(AccountTypeDto accountTypeDto, CancellationToken cancellationToken = default)
    {
        var dependencies = new List<DeletionDependency>();
        var accountType = mapperAccount.MapToDomain(accountTypeDto);

        using var scope = logger.BeginScope(new Dictionary<string, string>
        {
            ["AccountTypeName"] = accountType.Name
        });

        logger.LogInformation("Starting dependency loading for account type {AccountTypeName}", accountType.Name);

        var accounts = await accountRepository.GetAllAccountAsync(accountType, cancellationToken);
        var enumerable = accounts.ToArray();
        dependencies.Add(new DeletionDependency { Category = DependencyType.Account, Count = enumerable.Length });

        logger.LogInformation("Found {AccountCount} accounts", enumerable.Length);

        foreach (var account in enumerable)
        {
            logger.LogInformation("Loading dependencies for account {AccountName}", account.Name);

            var expenseCountTask = expenseRepository.GetAllExpenseCountAsync(account, cancellationToken);
            var bankTransactionCountTask = expenseRepository.GetAllBankTransactionCountAsync(account, cancellationToken);
            var recursiveExpenseCountTask = expenseRepository.GetAllRecursiveExpenseCountAsync(account, cancellationToken);

            await Task.WhenAll(expenseCountTask, bankTransactionCountTask, recursiveExpenseCountTask);

            var expenseCount = await expenseCountTask;
            var bankTransactionCount = await bankTransactionCountTask;
            var recursiveExpenseCount = await recursiveExpenseCountTask;

            dependencies.Add(new DeletionDependency { Category = DependencyType.Expense, Count = expenseCount });
            dependencies.Add(new DeletionDependency { Category = DependencyType.BankTransfer, Count = bankTransactionCount });
            dependencies.Add(new DeletionDependency { Category = DependencyType.RecurringExpense, Count = recursiveExpenseCount });

            logger.LogInformation(
                "Loaded dependencies for account {AccountName}: {ExpenseCount} expenses, {BankTransactionCount} bank transfers, {RecurringExpenseCount} recurring expenses",
                account.Name,
                expenseCount,
                bankTransactionCount,
                recursiveExpenseCount);
        }

        dependencies = dependencies.OrderBy(d => d.Category).ToList();

        logger.LogInformation("Finished dependency loading for account type with {DependencyCount} dependencies", dependencies.Count);
        return dependencies;
    }
}