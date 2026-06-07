using Domain.Models.Dependencies;
using Domain.Models.Expenses;
using Domain.Models.Systems;
using Microsoft.Extensions.Logging;
using MyExpenses.Presentation.Enums;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.Messages;
using MyExpenses.Presentation.Resources.Resx.ExpenseResources;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.Validations.Validator;
using MyExpenses.Presentation.ViewModels.Expenses;

namespace MyExpenses.Presentation.Services;

public class ExpenseActionService(IExpensePresentationService expensePresentationService,
    ISystemPresentationService systemPresentationService,
    ILocationPresentationService locationPresentationService,
    IExpenseDtoViewModelMapper expenseDtoViewModelMapper,
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
        if (!bankTransferViewModel.IsDirty) return;

        var bankTransferValidationTask = ValidateAsync<BankTransferViewModelValidator, BankTransferViewModel>(bankTransferViewModel, cancellationToken);
        var historyValidationTask = ValidateAsync<HistoryViewModelValidator, HistoryViewModel>(historyViewModel, cancellationToken);

        await Task.WhenAll(bankTransferValidationTask, historyValidationTask);

        if (bankTransferValidationTask.Result.IsValid && historyValidationTask.Result.IsValid)
        {
            bankTransferViewModel.DateAdded ??= DateTime.Now;
            historyViewModel.DateAdded ??= DateTime.Now;
            if (historyViewModel.IsPointed) historyViewModel.DatePointed ??= DateTime.Now;

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
            return;
        }

        bankTransferViewModel.ValidateWithFluent(bankTransferValidationTask.Result);
        historyViewModel.ValidateWithFluent(historyValidationTask.Result);
    }

    public async Task<bool> CreateCategoryType(CategoryTypeViewModel categoryTypeViewModel, CancellationToken cancellationToken = default)
    {
        if (!categoryTypeViewModel.IsDirty) return false;

        var valResultCategoryType = await ValidateAsync<CategoryTypeViewModelValidator, CategoryTypeViewModel>(categoryTypeViewModel, cancellationToken);
        if (valResultCategoryType.IsValid)
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

        categoryTypeViewModel.ValidateWithFluent(valResultCategoryType);
        return false;
    }

    public async Task<bool> UpdateCategoryType(CategoryTypeViewModel categoryTypeViewModel, CancellationToken cancellationToken = default)
    {
        if (!categoryTypeViewModel.IsDirty) return false;

        var valResultCategoryType = await ValidateAsync<CategoryTypeViewModelValidator, CategoryTypeViewModel>(categoryTypeViewModel, cancellationToken);
        if (valResultCategoryType.IsValid)
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

        categoryTypeViewModel.ValidateWithFluent(valResultCategoryType);
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

    public Task ManageModePaymentAction(ModePaymentViewModel? modePaymentViewModel, CancellationToken cancellationToken = default)
    {
        return ManageNamedEntityAction(
            currentViewModel: modePaymentViewModel,
            getName: viewModel => viewModel.Name,
            setName: (viewModel, name) => viewModel.Name = name,
            maxNameLength: ModePaymentDomain.MaxNameLength,
            addTitle: ExpenseResources.TitleWindowAddModePayment,
            editTitle: ExpenseResources.TitleWindowEditModePayment,
            addPlaceholder: ExpenseResources.TextBoxAddNewModePayment,
            editPlaceholder: ExpenseResources.TextBoxEditModePayment,
            createValidationViewModel: () => new ModePaymentViewModel(),
            cloneValidationViewModel: expenseDtoViewModelMapper.Clone,
            beforeValidationAsync: _ => Task.CompletedTask,
            validateAsync: ValidateAsync<ModePaymentViewModelValidator, ModePaymentViewModel>,
            logValidationError: error => LogDomainValidationError("mode payment", error),
            deleteAsync: DeleteModePayment,
            createAsync: CreateModePayment,
            updateAsync: UpdateModePayment,
            cancellationToken: cancellationToken);
    }

    public async Task CreateModePayment(string input, CancellationToken cancellationToken = default)
    {
        if (!AskCreateConfirmation(input)) return;

        var newModePayment = new ModePaymentViewModel { Name = input };
        var result = await expensePresentationService.CreateModePayment(newModePayment, cancellationToken);

        if (result.IsSuccess)
        {
            SendEntityChangedMessage(DependencyType.ModePayment, DataAction.Add, result.Value);
        }

        ShowCreateResultMessage(result.IsSuccess, newModePayment.Name);
    }

    public async Task UpdateModePayment(ModePaymentViewModel modePaymentViewModel, string input, CancellationToken cancellationToken = default)
    {
        if (!AskUpdateConfirmation(modePaymentViewModel.Name, input)) return;

        modePaymentViewModel.Name = input;

        var result = await expensePresentationService.UpdateModePayment(modePaymentViewModel, cancellationToken);

        if (result.IsSuccess)
        {
            SendEntityChangedMessage(DependencyType.ModePayment, DataAction.Update, modePaymentViewModel);
        }

        ShowUpdateResultMessage(result.IsSuccess);
    }

    public async Task DeleteModePayment(ModePaymentViewModel modePaymentViewModel, CancellationToken cancellationToken = default)
    {
        var result = await systemPresentationService.GetAllDependenciesAsync(
            modePaymentViewModel, cancellationToken);

        if (!result.IsSuccess)
        {
            throw new InvalidOperationException("Failed to get dependencies for mode payment.");
        }

        var dependenciesArray = result.Value!.ToArray();

        var response = dependenciesArray.Length is 0
            ? AskDeleteConfirmation(modePaymentViewModel.Name)
            : _dialogService.AskConfirmationOfDependenciesRemoval(DependencyType.ModePayment, dependenciesArray);

        if (response is not MessageBoxResult.Yes) return;

        var deleteResult = await expensePresentationService.DeleteModePaymentAsync(modePaymentViewModel, cancellationToken);

        if (deleteResult.IsSuccess) SendDeletedMessageIfNeeded(deleteResult.DeletedItems);
        ShowDeleteResultMessage(deleteResult.IsSuccess, modePaymentViewModel.Name);
    }
}