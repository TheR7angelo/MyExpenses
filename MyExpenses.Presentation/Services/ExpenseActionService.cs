using Domain.Models.Dependencies;
using Domain.Models.Systems;
using Microsoft.Extensions.Logging;
using MyExpenses.Presentation.Messages;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.Validations;
using MyExpenses.Presentation.ViewModels.Expenses;

namespace MyExpenses.Presentation.Services;

public class ExpenseActionService(IExpensePresentationService expensePresentationService,
    ISystemPresentationService systemPresentationService,
    IDialogService dialogService,
    ILogger<ExpenseActionService> logger,
    IServiceProvider serviceProvider) : AActionService(dialogService, logger, serviceProvider), IExpenseActionService
{
    public async Task<bool> ValidateBankTransfer(BankTransferViewModel bankTransferViewModel,
        HistoryViewModel historyViewModel, CancellationToken cancellationToken = default)
    {
        expensePresentationService.Merge(bankTransferViewModel, historyViewModel);
        historyViewModel.PlaceViewModel ??= await systemPresentationService.GetPlaceViewModel(PlaceDomain.DefaultPlaceId, cancellationToken);

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
}