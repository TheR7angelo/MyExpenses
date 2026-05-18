using Domain.Models.Dependencies;
using Domain.Models.Systems;
using Microsoft.Extensions.Logging;
using MyExpenses.Presentation.Enums;
using MyExpenses.Presentation.Messages;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.Validations.Validator;
using MyExpenses.Presentation.ViewModels.Expenses;

namespace MyExpenses.Presentation.Services;

public class ExpenseActionService(IExpensePresentationService expensePresentationService,
    ISystemPresentationService systemPresentationService,
    ILocationPresentationService locationPresentationService,
    IDialogService dialogService,
    ILogger<ExpenseActionService> logger,
    IServiceProvider serviceProvider) : AActionService(dialogService, logger, serviceProvider), IExpenseActionService
{
    private readonly IDialogService _dialogService = dialogService;

    public async Task<bool> ValidateBankTransfer(BankTransferViewModel bankTransferViewModel,
        HistoryViewModel historyViewModel, CancellationToken cancellationToken = default)
    {
        expensePresentationService.Merge(bankTransferViewModel, historyViewModel);
        historyViewModel.PlaceViewModel ??= await locationPresentationService.GetPlaceViewModel(PlaceDomain.DefaultPlaceId, cancellationToken);

        var bankTransferValidationTask = ValidateAsync<BankTransferViewModelValidator, BankTransferViewModel>(bankTransferViewModel, cancellationToken);
        var historyValidationTask = ValidateAsync<HistoryViewModelValidator, HistoryViewModel>(historyViewModel, cancellationToken);

        await Task.WhenAll(bankTransferValidationTask, historyValidationTask);

        if (bankTransferValidationTask.Result.IsValid && historyValidationTask.Result.IsValid) return true;

        bankTransferViewModel.ValidateWithFluent(bankTransferValidationTask.Result);
        historyViewModel.ValidateWithFluent(historyValidationTask.Result);
        return false;
    }

    public async Task CreateBankTransfer(BankTransferViewModel bankTransferViewModel, HistoryViewModel historyViewModel,
        CancellationToken cancellationToken = default)
    {
        var result = await expensePresentationService.CreateBankTransferAsync(bankTransferViewModel, historyViewModel, cancellationToken);

        if (result.IsSuccess)
        {
            SendEntityChangedMessage(DependencyType.BankTransfer, DataAction.Add, result.Value.bankTransferViewModel);
            foreach (var v in result.Value.historyViewModel)
            {
                SendEntityChangedMessage(DependencyType.Expense, DataAction.Add, v);
            }
        }

        ShowCreateResultMessage(result.IsSuccess, result.Value.bankTransferViewModel.MainReason!);
    }

    public async Task<bool> CreateCategoryType(CategoryTypeViewModel categoryTypeViewModel, CancellationToken cancellationToken = default)
    {
        if (!AskCreateConfirmation(categoryTypeViewModel.Name!)) return false;

        var result = await expensePresentationService.CreateCategoryType(categoryTypeViewModel, cancellationToken);

        if (result.IsSuccess)
        {
            SendEntityChangedMessage(DependencyType.CategoryType, DataAction.Add, categoryTypeViewModel);
        }

        ShowCreateResultMessage(result.IsSuccess, categoryTypeViewModel.Name!);
        return result.IsSuccess;
    }

    public async Task<bool> UpdateCategoryType(CategoryTypeViewModel categoryTypeViewModel, CancellationToken cancellationToken = default)
    {
        if (!categoryTypeViewModel.IsDirty) return false;

        var valResultAccount = await ValidateAsync<CategoryTypeViewModelValidator, CategoryTypeViewModel>(categoryTypeViewModel, cancellationToken);
        if (valResultAccount.IsValid)
        {
            if (!AskUpdateConfirmation(categoryTypeViewModel)) return false;

            var result = await expensePresentationService.UpdateCategoryTypeName(categoryTypeViewModel, cancellationToken);

            if (result.IsSuccess)
            {
                SendEntityChangedMessage(DependencyType.CategoryType, DataAction.Update, categoryTypeViewModel);
            }

            ShowUpdateResultMessage(result.IsSuccess);
            return true;
        }

        categoryTypeViewModel.ValidateWithFluent(valResultAccount);
        return false;
    }

    public async Task<bool> DeleteCategoryType(CategoryTypeViewModel categoryTypeViewModel, CancellationToken cancellationToken = default)
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

        if (response is not MessageBoxResult.Yes) return false;

        var deleteResult = await expensePresentationService.DeleteCategoryTypeAsync(categoryTypeViewModel, cancellationToken);

        if (deleteResult.IsSuccess)
        {
            SendEntityChangedMessage(DependencyType.CategoryType, DataAction.Delete, new[] { categoryTypeViewModel.Id });
        }

        ShowDeleteResultMessage(deleteResult.IsSuccess, categoryTypeViewModel.Name);
        return true;
    }
}