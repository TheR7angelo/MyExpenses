using CommunityToolkit.Mvvm.Messaging;
using Domain.Models.Accounts;
using Domain.Models.Dependencies;
using MyExpenses.Presentation.Enums;
using MyExpenses.Presentation.Messages;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.Validations.Interfaces;
using MyExpenses.Presentation.ViewModels.Accounts;
using MyExpenses.SharedUtils.Resources.Resx.AccountTypeManagement;

namespace MyExpenses.Wpf.Services;

public class AccountActionService(
    IDialogService dialogService,
    IAccountPresentationService accountPresentationService,
    IAccountPresentationValidationService validationService) : IAccountActionService
{
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
        // TODO translate
        var response = dialogService.ShowMessageBox("Confirmation", $"Are you sure you want to create '{input}' ?", MessageBoxButton.YesNo, MsgBoxImage.Question);
        if (response is not MessageBoxResult.Yes) return;

        var available = await validationService.IsAccountTypeNameAvailableAsync(input, cancellationToken);
        if (available)
        {
            var newAccountType = new AccountTypeViewModel { Name = input };
            var result = await accountPresentationService.AddAccountType(newAccountType, cancellationToken);

            if (result.IsSuccess)
            {
                WeakReferenceMessenger.Default.Send(new EntityChangedMessage<AccountTypeViewModel>((EntityType.AccountType, DataAction.Add, newAccountType)));
                // TODO translate
                dialogService.ShowMessageBox("Success", $"The account type '{newAccountType.Name}' was successfully created", MsgBoxImage.Check);
            }
            else
            {
                // TODO translate
                dialogService.ShowMessageBox("Error", $"Failed to create account type '{newAccountType.Name}'. Please try again.", MsgBoxImage.Error);
            }
            return;
        }

        // TODO translate
        dialogService.ShowMessageBox("Error", $"The name {input} is already used", MsgBoxImage.Error);
    }

    public async Task UpdateAccountType(AccountTypeViewModel accountTypeViewModel, string input, CancellationToken cancellationToken = default)
    {
        // TODO translate
        var response = dialogService.ShowMessageBox("Confirmation", $"Are you sure you want to rename '{accountTypeViewModel.Name}' to '{input}' ?", MessageBoxButton.YesNo, MsgBoxImage.Question);
        if (response is not MessageBoxResult.Yes) return;

        var available = await validationService.IsAccountTypeNameAvailableAsync(input, accountTypeViewModel, cancellationToken);
        if (available)
        {
            accountTypeViewModel.Name = input;
            var result = await accountPresentationService.UpdateAccountTypeName(accountTypeViewModel, cancellationToken);
            if (result.IsSuccess)
            {
                WeakReferenceMessenger.Default.Send(new EntityChangedMessage<AccountTypeViewModel>((EntityType.AccountType, DataAction.Update, accountTypeViewModel)));

                // TODO translate
                dialogService.ShowMessageBox("Success", $"The account type '{accountTypeViewModel.Name}' was successfully edited", MsgBoxImage.Check);
            }
            else
            {
                // TODO translate
                dialogService.ShowMessageBox("Error", "An error occurred please retry", MsgBoxImage.Error);
            }
            return;
        }

        dialogService.ShowMessageBox("Error", $"The name {input} is already used", MsgBoxImage.Error);
    }

    public async Task DeleteAccountType(AccountTypeViewModel accountTypeViewModel, CancellationToken cancellationToken = default)
    {
        var dependencies = await accountPresentationService.GetAllDependenciesAsync(accountTypeViewModel, cancellationToken);
        var dependenciesArray = dependencies.ToArray();

        var response = dependenciesArray.Length is 0
            // TODO translate
            ? dialogService.ShowMessageBox("Confirmation", $"Are you sure you want to delete '{accountTypeViewModel.Name}' ?", MessageBoxButton.YesNo, MsgBoxImage.Question)
            : dialogService.AskConfirmationOfDependenciesRemoval(EntityType.AccountType, dependenciesArray);

        if (response is not MessageBoxResult.Yes) return;

        var deleteResult = await accountPresentationService.DeleteAccountTypeAsync(accountTypeViewModel, cancellationToken);

        if (deleteResult.IsSuccess)
        {
            if (deleteResult.DeletedItems?.TryGetValue(EntityType.Account, out var accountIds) is true)
            {
                WeakReferenceMessenger.Default.Send(new EntityChangedMessage<int[]>((EntityType.Account, DataAction.Delete, accountIds)));
            }
            WeakReferenceMessenger.Default.Send(new EntityChangedMessage<int>((EntityType.AccountType, DataAction.Delete, accountTypeViewModel.Id)));

            // TODO translate
            dialogService.ShowMessageBox("Success", $"The account type '{accountTypeViewModel.Name}' was successfully deleted", MsgBoxImage.Check);
        }
        else
        {
            // TODO translate
            dialogService.ShowMessageBox("Error", "An error occurred please retry", MsgBoxImage.Error);
        }
    }
}