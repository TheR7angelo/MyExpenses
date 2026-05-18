using Domain.Models.Dependencies;
using Domain.Models.Validation;
using Microsoft.Extensions.Logging;
using MyExpenses.Application.Dtos.Accounts;
using MyExpenses.Application.Dtos.Expenses;
using MyExpenses.Application.Dtos.Systems;
using MyExpenses.Application.Interfaces.IRepositories;
using MyExpenses.Application.Interfaces.IServices;
using MyExpenses.Application.Interfaces.Mappings;

namespace MyExpenses.Infrastructure.Services;

public class SystemService(IAccountDtoDomainMapper mapperAccount, IExpenseDtoDomainMapper mapperExpense,
    ISystemDtoDomainMapper systemDtoDomainMapper,
    ILogger<SystemService> logger,
    IAccountRepository accountRepository, IExpenseRepository expenseRepository,
    ISystemRepository systemRepository) : ISystemService
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

        dependencies = GroupDependencies(dependencies).ToList();

        logger.LogInformation("Finished dependency loading for account type with {DependencyCount} dependencies", dependencies.Count);
        return dependencies;
    }

    public async Task<IEnumerable<DeletionDependency>> GetAllDependenciesAsync(CategoryTypeDto categoryTypeDto, CancellationToken cancellationToken = default)
    {
        var dependencies = new List<DeletionDependency>();
        var categoryTypeDomain = mapperExpense.MapToDomain(categoryTypeDto);

        using var scope = logger.BeginScope(new Dictionary<string, string>
        {
            ["CategoryTypeName"] = categoryTypeDomain.Name!
        });

        logger.LogInformation("Loading dependencies for category type {CategoryTypeName}", categoryTypeDomain.Name);

        var expenseCountTask = expenseRepository.GetAllExpenseCountAsync(categoryTypeDomain, cancellationToken);
        var bankTransactionCountTask = expenseRepository.GetAllBankTransactionCountAsync(categoryTypeDomain, cancellationToken);
        var recursiveExpenseCountTask = expenseRepository.GetAllRecursiveExpenseCountAsync(categoryTypeDomain, cancellationToken);

        await Task.WhenAll(expenseCountTask, bankTransactionCountTask, recursiveExpenseCountTask);

        var expenseCount = await expenseCountTask;
        var bankTransactionCount = await bankTransactionCountTask;
        var recursiveExpenseCount = await recursiveExpenseCountTask;

        dependencies.Add(new DeletionDependency { Category = DependencyType.Expense, Count = expenseCount });
        dependencies.Add(new DeletionDependency { Category = DependencyType.BankTransfer, Count = bankTransactionCount });
        dependencies.Add(new DeletionDependency { Category = DependencyType.RecurringExpense, Count = recursiveExpenseCount });

        dependencies = GroupDependencies(dependencies).ToList();

        logger.LogInformation("Loaded dependencies for category type {CategoryTypeName}: {ExpenseCount} expenses, {BankTransactionCount} bank transfers, {RecurringExpenseCount} recurring expenses",
        categoryTypeDomain.Name,
        expenseCount,
        bankTransactionCount,
        recursiveExpenseCount);

        logger.LogInformation("Finished dependency loading for category type with {DependencyCount} dependencies", dependencies.Count);
        return dependencies;
    }

    public async Task<IEnumerable<DeletionDependency>> GetAllDependenciesAsync(CurrencyDto currencyDto, CancellationToken cancellationToken = default)
    {
        var dependencies = new List<DeletionDependency>();
        var currencyDomain = mapperAccount.MapToDomain(currencyDto);

        using var scope = logger.BeginScope(new Dictionary<string, string>
        {
            ["CurrencySymbol"] = currencyDomain.Symbol
        });

        logger.LogInformation("Loading dependencies for currency {CurrencySymbol}", currencyDomain.Symbol);

        var accounts = await accountRepository.GetAllAccountAsync(currencyDomain, cancellationToken);
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

        dependencies = GroupDependencies(dependencies).ToList();

        logger.LogInformation("Finished dependency loading for currency with {DependencyCount} dependencies", dependencies.Count);
        return dependencies;
    }

    public async Task<IEnumerable<DeletionDependency>> GetAllDependenciesAsync(AccountDto accountDto, CancellationToken cancellationToken = default)
    {
        var dependencies = new List<DeletionDependency>();
        var account = mapperAccount.MapToDomain(accountDto);

        using var scope = logger.BeginScope(new Dictionary<string, string>
        {
            ["AccountName"] = account.Name
        });

        logger.LogInformation("Starting dependency loading for account {AccountName}", account.Name);

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

        dependencies = GroupDependencies(dependencies).ToList();

        logger.LogInformation("Finished dependency loading for account with {DependencyCount} dependencies", dependencies.Count);
        return dependencies;
    }

    public async Task<Result<IEnumerable<DeletionDependency>>> GetAllDependenciesAsync(ColorDto colorDto, CancellationToken cancellationToken = default)
    {
        var dependencies = new List<DeletionDependency>();
        var colorDomain = systemDtoDomainMapper.MapToDomain(colorDto);

        using var scope = logger.BeginScope(new Dictionary<string, string>
        {
            ["ColorName"] = colorDomain.Name
        });

        logger.LogInformation("Starting dependency loading for color {ColorName}", colorDomain.Name);

        var result = await expenseRepository.GetAllByColorAsync(colorDomain, cancellationToken);

        if (!result.IsSuccess)
        {
            logger.LogError("Error loading dependencies for color {ColorName}: {ErrorMessage}", colorDomain.Name, result.InternalMessage);
            return Result<IEnumerable<DeletionDependency>>.Failure(result.ErrorCode, result.InternalMessage!);
        }

        var enumerable = result.Value!.ToArray();
        dependencies.Add(new DeletionDependency { Category = DependencyType.CategoryType, Count = enumerable.Length });

        logger.LogInformation("Found {CategoryTypeCount} category types", enumerable.Length);

        foreach (var categoryTypeDomain in enumerable)
        {
            logger.LogInformation("Loading dependencies for categoryType {CategoryTypeName}", categoryTypeDomain.Name);

            var expenseCountTask = expenseRepository.GetAllExpenseCountAsync(categoryTypeDomain, cancellationToken);
            var bankTransactionCountTask = expenseRepository.GetAllBankTransactionCountAsync(categoryTypeDomain, cancellationToken);
            var recursiveExpenseCountTask = expenseRepository.GetAllRecursiveExpenseCountAsync(categoryTypeDomain, cancellationToken);

            await Task.WhenAll(expenseCountTask, bankTransactionCountTask, recursiveExpenseCountTask);

            var expenseCount = await expenseCountTask;
            var bankTransactionCount = await bankTransactionCountTask;
            var recursiveExpenseCount = await recursiveExpenseCountTask;

            dependencies.Add(new DeletionDependency { Category = DependencyType.Expense, Count = expenseCount });
            dependencies.Add(new DeletionDependency { Category = DependencyType.BankTransfer, Count = bankTransactionCount });
            dependencies.Add(new DeletionDependency { Category = DependencyType.RecurringExpense, Count = recursiveExpenseCount });

            logger.LogInformation(
                "Loaded dependencies for categoryType {CategoryTypeName}: {ExpenseCount} expenses, {BankTransactionCount} bank transfers, {RecurringExpenseCount} recurring expenses",
                categoryTypeDomain.Name,
                expenseCount,
                bankTransactionCount,
                recursiveExpenseCount);
        }

        dependencies = GroupDependencies(dependencies).ToList();
        logger.LogInformation("Finished dependency loading for account with {DependencyCount} dependencies", dependencies.Count);
        return Result<IEnumerable<DeletionDependency>>.Success(dependencies);
    }

    public async Task<ColorDto> GetRandomColor(CancellationToken cancellationToken)
    {
        var colorDomain = await systemRepository.GetRandomColor(cancellationToken);
        var colorDto = systemDtoDomainMapper.MapToDto(colorDomain);

        return colorDto;
    }

    public async Task<IEnumerable<ColorDto>> GetAllColors(CancellationToken cancellationToken = default)
    {
        var colorDomains = await systemRepository.GetAllColors(cancellationToken);
        var colorDtos = colorDomains.Select(systemDtoDomainMapper.MapToDto);

        return colorDtos;
    }

    public async Task<Result<ColorDto>> CreateColorAsync(ColorDto colorDto, CancellationToken cancellationToken = default)
    {
        var colorDomain = systemDtoDomainMapper.MapToDomain(colorDto);
        var result = await systemRepository.CreateColorAsync(colorDomain, cancellationToken);
        return result.IsSuccess
            ? Result<ColorDto>.Success(systemDtoDomainMapper.MapToDto(result.Value!))
            : Result<ColorDto>.Failure(result.ErrorCode, result.InternalMessage!);
    }

    public async Task<Result<ColorDto>> UpdateColorAsync(ColorDto colorDto, CancellationToken cancellationToken = default)
    {
        var colorDomain = systemDtoDomainMapper.MapToDomain(colorDto);
        var result = await systemRepository.UpdateColorAsync(colorDomain, cancellationToken);
        return result.IsSuccess
            ? Result<ColorDto>.Success(systemDtoDomainMapper.MapToDto(result.Value!))
            : Result<ColorDto>.Failure(result.ErrorCode, result.InternalMessage!);
    }

    public Task<DeletionResult> DeleteColorAsync(ColorDto colorDto, CancellationToken cancellationToken = default)
    {
        var colorDomain = systemDtoDomainMapper.MapToDomain(colorDto);
        return systemRepository.DeleteColorAsync(colorDomain, cancellationToken);
    }

    public Task<bool> IsColorNameAvailableAsync(string name, CancellationToken cancellationToken = default)
    {
        return systemRepository.IsColorNameAvailableAsync(name, cancellationToken);
    }

    public Task<bool> IsColorHexadecimalCodeAvailableAsync(string hexadecimalCode, CancellationToken cancellationToken = default)
    {
        return systemRepository.IsColorHexadecimalCodeAvailableAsync(hexadecimalCode, cancellationToken);
    }

    private IEnumerable<DeletionDependency> GroupDependencies(IEnumerable<DeletionDependency> dependencies)
    {
        return dependencies
            .Where(d => d.Count > 0)
            .GroupBy(d => d.Category)
            .Select(d => new DeletionDependency { Category = d.Key, Count = d.Sum(dd => dd.Count) })
            .OrderBy(d => d.Category);
    }
}