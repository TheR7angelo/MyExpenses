using CommunityToolkit.Mvvm.Messaging;
using Domain.Models.Accounts;
using Domain.Models.Categories;
using Domain.Models.Dependencies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyExpenses.Presentation.Enums;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.Messages;
using MyExpenses.Presentation.Resources.Resx.AccountResources;
using MyExpenses.Presentation.Resources.Resx.CategoryManagementResources;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.Validations;
using MyExpenses.Presentation.Validations.Interfaces;
using MyExpenses.Presentation.ViewModels.Accounts;
using MyExpenses.Presentation.ViewModels.Expenses;

namespace MyExpenses.Wpf.Services;

public class AccountActionService(
    IDialogService dialogService,
    IAccountPresentationService accountPresentationService, ISystemPresentationService systemPresentationService,
    IAccountPresentationValidationService accountPresentationValidationService,
    IExpensePresentationValidationService expensePresentationValidationService, IExpensePresentationService expensePresentationService,
    IAccountDtoViewModelMapper accountDtoViewModelMapper,
    ILogger<AccountActionService> logger) : IAccountActionService
{
    public async Task ManageCategoryTypeAction(HistoryViewModel historyViewModel, CancellationToken cancellationToken = default)
    {
        var editMode = historyViewModel.CategoryTypeViewModel != null;
        var defaultText = editMode ? historyViewModel.CategoryTypeViewModel!.Name ?? string.Empty : string.Empty;

        var placeholder = editMode ? CategoryManagementResources.TextBoxEditCategoryTypeName
            : CategoryManagementResources.TextBoxAddNewCategoryTypeName;

        var titleWindow = editMode
            ? CategoryManagementResources.TitleWindowEditCategoryTypeName
            : CategoryManagementResources.TitleWindowAddCategoryTypeName;

        var result = dialogService.ShowInputDialog(titleWindow, defaultText,
            out var messageBoxResult, out var input, CategoryTypeDomain.MaxNameLength, placeholder);

        if (result is not true || string.IsNullOrWhiteSpace(input)) return;
        if (input.Equals(defaultText)) return;

        switch (messageBoxResult, editMode)
        {
            // case (MessageBoxInputResult.Delete, _):
                // await DeleteAccountType(accountViewModel.AccountType!, cancellationToken);
                // break;

            case (MessageBoxInputResult.Valid, false):
                await CreateCategoryType(input, cancellationToken);
                break;

            case (MessageBoxInputResult.Valid, true):
                await UpdateCategoryType(historyViewModel.CategoryTypeViewModel!, input, cancellationToken);
                break;
        }
    }

    public async Task CreateCategoryType(string input, CancellationToken cancellationToken = default)
    {
        var response = dialogService.ShowMessageBox(AccountResources.MessageBoxCreateItemQuestionCaption,
            string.Format(AccountResources.MessageBoxCreateItemQuestionContent, input),
            MessageBoxButton.YesNo, MsgBoxImage.Question);

        if (response is not MessageBoxResult.Yes) return;

        var available = await expensePresentationValidationService.IsCategoryTypeNameAvailableAsync(input, cancellationToken);
        if (available)
        {
            var randomColor = await systemPresentationService.GetRandomColorViewModel(cancellationToken);

            var newCategoryType = new CategoryTypeViewModel { Name = input, Color = randomColor };
            var result = await expensePresentationService.AddCategoryType(newCategoryType, cancellationToken);

            if (result.IsSuccess)
            {
                WeakReferenceMessenger.Default.Send(new EntityChangedMessage<CategoryTypeViewModel>((DependencyType.CategoryType, DataAction.Add, newCategoryType)));
                dialogService.ShowMessageBox(AccountResources.MessageBoxCreateItemSuccessCaption,
                    string.Format(AccountResources.MessageBoxCreateItemSuccessContent, newCategoryType.Name),
                    MsgBoxImage.Check);
            }
            else
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

    public async Task UpdateCategoryType(CategoryTypeViewModel categoryTypeViewModel, string input,
        CancellationToken cancellationToken = default)
    {
        var response = dialogService.ShowMessageBox(AccountResources.MessageBoxEditItemQuestionCaption,
            string.Format(AccountResources.MessageBoxEditItemQuestionContent, categoryTypeViewModel.Name, input),
            MessageBoxButton.YesNo, MsgBoxImage.Question);

        if (response is not MessageBoxResult.Yes) return;

        // TODO continue here
        var available = await expensePresentationValidationService.IsCategoryTypeNameAvailableAsync(input, categoryTypeViewModel, cancellationToken);
        Console.WriteLine(available);
        // if (available)
        // {
        //     categoryTypeViewModel.Name = input;
        //     var result = await accountPresentationService.UpdateAccountTypeName(categoryTypeViewModel, cancellationToken);
        //     if (result.IsSuccess)
        //     {
        //         WeakReferenceMessenger.Default.Send(new EntityChangedMessage<CategoryTypeViewModel>((DependencyType.CategoryType, DataAction.Update, categoryTypeViewModel)));
        //
        //         dialogService.ShowMessageBox(AccountResources.MessageBoxEditItemSuccessCaption,
        //             AccountResources.MessageBoxEditItemSuccessContent,
        //             MsgBoxImage.Check);
        //     }
        //     else
        //     {
        //         dialogService.ShowMessageBox(AccountResources.MessageBoxEditItemErrorCaption,
        //             AccountResources.MessageBoxEditItemErrorContent, MsgBoxImage.Error);
        //     }
        //     return;
        // }
        //
        // dialogService.ShowMessageBox(AccountResources.MessageBoxEditItemErrorAlreadyUsedCaption,
        //     string.Format(AccountResources.MessageBoxEditItemErrorAlreadyUsedContent, categoryTypeViewModel.Name),
        //     MsgBoxImage.Error);
    }

    public async Task DeleteCategoryType(CategoryTypeViewModel categoryTypeViewModel, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task ManageAccountTypeAction(AccountViewModel accountViewModel, CancellationToken cancellationToken = default)
    {
        var isEdit = accountViewModel.AccountTypeViewModel != null;
        var currentName = isEdit ? accountViewModel.AccountTypeViewModel!.Name ?? string.Empty : string.Empty;

        var title = isEdit ? AccountResources.TitleWindowEditAccountTypeName : AccountResources.TitleWindowAddAccountTypeName;
        var placeholder = isEdit ? AccountResources.TextBoxEditAccountTypeName : AccountResources.TextBoxAddNewAccountTypeName;

        var dialogResult = dialogService.ShowInputDialog(title, currentName,
            out var btnResult, out var input, AccountTypeDomain.MaxNameLength, placeholder);

        if (dialogResult is not true || btnResult is MessageBoxInputResult.Cancel) return;

        if (btnResult is MessageBoxInputResult.Delete && isEdit)
        {
            await DeleteAccountType(accountViewModel.AccountTypeViewModel!, cancellationToken);
            return;
        }

        var tempVm = isEdit ? accountDtoViewModelMapper.Clone(accountViewModel.AccountTypeViewModel!) : new AccountTypeViewModel();
        tempVm.Name = input;

        var validator = App.ServiceProvider.GetRequiredService<AccountTypeViewModelValidator>();
        var valResult = await validator.ValidateAsync(tempVm, cancellationToken);

        if (!valResult.IsValid)
        {
            var error = valResult.Errors.First();
            dialogService.ShowMessageBox(AccountResources.MessageBoxCreateItemErrorCaption, error.ErrorMessage, MsgBoxImage.Error);

            if (error.CustomState is DomainValidationFailure domainError)
            {
                logger.LogError("Validation failed for account type with error {ErrorCodeString}: {InternalMessage}", domainError.ErrorCodeString, domainError.InternalMessage);
            }

            // ReSharper disable once TailRecursiveCall
            await ManageAccountTypeAction(accountViewModel, cancellationToken);
            return;
        }

        if (isEdit)
        {
            await UpdateAccountType(tempVm, input!, cancellationToken);
        }
        else
        {
            await CreateAccountType(input!, cancellationToken);
        }
    }

    public async Task CreateAccountType(string input, CancellationToken cancellationToken = default)
    {
        var response = dialogService.ShowMessageBox(AccountResources.MessageBoxCreateItemQuestionCaption,
            string.Format(AccountResources.MessageBoxCreateItemQuestionContent, input),
            MessageBoxButton.YesNo, MsgBoxImage.Question);

        if (response is not MessageBoxResult.Yes) return;

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

        // var available = await accountPresentationValidationService.IsAccountTypeNameAvailableAsync(input, cancellationToken);
        // if (available)
        // {
        //     var newAccountType = new AccountTypeViewModel { Name = input };
        //     var result = await accountPresentationService.AddAccountType(newAccountType, cancellationToken);
        //
        //     if (result.IsSuccess)
        //     {
        //         WeakReferenceMessenger.Default.Send(new EntityChangedMessage<AccountTypeViewModel>((DependencyType.AccountType, DataAction.Add, newAccountType)));
        //         dialogService.ShowMessageBox(AccountResources.MessageBoxCreateItemSuccessCaption,
        //             string.Format(AccountResources.MessageBoxCreateItemSuccessContent, newAccountType.Name),
        //             MsgBoxImage.Check);
        //     }
        //     else
        //     {
        //         dialogService.ShowMessageBox(AccountResources.MessageBoxCreateItemErrorCaption,
        //             string.Format(AccountResources.MessageBoxCreateItemErrorContent, newAccountType.Name),
        //             MsgBoxImage.Error);
        //     }
        //     return;
        // }
        //
        // dialogService.ShowMessageBox(AccountResources.MessageBoxCreateItemErrorCaption,
        //     string.Format(AccountResources.MessageBoxCreateItemErrorAlreadyUsedContent, input),
        //     MsgBoxImage.Error);
    }

    public async Task UpdateAccountType(AccountTypeViewModel accountTypeViewModel, string input, CancellationToken cancellationToken = default)
    {
        var response = dialogService.ShowMessageBox(AccountResources.MessageBoxEditItemQuestionCaption,
            string.Format(AccountResources.MessageBoxEditItemQuestionContent, accountTypeViewModel.Name, input),
            MessageBoxButton.YesNo, MsgBoxImage.Question);

        if (response is not MessageBoxResult.Yes) return;

        var available = await accountPresentationValidationService.IsAccountTypeNameAvailableAsync(input, accountTypeViewModel, cancellationToken);
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