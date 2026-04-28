using CommunityToolkit.Mvvm.Messaging;
using Domain.Models.Accounts;
using Domain.Models.Categories;
using Domain.Models.Dependencies;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyExpenses.Presentation.Enums;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.Messages;
using MyExpenses.Presentation.Resources.Resx.AccountResources;
using MyExpenses.Presentation.Resources.Resx.ExpenseManagementResources;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.Validations;
using MyExpenses.Presentation.ViewModels.Accounts;
using MyExpenses.Presentation.ViewModels.Expenses;
using MessageBoxButton = MyExpenses.Presentation.Enums.MessageBoxButton;
using MessageBoxResult = MyExpenses.Presentation.Enums.MessageBoxResult;

namespace MyExpenses.Wpf.Services;

public class ActionService(
    IDialogService dialogService,
    IAccountPresentationService accountPresentationService,
    ISystemPresentationService systemPresentationService,
    IExpensePresentationService expensePresentationService,
    IAccountDtoViewModelMapper accountDtoViewModelMapper,
    IExpenseDtoViewModelMapper expenseDtoViewModelMapper,
    ILogger<ActionService> logger) : IActionService
{
    public Task ManageCategoryTypeAction(HistoryViewModel historyViewModel, CancellationToken cancellationToken = default)
    {
        return ManageNamedEntityAction(
            currentViewModel: historyViewModel.CategoryTypeViewModel,
            getName: viewModel => viewModel.Name,
            setName: (viewModel, name) => viewModel.Name = name,
            maxNameLength: CategoryTypeDomain.MaxNameLength,
            addTitle: ExpenseManagementResources.TitleWindowAddCategoryTypeName,
            editTitle: ExpenseManagementResources.TitleWindowEditCategoryTypeName,
            addPlaceholder: ExpenseManagementResources.TextBoxAddNewCategoryTypeName,
            editPlaceholder: ExpenseManagementResources.TextBoxEditCategoryTypeName,
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

        var result = await expensePresentationService.AddCategoryType(newCategoryTypeViewModel, cancellationToken);

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
            : dialogService.AskConfirmationOfDependenciesRemoval(
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

    public Task ManageAccountTypeAction(
        AccountViewModel accountViewModel,
        CancellationToken cancellationToken = default)
    {
        return ManageNamedEntityAction(
            currentViewModel: accountViewModel.AccountTypeViewModel,
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
            : dialogService.AskConfirmationOfDependenciesRemoval(DependencyType.AccountType,dependenciesArray);

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
            : dialogService.AskConfirmationOfDependenciesRemoval(DependencyType.Currency,dependenciesArray);

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
            : dialogService.AskConfirmationOfDependenciesRemoval(DependencyType.Account,dependenciesArray);

        if (response is not MessageBoxResult.Yes) return;

        var deleteResult = await accountPresentationService.DeleteAccountAsync(accountViewModel, cancellationToken);

        if (deleteResult.IsSuccess)
        {
            SendDeletedAccountsMessageIfNeeded(deleteResult.DeletedItems);
            SendEntityChangedMessage(DependencyType.Account, DataAction.Delete, accountViewModel.Id);
        }

        ShowDeleteResultMessage(deleteResult.IsSuccess, accountViewModel.Name);
    }

    public async Task<bool> EditAccount(AccountViewModel accountViewModel, CancellationToken cancellationToken = default)
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

    public Task<bool> AddAccount(AccountViewModel accountViewModel, HistoryViewModel historyViewModel,
        CancellationToken cancellationToken = default)
    {
        // // TODO correct
        // // TODO continue here
        //
        // // var error = await CheckIsError();
        // // if (error) return;
        // //
        // // DialogResult = true;
        // // Close();
        //
        // var validator = App.ServiceProvider.GetRequiredService<AccountViewModelValidator>();
        // var valResult = await validator.ValidateAsync(AccountViewModel, CancellationToken.None);
        //
        // AccountViewModel.ValidateWithFluent(valResult);

        // TODO correct
        throw new NotImplementedException();
    }

    public async Task<bool> AddAccount(AccountViewModel accountViewModel, CancellationToken cancellationToken = default)
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

    private async Task ManageNamedEntityAction<TViewModel>(TViewModel? currentViewModel, Func<TViewModel, string?> getName,
        Action<TViewModel, string?> setName,
        int maxNameLength, string addTitle, string editTitle, string addPlaceholder, string editPlaceholder,
        Func<TViewModel> createValidationViewModel,
        Func<TViewModel, TViewModel> cloneValidationViewModel,
        Func<TViewModel, Task> beforeValidationAsync,
        Func<TViewModel, CancellationToken, Task<ValidationResult>> validateAsync,
        Action<ValidationFailure> logValidationError,
        Func<TViewModel, CancellationToken, Task> deleteAsync,
        Func<string, CancellationToken, Task> createAsync,
        Func<TViewModel, string, CancellationToken, Task> updateAsync,
        CancellationToken cancellationToken)
        where TViewModel : class
    {
        while (true)
        {
            var dialogContext = ShowEntityInputDialog(currentViewModel, getName, maxNameLength, addTitle, editTitle,
                addPlaceholder, editPlaceholder);

            if (dialogContext.ShouldCancel) return;

            if (dialogContext.ShouldDelete)
            {
                await deleteAsync(currentViewModel!, cancellationToken);
                return;
            }

            var isValid = await ValidateEntityInputAsync(currentViewModel, dialogContext.Input, setName,
                createValidationViewModel, cloneValidationViewModel, beforeValidationAsync, validateAsync,
                logValidationError, cancellationToken);

            if (!isValid) continue;

            await ExecuteEntitySaveActionAsync(currentViewModel, dialogContext.Input!,
                createAsync, updateAsync, cancellationToken);
            return;
        }
    }

    private EntityInputDialogContext ShowEntityInputDialog<TViewModel>(TViewModel? currentViewModel,
        Func<TViewModel, string?> getName,
        int maxNameLength, string addTitle, string editTitle, string addPlaceholder, string editPlaceholder)
        where TViewModel : class
    {
        var isEdit = currentViewModel is not null;
        var currentName = isEdit ? getName(currentViewModel!) ?? string.Empty : string.Empty;
        var title = isEdit ? editTitle : addTitle;
        var placeholder = isEdit ? editPlaceholder : addPlaceholder;

        var dialogResult = dialogService.ShowInputDialog(title, currentName,
            out var btnResult,
            out var input,
            maxNameLength, placeholder);

        return new EntityInputDialogContext(Input: input,
            ShouldCancel: dialogResult is not true || btnResult is MessageBoxInputResult.Cancel,
            ShouldDelete: btnResult is MessageBoxInputResult.Delete && isEdit);
    }

    private async Task<bool> ValidateEntityInputAsync<TViewModel>(TViewModel? currentViewModel, string? input,
        Action<TViewModel, string?> setName,
        Func<TViewModel> createValidationViewModel,
        Func<TViewModel, TViewModel> cloneValidationViewModel,
        Func<TViewModel, Task> beforeValidationAsync,
        Func<TViewModel, CancellationToken, Task<ValidationResult>> validateAsync,
        Action<ValidationFailure> logValidationError, CancellationToken cancellationToken) where TViewModel : class
    {
        var validationViewModel = CreateValidationViewModel(currentViewModel, createValidationViewModel, cloneValidationViewModel);

        setName(validationViewModel, input);
        await beforeValidationAsync(validationViewModel);
        var validationResult = await validateAsync(validationViewModel, cancellationToken);

        if (validationResult.IsValid) return true;

        ShowValidationError(validationResult, logValidationError);
        return false;
    }

    private static TViewModel CreateValidationViewModel<TViewModel>(TViewModel? currentViewModel,
        Func<TViewModel> createValidationViewModel,
        Func<TViewModel, TViewModel> cloneValidationViewModel) where TViewModel : class
    {
        return currentViewModel is not null
            ? cloneValidationViewModel(currentViewModel)
            : createValidationViewModel();
    }

    private static Task ExecuteEntitySaveActionAsync<TViewModel>(TViewModel? currentViewModel, string input,
        Func<string, CancellationToken, Task> createAsync,
        Func<TViewModel, string, CancellationToken, Task> updateAsync,
        CancellationToken cancellationToken) where TViewModel : class
    {
        return currentViewModel is not null
            ? updateAsync(currentViewModel, input, cancellationToken)
            : createAsync(input, cancellationToken);
    }

    private Task<ValidationResult> ValidateAsync<TValidator, TEntity>(TEntity viewModel, CancellationToken cancellationToken)
        where TValidator : AbstractValidator<TEntity>
    {
        var validator = App.ServiceProvider.GetRequiredService<TValidator>();
        return validator.ValidateAsync(viewModel, cancellationToken);
    }

    private void ShowValidationError(ValidationResult validationResult, Action<ValidationFailure> logValidationError)
    {
        var error = validationResult.Errors.First();

        dialogService.ShowMessageBox(AccountResources.MessageBoxCreateItemErrorCaption, error.ErrorMessage,
            MsgBoxImage.Error);

        logValidationError(error);
    }

    private void LogDomainValidationError(string entityName, ValidationFailure error)
    {
        if (error.CustomState is not DomainValidationFailure domainError) return;

        logger.LogError("Validation failed for {EntityName} with error {ErrorCodeString}: {InternalMessage}",
            entityName, domainError.ErrorCodeString, domainError.InternalMessage);
    }

    private bool AskCreateConfirmation(string name)
    {
        var response = dialogService.ShowMessageBox(AccountResources.MessageBoxCreateItemQuestionCaption,
            string.Format(AccountResources.MessageBoxCreateItemQuestionContent, name),
            MessageBoxButton.YesNo,MsgBoxImage.Question);

        return response is MessageBoxResult.Yes;
    }

    private bool AskUpdateConfirmation(string? oldName, string? newName)
    {
        var response = dialogService.ShowMessageBox(AccountResources.MessageBoxEditItemQuestionCaption,
            string.Format(AccountResources.MessageBoxEditItemQuestionContent, oldName, newName),
            MessageBoxButton.YesNo,MsgBoxImage.Question);

        return response is MessageBoxResult.Yes;
    }

    private bool AskUpdateConfirmation(string?[] oldNames, string?[] newNames)
    {
        if (oldNames.Length != newNames.Length)
        {
            throw new ArgumentException(@"the number of old names must be equal to the number of new names", nameof(oldNames));
        }

        var lines = oldNames.Select((t, i) => $"- \"{t}\" → \"{newNames[i]}\"");
        var changes = string.Join(Environment.NewLine, lines);

        var response = dialogService.ShowMessageBox(AccountResources.MessageBoxEditItemQuestionCaption,
            string.Format(AccountResources.MessageBoxEditItemQuestionContent, Environment.NewLine, changes),
            MessageBoxButton.YesNo,MsgBoxImage.Question);

        return response is MessageBoxResult.Yes;
    }

    private MessageBoxResult AskDeleteConfirmation(string? name)
    {
        return dialogService.ShowMessageBox(AccountResources.MessageBoxDeleteItemQuestionCaption,
            string.Format(AccountResources.MessageBoxDeleteItemQuestionContent, name),
            MessageBoxButton.YesNo, MsgBoxImage.Question);
    }

    private void ShowCreateResultMessage(bool isSuccess, string? name)
    {
        if (isSuccess)
        {
            dialogService.ShowMessageBox(AccountResources.MessageBoxCreateItemSuccessCaption,
                string.Format(AccountResources.MessageBoxCreateItemSuccessContent, name), MsgBoxImage.Check);

            return;
        }

        dialogService.ShowMessageBox(AccountResources.MessageBoxCreateItemErrorCaption,
            string.Format(AccountResources.MessageBoxCreateItemErrorContent, name), MsgBoxImage.Error);
    }

    private void ShowUpdateResultMessage(bool isSuccess)
    {
        if (isSuccess)
        {
            dialogService.ShowMessageBox(AccountResources.MessageBoxEditItemSuccessCaption,
                AccountResources.MessageBoxEditItemSuccessContent, MsgBoxImage.Check);

            return;
        }

        dialogService.ShowMessageBox(AccountResources.MessageBoxEditItemErrorCaption,
            AccountResources.MessageBoxEditItemErrorContent, MsgBoxImage.Error);
    }

    private void ShowDeleteResultMessage(bool isSuccess, string? name)
    {
        if (isSuccess)
        {
            dialogService.ShowMessageBox(AccountResources.MessageBoxDeleteItemSuccessCaption,
                string.Format(AccountResources.MessageBoxDeleteItemSuccessContent, name),
                MsgBoxImage.Check);
            return;
        }

        dialogService.ShowMessageBox(AccountResources.MessageBoxDeletetemErrorCaption,
            AccountResources.MessageBoxDeleteteErrorContent,MsgBoxImage.Error);
    }

    private static void SendDeletedAccountsMessageIfNeeded(Dictionary<DependencyType, int[]>? deletedItems)
    {
        if (deletedItems?.TryGetValue(DependencyType.Account, out var accountIds) is not true) return;

        SendEntityChangedMessage(DependencyType.Account, DataAction.Delete, accountIds);
    }

    private static void SendEntityChangedMessage<T>(DependencyType entityType, DataAction dataAction, T content)
    {
        WeakReferenceMessenger.Default.Send(
            new EntityChangedMessage<T>(
                new EntityChanged<T>
                {
                    EntityType = entityType,
                    DataAction = dataAction,
                    Content = content
                }));
    }

    private readonly record struct EntityInputDialogContext(
        string? Input,
        bool ShouldCancel,
        bool ShouldDelete);
}