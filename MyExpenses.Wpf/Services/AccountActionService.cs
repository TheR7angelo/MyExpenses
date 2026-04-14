using CommunityToolkit.Mvvm.Messaging;
using Domain.Models.Accounts;
using Domain.Models.Dependencies;
using MyExpenses.Presentation.Enums;
using MyExpenses.Presentation.Messages;
using MyExpenses.Presentation.Resources.Resx.AccountResources;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.Validations.Interfaces;
using MyExpenses.Presentation.ViewModels.Accounts;
using MyExpenses.Presentation.ViewModels.Categories;
using MyExpenses.Presentation.ViewModels.Expenses;
using MyExpenses.Presentation.ViewModels.Systems;
using MyExpenses.SharedUtils.Resources.Resx.AccountTypeManagement;

namespace MyExpenses.Wpf.Services;

public class AccountActionService(
    IDialogService dialogService,
    IAccountPresentationService accountPresentationService,
    IAccountPresentationValidationService validationService) : IAccountActionService
{
    public async Task ManageCategoryTypeAction(HistoryViewModel historyViewModel, CancellationToken cancellationToken = default)
    {
        var editMode = historyViewModel.CategoryTypeViewModel != null;
        var defaultText = editMode ? historyViewModel.CategoryTypeViewModel!.Name ?? string.Empty : string.Empty;

        // TODO change
        var placeholder = editMode ? AccountTypeManagementResources.TextBoxEditAccountTypeName
            : AccountTypeManagementResources.TextBoxAddNewAccountTypeName;

        // TODO change
        var result = dialogService.ShowInputDialog(AccountTypeManagementResources.TitleWindow, defaultText,
            out var messageBoxResult, out var input, AccountTypeDomain.MaxNameLength, placeholder);

        if (result is not true || string.IsNullOrWhiteSpace(input)) return;

        switch (messageBoxResult, editMode)
        {
            // case (MessageBoxInputResult.Delete, _):
                // await DeleteAccountType(accountViewModel.AccountType!, cancellationToken);
                // break;

            case (MessageBoxInputResult.Valid, false):
                await CreateCategoryType(input, cancellationToken);
                break;

            // case (MessageBoxInputResult.Valid, true):
                // await UpdateAccountType(accountViewModel.AccountType!, input, cancellationToken);
                // break;
        }
    }

    public async Task CreateCategoryType(string input, CancellationToken cancellationToken = default)
    {
        var response = dialogService.ShowMessageBox(AccountResources.MessageBoxCreateItemQuestionCaption,
            string.Format(AccountResources.MessageBoxCreateItemQuestionContent, input),
            MessageBoxButton.YesNo, MsgBoxImage.Question);

        if (response is not MessageBoxResult.Yes) return;

        var available = await validationService.IsCategoryTypeNameAvailableAsync(input, cancellationToken);
        if (available)
        {
            var randomColor = await accountPresentationService.GetRandomColorViewModel(cancellationToken);

            var newCategoryType = new CategoryTypeViewModel { Name = input, Color = randomColor };
            var result = await accountPresentationService.AddCategoryType(newCategoryType, cancellationToken);

            if (!result.IsSuccess)
            {
                dialogService.ShowMessageBox(AccountResources.MessageBoxCreateItemErrorCaption,
                    string.Format(AccountResources.MessageBoxCreateItemErrorContent, newCategoryType.Name),
                    MsgBoxImage.Error);
            }
            return;
        }

        dialogService.ShowMessageBox(AccountResources.MessageBoxCreateItemErrorCaption,
            string.Format(AccountResources.MessageBoxCreateItemErrorAlreadyUsedContent, input),
            MsgBoxImage.Error);
    }

    public Task UpdateCategoryType(CategoryTypeViewModel categoryTypeViewModel, string input,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteCategoryType(CategoryTypeViewModel categoryTypeViewModel, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task ManageAccountTypeAction(AccountViewModel accountViewModel, CancellationToken cancellationToken = default)
    {
        var editMode = accountViewModel.AccountType != null;
        var defaultText = editMode ? accountViewModel.AccountType!.Name ?? string.Empty : string.Empty;
        var placeholder = editMode ? AccountTypeManagementResources.TextBoxEditAccountTypeName
                                   : AccountTypeManagementResources.TextBoxAddNewAccountTypeName;

        var result = dialogService.ShowInputDialog(AccountTypeManagementResources.TitleWindow, defaultText,
            out var messageBoxResult, out var input, AccountTypeDomain.MaxNameLength, placeholder);

        if (result is not true || string.IsNullOrWhiteSpace(input)) return;

        switch (messageBoxResult, editMode)
        {
            case (MessageBoxInputResult.Delete, _):
                await DeleteAccountType(accountViewModel.AccountType!, cancellationToken);
                break;

            case (MessageBoxInputResult.Valid, false):
                await CreateAccountType(input, cancellationToken);
                break;

            case (MessageBoxInputResult.Valid, true):
                await UpdateAccountType(accountViewModel.AccountType!, input, cancellationToken);
                break;
        }
    }

    public async Task CreateAccountType(string input, CancellationToken cancellationToken = default)
    {
        var response = dialogService.ShowMessageBox(AccountResources.MessageBoxCreateItemQuestionCaption,
            string.Format(AccountResources.MessageBoxCreateItemQuestionContent, input),
            MessageBoxButton.YesNo, MsgBoxImage.Question);

        if (response is not MessageBoxResult.Yes) return;

        var available = await validationService.IsAccountTypeNameAvailableAsync(input, cancellationToken);
        if (available)
        {
            var newAccountType = new AccountTypeViewModel { Name = input };
            var result = await accountPresentationService.AddAccountType(newAccountType, cancellationToken);

            if (result.IsSuccess)
            {
                WeakReferenceMessenger.Default.Send(new EntityChangedMessage<AccountTypeViewModel>((DependencyType.AccountType, DataAction.Add, newAccountType)));
                dialogService.ShowMessageBox(AccountResources.MessageBoxCreateItemSuccessCaption,
                    string.Format(AccountResources.MessageBoxCreateItemSuccessContent, newAccountType.Name),
                    MsgBoxImage.Check);
            }
            else
            {
                dialogService.ShowMessageBox(AccountResources.MessageBoxCreateItemErrorCaption,
                    string.Format(AccountResources.MessageBoxCreateItemErrorContent, newAccountType.Name),
                    MsgBoxImage.Error);
            }
            return;
        }

        dialogService.ShowMessageBox(AccountResources.MessageBoxCreateItemErrorCaption,
            string.Format(AccountResources.MessageBoxCreateItemErrorAlreadyUsedContent, input),
            MsgBoxImage.Error);
    }

    public async Task UpdateAccountType(AccountTypeViewModel accountTypeViewModel, string input, CancellationToken cancellationToken = default)
    {
        var response = dialogService.ShowMessageBox(AccountResources.MessageBoxEditItemQuestionCaption,
            string.Format(AccountResources.MessageBoxEditItemQuestionContent, accountTypeViewModel.Name, input),
            MessageBoxButton.YesNo, MsgBoxImage.Question);

        if (response is not MessageBoxResult.Yes) return;

        var available = await validationService.IsAccountTypeNameAvailableAsync(input, accountTypeViewModel, cancellationToken);
        if (available)
        {
            accountTypeViewModel.Name = input;
            var result = await accountPresentationService.UpdateAccountTypeName(accountTypeViewModel, cancellationToken);
            if (result.IsSuccess)
            {
                WeakReferenceMessenger.Default.Send(new EntityChangedMessage<AccountTypeViewModel>((DependencyType.AccountType, DataAction.Update, accountTypeViewModel)));

                dialogService.ShowMessageBox(AccountResources.MessageBoxEditItemSuccessCaption,
                    AccountResources.MessageBoxEditItemSuccessContent,
                    MsgBoxImage.Check);
            }
            else
            {
                dialogService.ShowMessageBox(AccountResources.MessageBoxEditItemErrorCaption,
                    AccountResources.MessageBoxEditItemErrorContent, MsgBoxImage.Error);
            }
            return;
        }

        dialogService.ShowMessageBox(AccountResources.MessageBoxEditItemErrorAlreadyUsedCaption,
            string.Format(AccountResources.MessageBoxEditItemErrorAlreadyUsedContent, accountTypeViewModel.Name),
            MsgBoxImage.Error);
    }

    public async Task DeleteAccountType(AccountTypeViewModel accountTypeViewModel, CancellationToken cancellationToken = default)
    {
        var dependencies = await accountPresentationService.GetAllDependenciesAsync(accountTypeViewModel, cancellationToken);
        var dependenciesArray = dependencies.ToArray();

        var response = dependenciesArray.Length is 0
            ? dialogService.ShowMessageBox(AccountResources.MessageBoxDeleteItemQuestionCaption,
                string.Format(AccountResources.MessageBoxDeleteItemQuestionContent, accountTypeViewModel.Name),
                MessageBoxButton.YesNo, MsgBoxImage.Question)
            : dialogService.AskConfirmationOfDependenciesRemoval(DependencyType.AccountType, dependenciesArray);

        if (response is not MessageBoxResult.Yes) return;

        var deleteResult = await accountPresentationService.DeleteAccountTypeAsync(accountTypeViewModel, cancellationToken);

        if (deleteResult.IsSuccess)
        {
            if (deleteResult.DeletedItems?.TryGetValue(DependencyType.Account, out var accountIds) is true)
            {
                WeakReferenceMessenger.Default.Send(new EntityChangedMessage<int[]>((DependencyType.Account, DataAction.Delete, accountIds)));
            }
            WeakReferenceMessenger.Default.Send(new EntityChangedMessage<int>((DependencyType.AccountType, DataAction.Delete, accountTypeViewModel.Id)));

            dialogService.ShowMessageBox(AccountResources.MessageBoxDeleteItemSuccessCaption,
                string.Format(AccountResources.MessageBoxDeleteItemSuccessContent, accountTypeViewModel.Name),
                MsgBoxImage.Check);
        }
        else
        {
            dialogService.ShowMessageBox(AccountResources.MessageBoxDeletetemErrorCaption,
                AccountResources.MessageBoxDeletetemErrorContent,
                MsgBoxImage.Error);
        }
    }
}