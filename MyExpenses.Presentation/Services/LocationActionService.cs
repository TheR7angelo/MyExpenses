using Domain.Models.Dependencies;
using Domain.Models.Validation;
using Microsoft.Extensions.Logging;
using MyExpenses.Presentation.Messages;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.Validations.Validator;
using MyExpenses.Presentation.ViewModels.Locations;

namespace MyExpenses.Presentation.Services;

public class LocationActionService(
    IDialogService dialogService,
    IServiceProvider serviceProvider,
    ILocationPresentationService locationPresentationService,
    ILogger<LocationActionService> logger) : AActionService(dialogService, logger, serviceProvider), ILocationActionService
{
    // public async Task<bool> CreatePlace(PlaceViewModel placeViewModel, CancellationToken cancellationToken = default)
    // {
    //     var validationResult = await ValidateAsync<PlaceViewModelValidator, PlaceViewModel>(placeViewModel, cancellationToken);
    //
    //     if (validationResult.IsValid)
    //     {
    //         Result<HistoryViewModel> expenseResult;
    //
    //         var accountResult = await accountPresentationService.CreateAccount(accountViewModel, cancellationToken);
    //         historyViewModel.AccountViewModel = accountResult.Value;
    //         if (accountResult.IsSuccess) expenseResult = await expensePresentationService.CreateExpense(historyViewModel, cancellationToken);
    //         else
    //         {
    //             ShowCreateResultMessage(accountResult.IsSuccess, accountResult.Value?.Name ?? string.Empty);
    //             return false;
    //         }
    //
    //         if (!expenseResult.IsSuccess)
    //         {
    //             await accountPresentationService.DeleteAccountAsync(accountResult.Value!, cancellationToken);
    //             ShowCreateResultMessage(expenseResult.IsSuccess, expenseResult.Value?.Description ?? string.Empty);
    //             return false;
    //         }
    //
    //         SendEntityChangedMessage(DependencyType.Account, DataAction.Add, accountResult.Value);
    //         SendEntityChangedMessage(DependencyType.Expense, DataAction.Add, expenseResult.Value);
    //         return true;
    //     }
    //
    //     placeViewModel.ValidateWithFluent(validationResult);
    //     return false;
    // }

    public async Task<Result<PlaceViewModel>> CreatePlaceAsync(PlaceViewModel placeViewModel, CancellationToken cancellationToken = default)
    {
        var valResultPlace = await ValidateAsync<PlaceViewModelValidator, PlaceViewModel>(placeViewModel, cancellationToken);
        if (valResultPlace.IsValid)
        {
            if (!AskCreateConfirmation(placeViewModel.Name!)) return Result<PlaceViewModel>.Failure(ErrorCode.None, "Create cancelled.");

            var result = await locationPresentationService.CreatePlaceAsync(placeViewModel, cancellationToken);
            ShowCreateResultMessage(result.IsSuccess, placeViewModel.Name!);

            if (!result.IsSuccess) return result;
            SendEntityChangedMessage(DependencyType.Place, DataAction.Add, result.Value);
            return result;
        }

        placeViewModel.ValidateWithFluent(valResultPlace);
        return Result<PlaceViewModel>.Failure(ErrorCode.ValidationFailed, "Validation failed.");
    }

    public async Task<Result<PlaceViewModel>> UpdatePlaceAsync(PlaceViewModel placeViewModel, CancellationToken cancellationToken = default)
    {
        if (!placeViewModel.IsDirty) return Result<PlaceViewModel>.Failure(ErrorCode.None, "No changes to save.");

        var valResultColor = await ValidateAsync<PlaceViewModelValidator, PlaceViewModel>(placeViewModel, cancellationToken);
        if (valResultColor.IsValid)
        {
            var response = AskUpdateConfirmation(placeViewModel);
            if (!response)
            {
                placeViewModel.RejectChanges();
                return Result<PlaceViewModel>.Failure(ErrorCode.None, "Update cancelled.");
            }

            var result = await locationPresentationService.UpdatePlaceAsync(placeViewModel, cancellationToken);
            ShowUpdateResultMessage(result.IsSuccess);

            if (!result.IsSuccess) return result;
            SendEntityChangedMessage(DependencyType.Place, DataAction.Update, result.Value);
            return result;
        }

        placeViewModel.ValidateWithFluent(valResultColor);
        return Result<PlaceViewModel>.Failure(ErrorCode.ValidationFailed, "Validation failed.");
    }

    public Task<DeletionResult> DeletePlaceAsync(PlaceViewModel placeViewModel, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}