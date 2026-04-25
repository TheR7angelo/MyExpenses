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
using MyExpenses.Presentation.Resources.Resx.ExpenseManagementResources;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.Validations;
using MyExpenses.Presentation.ViewModels.Accounts;
using MyExpenses.Presentation.ViewModels.Expenses;

namespace MyExpenses.Wpf.Services;

public class AccountActionService(
    IDialogService dialogService,
    IAccountPresentationService accountPresentationService, ISystemPresentationService systemPresentationService,
    IExpensePresentationService expensePresentationService,
    IAccountDtoViewModelMapper accountDtoViewModelMapper, IExpenseDtoViewModelMapper expenseDtoViewModelMapper,
    ILogger<AccountActionService> logger) : IAccountActionService
{
    public async Task ManageCategoryTypeAction(HistoryViewModel historyViewModel, CancellationToken cancellationToken = default)
    {
        var isEdit = historyViewModel.CategoryTypeViewModel != null;
        var currentName = isEdit ? historyViewModel.CategoryTypeViewModel!.Name ?? string.Empty : string.Empty;

        var placeholder = isEdit ? ExpenseManagementResources.TextBoxEditCategoryTypeName
            : ExpenseManagementResources.TextBoxAddNewCategoryTypeName;

        var title = isEdit
            ? ExpenseManagementResources.TitleWindowEditCategoryTypeName
            : ExpenseManagementResources.TitleWindowAddCategoryTypeName;

        var dialogResult = dialogService.ShowInputDialog(title, currentName,
        out var btnResult, out var input, CategoryTypeDomain.MaxNameLength, placeholder);

        if (dialogResult is not true || btnResult is MessageBoxInputResult.Cancel) return;

        if (btnResult is MessageBoxInputResult.Delete && isEdit)
        {
            await DeleteCategoryType(historyViewModel.CategoryTypeViewModel!, cancellationToken);
            return;
        }

        var tempVm = isEdit ? expenseDtoViewModelMapper.Clone(historyViewModel.CategoryTypeViewModel!) : new CategoryTypeViewModel();
        tempVm.Name = input;
        tempVm.Color ??= await systemPresentationService.GetRandomColorViewModel(cancellationToken);

        var validator = App.ServiceProvider.GetRequiredService<CategoryTypeViewModelValidator>();
        var valResult = await validator.ValidateAsync(tempVm, cancellationToken);

        if (!valResult.IsValid)
        {
            var error = valResult.Errors.First();
            dialogService.ShowMessageBox(AccountResources.MessageBoxCreateItemErrorCaption, error.ErrorMessage, MsgBoxImage.Error);

            if (error.CustomState is DomainValidationFailure domainError)
            {
                logger.LogError("Validation failed for category type with error {ErrorCodeString}: {InternalMessage}", domainError.ErrorCodeString, domainError.InternalMessage);
            }

            // ReSharper disable once TailRecursiveCall
            await ManageCategoryTypeAction(historyViewModel, cancellationToken);
            return;
        }

        if (isEdit)
        {
            await UpdateCategoryType(historyViewModel.CategoryTypeViewModel!, input!, cancellationToken);
        }
        else
        {
            await CreateCategoryType(input!, cancellationToken);
        }
    }

    public async Task CreateCategoryType(string input, CancellationToken cancellationToken = default)
    {
        var response = dialogService.ShowMessageBox(AccountResources.MessageBoxCreateItemQuestionCaption,
            string.Format(AccountResources.MessageBoxCreateItemQuestionContent, input),
            MessageBoxButton.YesNo, MsgBoxImage.Question);

        if (response is not MessageBoxResult.Yes) return;

        var newCategoryTypeViewModel = new CategoryTypeViewModel { Name = input, Color = await systemPresentationService.GetRandomColorViewModel(cancellationToken) };
        var result = await expensePresentationService.AddCategoryType(newCategoryTypeViewModel, cancellationToken);

        if (result.IsSuccess)
        {
            WeakReferenceMessenger.Default.Send(new EntityChangedMessage<CategoryTypeViewModel>(new EntityChanged<CategoryTypeViewModel>
            {
                EntityType = DependencyType.CategoryType,
                DataAction = DataAction.Add,
                Content = newCategoryTypeViewModel
            }));

            dialogService.ShowMessageBox(AccountResources.MessageBoxCreateItemSuccessCaption,
                string.Format(AccountResources.MessageBoxCreateItemSuccessContent, newCategoryTypeViewModel.Name),
                MsgBoxImage.Check);
        }
        else
        {
            dialogService.ShowMessageBox(AccountResources.MessageBoxCreateItemErrorCaption,
                string.Format(AccountResources.MessageBoxCreateItemErrorContent, newCategoryTypeViewModel.Name),
                MsgBoxImage.Error);
        }
    }

    public async Task UpdateCategoryType(CategoryTypeViewModel categoryTypeViewModel, string input,
        CancellationToken cancellationToken = default)
    {
        var response = dialogService.ShowMessageBox(AccountResources.MessageBoxEditItemQuestionCaption,
            string.Format(AccountResources.MessageBoxEditItemQuestionContent, categoryTypeViewModel.Name, input),
            MessageBoxButton.YesNo, MsgBoxImage.Question);

        if (response is not MessageBoxResult.Yes) return;

        categoryTypeViewModel.Name = input;
        var result = await expensePresentationService.UpdateCategoryTypeName(categoryTypeViewModel, cancellationToken);
        if (result.IsSuccess)
        {
            WeakReferenceMessenger.Default.Send(new EntityChangedMessage<CategoryTypeViewModel>(new EntityChanged<CategoryTypeViewModel>
            {
                EntityType = DependencyType.CategoryType,
                DataAction = DataAction.Update,
                Content = categoryTypeViewModel
            }));

            dialogService.ShowMessageBox(AccountResources.MessageBoxEditItemSuccessCaption,
                AccountResources.MessageBoxEditItemSuccessContent,
                MsgBoxImage.Check);
        }
        else
        {
            dialogService.ShowMessageBox(AccountResources.MessageBoxEditItemErrorCaption,
                AccountResources.MessageBoxEditItemErrorContent, MsgBoxImage.Error);
        }
    }

    public async Task DeleteCategoryType(CategoryTypeViewModel categoryTypeViewModel, CancellationToken cancellationToken = default)
    {
        var dependencies = await systemPresentationService.GetAllDependenciesAsync(categoryTypeViewModel, cancellationToken);
        var dependenciesArray = dependencies.ToArray();

        var response = dependenciesArray.Length is 0
            ? dialogService.ShowMessageBox(AccountResources.MessageBoxDeleteItemQuestionCaption,
                string.Format(AccountResources.MessageBoxDeleteItemQuestionContent, categoryTypeViewModel.Name),
                MessageBoxButton.YesNo, MsgBoxImage.Question)
            : dialogService.AskConfirmationOfDependenciesRemoval(DependencyType.CategoryType, dependenciesArray);

        if (response is not MessageBoxResult.Yes) return;

        var deleteResult = await expensePresentationService.DeleteCategoryTypeAsync(categoryTypeViewModel, cancellationToken);

        if (deleteResult.IsSuccess)
        {
            WeakReferenceMessenger.Default.Send(new EntityChangedMessage<int>(new EntityChanged<int>
            {
                EntityType = DependencyType.CategoryType,
                DataAction = DataAction.Delete,
                Content = categoryTypeViewModel.Id
            }));

            dialogService.ShowMessageBox(AccountResources.MessageBoxDeleteItemSuccessCaption,
                string.Format(AccountResources.MessageBoxDeleteItemSuccessContent, categoryTypeViewModel.Name),
                MsgBoxImage.Check);
        }
        else
        {
            dialogService.ShowMessageBox(AccountResources.MessageBoxDeletetemErrorCaption,
                AccountResources.MessageBoxDeleteteErrorContent,
                MsgBoxImage.Error);
        }
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
            WeakReferenceMessenger.Default.Send(new EntityChangedMessage<AccountTypeViewModel>(new EntityChanged<AccountTypeViewModel>
            {
                EntityType = DependencyType.AccountType,
                DataAction = DataAction.Add,
                Content = newAccountType
            }));

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
    }

    public async Task UpdateAccountType(AccountTypeViewModel accountTypeViewModel, string input, CancellationToken cancellationToken = default)
    {
        var response = dialogService.ShowMessageBox(AccountResources.MessageBoxEditItemQuestionCaption,
            string.Format(AccountResources.MessageBoxEditItemQuestionContent, accountTypeViewModel.Name, input),
            MessageBoxButton.YesNo, MsgBoxImage.Question);

        if (response is not MessageBoxResult.Yes) return;

        accountTypeViewModel.Name = input;
        var result = await accountPresentationService.UpdateAccountTypeName(accountTypeViewModel, cancellationToken);
        if (result.IsSuccess)
        {
            WeakReferenceMessenger.Default.Send(new EntityChangedMessage<AccountTypeViewModel>(new EntityChanged<AccountTypeViewModel>
            {
                EntityType = DependencyType.AccountType,
                DataAction = DataAction.Update,
                Content = accountTypeViewModel
            }));

            dialogService.ShowMessageBox(AccountResources.MessageBoxEditItemSuccessCaption,
                AccountResources.MessageBoxEditItemSuccessContent,
                MsgBoxImage.Check);
        }
        else
        {
            dialogService.ShowMessageBox(AccountResources.MessageBoxEditItemErrorCaption,
                AccountResources.MessageBoxEditItemErrorContent, MsgBoxImage.Error);
        }
    }

    public async Task DeleteAccountType(AccountTypeViewModel accountTypeViewModel, CancellationToken cancellationToken = default)
    {
        var dependencies = await systemPresentationService.GetAllDependenciesAsync(accountTypeViewModel, cancellationToken);
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
                WeakReferenceMessenger.Default.Send(new EntityChangedMessage<int[]>(new EntityChanged<int[]>
                {
                    EntityType = DependencyType.Account,
                    DataAction = DataAction.Delete,
                    Content = accountIds
                }));
            }
            WeakReferenceMessenger.Default.Send(new EntityChangedMessage<int>(new EntityChanged<int>
            {
                EntityType = DependencyType.AccountType,
                DataAction = DataAction.Delete,
                Content = accountTypeViewModel.Id
            }));

            dialogService.ShowMessageBox(AccountResources.MessageBoxDeleteItemSuccessCaption,
                string.Format(AccountResources.MessageBoxDeleteItemSuccessContent, accountTypeViewModel.Name),
                MsgBoxImage.Check);
        }
        else
        {
            dialogService.ShowMessageBox(AccountResources.MessageBoxDeletetemErrorCaption,
                AccountResources.MessageBoxDeleteteErrorContent,
                MsgBoxImage.Error);
        }
    }
}