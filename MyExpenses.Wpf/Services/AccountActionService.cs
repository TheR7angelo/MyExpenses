using Domain.Models.Accounts;
using Domain.Models.Dependencies;
using Domain.Models.Expenses;
using Domain.Models.Validation;
using Microsoft.Extensions.Logging;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.Messages;
using MyExpenses.Presentation.Resources.Resx.AccountResources;
using MyExpenses.Presentation.Resources.Resx.ExpenseResources;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.Validations;
using MyExpenses.Presentation.ViewModels.Accounts;
using MyExpenses.Presentation.ViewModels.Expenses;
using MessageBoxResult = MyExpenses.Presentation.Enums.MessageBoxResult;

namespace MyExpenses.Wpf.Services;

public class AccountActionService(
    IDialogService dialogService,
    IAccountPresentationService accountPresentationService,
    ISystemPresentationService systemPresentationService,
    IExpensePresentationService expensePresentationService,
    IAccountDtoViewModelMapper accountDtoViewModelMapper,
    IExpenseDtoViewModelMapper expenseDtoViewModelMapper,
    ILogger<AccountActionService> logger) : AActionService(dialogService, logger), IAccountActionService
{
    private readonly IDialogService _dialogService = dialogService;

    public Task ManageCategoryTypeAction(HistoryViewModel historyViewModel, CancellationToken cancellationToken = default)
        => ManageCategoryTypeAction(historyViewModel.CategoryTypeViewModel, cancellationToken);

    public Task ManageCategoryTypeAction(CategoryTypeViewModel? categoryTypeViewModel, CancellationToken cancellationToken = default)
    {
        return ManageNamedEntityAction(
            currentViewModel: categoryTypeViewModel,
            getName: viewModel => viewModel.Name,
            setName: (viewModel, name) => viewModel.Name = name,
            maxNameLength: CategoryTypeDomain.MaxNameLength,
            addTitle: ExpenseResources.TitleWindowAddCategoryTypeName,
            editTitle: ExpenseResources.TitleWindowEditCategoryTypeName,
            addPlaceholder: ExpenseResources.TextBoxAddNewCategoryTypeName,
            editPlaceholder: ExpenseResources.TextBoxEditCategoryTypeName,
            createValidationViewModel: () => new CategoryTypeViewModel(),
            cloneValidationViewModel: expenseDtoViewModelMapper.Clone,
            beforeValidationAsync: async viewModel => { viewModel.Color ??= await systemPresentationService.GetRandomColorViewModel(cancellationToken); },
            validateAsync: ValidateAsync<CategoryTypeViewModelValidator, CategoryTypeViewModel>,
            logValidationError: error => LogDomainValidationError("category type", error),
            deleteAsync: DeleteCategoryType,
            createAsync: CreateCategoryType,
            updateAsync: UpdateCategoryType,
            cancellationToken: cancellationToken);
    }

    public async Task CreateCategoryType(string input, CancellationToken cancellationToken = default)
    {
        if (!AskCreateConfirmation(input)) return;

        var newCategoryTypeViewModel = new CategoryTypeViewModel
        {
            Name = input, Color = await systemPresentationService.GetRandomColorViewModel(cancellationToken)
        };

        var result = await expensePresentationService.CreateCategoryType(newCategoryTypeViewModel, cancellationToken);

        if (result.IsSuccess)
        {
            SendEntityChangedMessage(DependencyType.CategoryType, DataAction.Add, newCategoryTypeViewModel);
        }

        ShowCreateResultMessage(result.IsSuccess, newCategoryTypeViewModel.Name);
    }

    public async Task UpdateCategoryType(CategoryTypeViewModel categoryTypeViewModel, string input, CancellationToken cancellationToken = default)
    {
        if (!AskUpdateConfirmation(categoryTypeViewModel.Name, input)) return;

        categoryTypeViewModel.Name = input;

        var result = await expensePresentationService.UpdateCategoryTypeName(categoryTypeViewModel, cancellationToken);

        if (result.IsSuccess)
        {
            SendEntityChangedMessage(DependencyType.CategoryType, DataAction.Update, categoryTypeViewModel);
        }

        ShowUpdateResultMessage(result.IsSuccess);
    }

    public async Task DeleteCategoryType(
        CategoryTypeViewModel categoryTypeViewModel,
        CancellationToken cancellationToken = default)
    {
        var dependencies = await systemPresentationService.GetAllDependenciesAsync(
            categoryTypeViewModel,
            cancellationToken);

        var dependenciesArray = dependencies.ToArray();

        var response = dependenciesArray.Length is 0
            ? AskDeleteConfirmation(categoryTypeViewModel.Name)
            : _dialogService.AskConfirmationOfDependenciesRemoval(
                DependencyType.CategoryType,
                dependenciesArray);

        if (response is not MessageBoxResult.Yes) return;

        var deleteResult = await expensePresentationService.DeleteCategoryTypeAsync(categoryTypeViewModel, cancellationToken);

        if (deleteResult.IsSuccess)
        {
            SendEntityChangedMessage(DependencyType.CategoryType, DataAction.Delete, categoryTypeViewModel.Id);
        }

        ShowDeleteResultMessage(deleteResult.IsSuccess, categoryTypeViewModel.Name);
    }

    public Task ManageAccountTypeAction(AccountViewModel accountViewModel, CancellationToken cancellationToken = default)
        => ManageAccountTypeAction(accountViewModel.AccountTypeViewModel, cancellationToken);

    public Task ManageAccountTypeAction(AccountTypeViewModel? accountTypeViewModel = null, CancellationToken cancellationToken = default)
    {
        return ManageNamedEntityAction(
            currentViewModel: accountTypeViewModel,
            getName: viewModel => viewModel.Name,
            setName: (viewModel, name) => viewModel.Name = name,
            maxNameLength: AccountTypeDomain.MaxNameLength,
            addTitle: AccountResources.TitleWindowAddAccountTypeName,
            editTitle: AccountResources.TitleWindowEditAccountTypeName,
            addPlaceholder: AccountResources.TextBoxAddNewAccountTypeName,
            editPlaceholder: AccountResources.TextBoxEditAccountTypeName,
            createValidationViewModel: () => new AccountTypeViewModel(),
            cloneValidationViewModel: accountDtoViewModelMapper.Clone,
            beforeValidationAsync: _ => Task.CompletedTask,
            validateAsync: ValidateAsync<AccountTypeViewModelValidator, AccountTypeViewModel>,
            logValidationError: error => LogDomainValidationError("account type", error),
            deleteAsync: DeleteAccountType,
            createAsync: CreateAccountType,
            updateAsync: UpdateAccountType,
            cancellationToken: cancellationToken);
    }

    public async Task CreateAccountType(string input, CancellationToken cancellationToken = default)
    {
        if (!AskCreateConfirmation(input)) return;

        var newAccountType = new AccountTypeViewModel { Name = input };
        var result = await accountPresentationService.AddAccountType(newAccountType, cancellationToken);

        if (result.IsSuccess)
        {
            SendEntityChangedMessage(DependencyType.AccountType, DataAction.Add, result.Value);
        }

        ShowCreateResultMessage(result.IsSuccess, newAccountType.Name);
    }

    public async Task UpdateAccountType(AccountTypeViewModel accountTypeViewModel, string input,
        CancellationToken cancellationToken = default)
    {
        if (!AskUpdateConfirmation(accountTypeViewModel.Name, input)) return;

        accountTypeViewModel.Name = input;

        var result = await accountPresentationService.UpdateAccountTypeName(accountTypeViewModel, cancellationToken);

        if (result.IsSuccess)
        {
            SendEntityChangedMessage(DependencyType.AccountType, DataAction.Update, accountTypeViewModel);
        }

        ShowUpdateResultMessage(result.IsSuccess);
    }

    public async Task DeleteAccountType(AccountTypeViewModel accountTypeViewModel, CancellationToken cancellationToken = default)
    {
        var dependencies = await systemPresentationService.GetAllDependenciesAsync(
            accountTypeViewModel, cancellationToken);

        var dependenciesArray = dependencies.ToArray();

        var response = dependenciesArray.Length is 0
            ? AskDeleteConfirmation(accountTypeViewModel.Name)
            : _dialogService.AskConfirmationOfDependenciesRemoval(DependencyType.AccountType,dependenciesArray);

        if (response is not MessageBoxResult.Yes) return;

        var deleteResult = await accountPresentationService.DeleteAccountTypeAsync(accountTypeViewModel, cancellationToken);

        if (deleteResult.IsSuccess)
        {
            SendDeletedAccountsMessageIfNeeded(deleteResult.DeletedItems);
            SendEntityChangedMessage(DependencyType.AccountType, DataAction.Delete, accountTypeViewModel.Id);
        }

        ShowDeleteResultMessage(deleteResult.IsSuccess, accountTypeViewModel.Name);
    }

    public Task ManageCurrencyAction(AccountViewModel accountViewModel, CancellationToken cancellationToken = default)
    {
        return ManageNamedEntityAction(
            currentViewModel: accountViewModel.CurrencyViewModel,
            getName: viewModel => viewModel.Symbol,
            setName: (viewModel, name) => viewModel.Symbol = name,
            maxNameLength: CurrencyDomain.MaxSymbolLength,
            addTitle: AccountResources.TitleWindowAddCurrencySymbol,
            editTitle: AccountResources.TitleWindowEditCurrencySymbol,
            addPlaceholder: AccountResources.TextBoxAddNewCurrencySymbol,
            editPlaceholder: AccountResources.TextBoxEditCurrencySymbol,
            createValidationViewModel: () => new CurrencyViewModel(),
            cloneValidationViewModel: accountDtoViewModelMapper.Clone,
            beforeValidationAsync: _ => Task.CompletedTask,
            validateAsync: ValidateAsync<CurrencyViewModelValidator, CurrencyViewModel>,
            logValidationError: error => LogDomainValidationError("currency", error),
            deleteAsync: DeleteCurrency,
            createAsync: CreateCurrency,
            updateAsync: UpdateCurrency,
            cancellationToken: cancellationToken);
    }

    public async Task CreateCurrency(string input, CancellationToken cancellationToken = default)
    {
        if (!AskCreateConfirmation(input)) return;

        var newCurrency = new CurrencyViewModel { Symbol = input };
        var result = await accountPresentationService.AddCurrency(newCurrency, cancellationToken);

        if (result.IsSuccess)
        {
            SendEntityChangedMessage(DependencyType.Currency, DataAction.Add, result.Value);
        }

        ShowCreateResultMessage(result.IsSuccess, newCurrency.Symbol);
    }

    public async Task UpdateCurrency(CurrencyViewModel currencyViewModel, string input, CancellationToken cancellationToken = default)
    {
        if (!AskUpdateConfirmation(currencyViewModel.Symbol, input)) return;

        currencyViewModel.Symbol = input;

        var result = await accountPresentationService.UpdateCurrencySymbol(currencyViewModel, cancellationToken);

        if (result.IsSuccess)
        {
            SendEntityChangedMessage(DependencyType.Currency, DataAction.Update, currencyViewModel);
        }

        ShowUpdateResultMessage(result.IsSuccess);
    }

    public async Task DeleteCurrency(CurrencyViewModel currencyViewModel, CancellationToken cancellationToken = default)
    {
        var dependencies = await systemPresentationService.GetAllDependenciesAsync(
            currencyViewModel, cancellationToken);

        var dependenciesArray = dependencies.ToArray();

        var response = dependenciesArray.Length is 0
            ? AskDeleteConfirmation(currencyViewModel.Symbol)
            : _dialogService.AskConfirmationOfDependenciesRemoval(DependencyType.Currency,dependenciesArray);

        if (response is not MessageBoxResult.Yes) return;

        var deleteResult = await accountPresentationService.DeleteCurrencyAsync(currencyViewModel, cancellationToken);

        if (deleteResult.IsSuccess)
        {
            SendDeletedAccountsMessageIfNeeded(deleteResult.DeletedItems);
            SendEntityChangedMessage(DependencyType.Currency, DataAction.Delete, currencyViewModel.Id);
        }

        ShowDeleteResultMessage(deleteResult.IsSuccess, currencyViewModel.Symbol);
    }

    public async Task DeleteAccount(AccountViewModel accountViewModel, CancellationToken cancellationToken = default)
    {
        var dependencies = await systemPresentationService.GetAllDependenciesAsync(
            accountViewModel, cancellationToken);

        var dependenciesArray = dependencies.ToArray();

        var response = dependenciesArray.Length is 0
            ? AskDeleteConfirmation(accountViewModel.Name)
            : _dialogService.AskConfirmationOfDependenciesRemoval(DependencyType.Account,dependenciesArray);

        if (response is not MessageBoxResult.Yes) return;

        var deleteResult = await accountPresentationService.DeleteAccountAsync(accountViewModel, cancellationToken);

        if (deleteResult.IsSuccess)
        {
            SendDeletedAccountsMessageIfNeeded(deleteResult.DeletedItems);
            SendEntityChangedMessage(DependencyType.Account, DataAction.Delete, accountViewModel.Id);
        }

        ShowDeleteResultMessage(deleteResult.IsSuccess, accountViewModel.Name);
    }

    public async Task<bool> UpdateAccount(AccountViewModel accountViewModel, CancellationToken cancellationToken = default)
    {
        if (!accountViewModel.IsDirty) return false;

        var valResultAccount = await ValidateAsync<AccountViewModelValidator, AccountViewModel>(accountViewModel, cancellationToken);
        if (valResultAccount.IsValid)
        {
            var pendingChanges = accountViewModel.PendingChanges;
            var oldValues = pendingChanges.Select(s => s.OldValueDisplay).ToArray();
            var newValues = pendingChanges.Select(s => s.NewValueDisplay).ToArray();
            if (!AskUpdateConfirmation(oldValues, newValues)) return false;

            var result = await accountPresentationService.UpdateAccount(accountViewModel, cancellationToken);
            ShowUpdateResultMessage(result.IsSuccess);

            if (!result.IsSuccess) return false;
            SendEntityChangedMessage(DependencyType.Account, DataAction.Update, accountViewModel);
            return true;
        }

        accountViewModel.ValidateWithFluent(valResultAccount);
        return false;
    }

    public async Task<bool> CreateAccount(AccountViewModel accountViewModel, HistoryViewModel historyViewModel,
        CancellationToken cancellationToken = default)
    {
        if (!accountViewModel.IsDirty && !historyViewModel.IsDirty) return false;

        historyViewModel.AccountViewModel = accountViewModel;

        var validateAccountTask = ValidateAsync<AccountViewModelValidator, AccountViewModel>(accountViewModel, cancellationToken);
        var validateHistoryTask = ValidateAsync<HistoryViewModelValidator, HistoryViewModel>(historyViewModel, cancellationToken);

        await Task.WhenAll(validateAccountTask, validateHistoryTask);

        var valResultAccount = validateAccountTask.Result;
        var valResultHistory = validateHistoryTask.Result;
        if (valResultAccount.IsValid && valResultHistory.IsValid)
        {
            Result<HistoryViewModel> expenseResult;

            var accountResult = await accountPresentationService.CreateAccount(accountViewModel, cancellationToken);
            historyViewModel.AccountViewModel = accountResult.Value;
            if (accountResult.IsSuccess) expenseResult = await expensePresentationService.CreateExpense(historyViewModel, cancellationToken);
            else
            {
                ShowCreateResultMessage(accountResult.IsSuccess, accountResult.Value?.Name);
                return false;
            }

            if (!expenseResult.IsSuccess)
            {
                await accountPresentationService.DeleteAccountAsync(accountResult.Value!, cancellationToken);
                ShowCreateResultMessage(expenseResult.IsSuccess, expenseResult.Value?.Description);
                return false;
            }

            SendEntityChangedMessage(DependencyType.Account, DataAction.Add, accountResult.Value);
            SendEntityChangedMessage(DependencyType.Expense, DataAction.Add, expenseResult.Value);
            return true;
        }

        accountViewModel.ValidateWithFluent(valResultAccount);
        historyViewModel.ValidateWithFluent(valResultHistory);
        return false;
    }

    public async Task<bool> CreateAccount(AccountViewModel accountViewModel, CancellationToken cancellationToken = default)
    {
        if (!accountViewModel.IsDirty) return false;

        var valResultAccount = await ValidateAsync<AccountViewModelValidator, AccountViewModel>(accountViewModel, cancellationToken);
        if (valResultAccount.IsValid)
        {
            var result = await accountPresentationService.CreateAccount(accountViewModel, cancellationToken);
            ShowUpdateResultMessage(result.IsSuccess);

            if (!result.IsSuccess) return false;
            SendEntityChangedMessage(DependencyType.Account, DataAction.Add, result.Value);
            return true;
        }

        accountViewModel.ValidateWithFluent(valResultAccount);
        return false;
    }
}