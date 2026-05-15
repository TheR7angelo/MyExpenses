using Domain.Models.Dependencies;
using Domain.Models.Validation;
using Microsoft.Extensions.Logging;
using MyExpenses.Presentation.Enums;
using MyExpenses.Presentation.Messages;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.Validations;
using MyExpenses.Presentation.ViewModels.Systems;

namespace MyExpenses.Presentation.Services;

public class SystemActionService(ISystemPresentationService systemPresentationService,
    IDialogService dialogService, ILogger<AActionService> logger, IServiceProvider serviceProvider)
    : AActionService(dialogService, logger, serviceProvider), ISystemActionService
{
    private readonly IDialogService _dialogService = dialogService;

    public async Task<Result<ColorViewModel>> CreateColorAsync(ColorViewModel colorViewModel, CancellationToken cancellationToken = default)
    {
        var valResultColor = await ValidateAsync<ColorViewModelValidator, ColorViewModel>(colorViewModel, cancellationToken);
        if (valResultColor.IsValid)
        {
            if (AskCreateConfirmation(colorViewModel.Name!)) return Result<ColorViewModel>.Failure(ErrorCode.None, "Create cancelled.");

            var result = await systemPresentationService.CreateColorAsync(colorViewModel, cancellationToken);
            ShowCreateResultMessage(result.IsSuccess, colorViewModel.Name!);

            if (!result.IsSuccess) return result;
            SendEntityChangedMessage(DependencyType.Color, DataAction.Add, result.Value);
            return result;
        }

        colorViewModel.ValidateWithFluent(valResultColor);
        return Result<ColorViewModel>.Failure(ErrorCode.ValidationFailed, "Validation failed.");
    }

    public async Task<Result<ColorViewModel>> UpdateColorAsync(ColorViewModel colorViewModel, CancellationToken cancellationToken = default)
    {
        if (!colorViewModel.IsDirty) return Result<ColorViewModel>.Failure(ErrorCode.None, "No changes to save.");

        var valResultColor = await ValidateAsync<ColorViewModelValidator, ColorViewModel>(colorViewModel, cancellationToken);
        if (valResultColor.IsValid)
        {
            var response = AskUpdateConfirmation(colorViewModel);
            if (!response)
            {
                colorViewModel.RejectChanges();
                return Result<ColorViewModel>.Failure(ErrorCode.None, "Update cancelled.");
            }

            var result = await systemPresentationService.UpdateColorAsync(colorViewModel, cancellationToken);
            ShowUpdateResultMessage(result.IsSuccess);

            if (!result.IsSuccess) return result;
            SendEntityChangedMessage(DependencyType.Color, DataAction.Update, result.Value);
            return result;
        }

        colorViewModel.ValidateWithFluent(valResultColor);
        return Result<ColorViewModel>.Failure(ErrorCode.ValidationFailed, "Validation failed.");
    }

    public async Task<DeletionResult> DeleteColorAsync(ColorViewModel colorViewModel, CancellationToken cancellationToken = default)
    {
        var resultDependency = await systemPresentationService.GetAllDependenciesAsync(colorViewModel, cancellationToken);
        if (!resultDependency.IsSuccess)
        {
            ShowDeleteResultMessage(false, colorViewModel.Name);
            return DeletionResult.Failure(ErrorCode.DatabaseError, "Failed to retrieve dependencies.");
        }

        var dependenciesArray = resultDependency.Value!.ToArray();

        var response = dependenciesArray.Length is 0
            ? AskDeleteConfirmation(colorViewModel.Name)
            : _dialogService.AskConfirmationOfDependenciesRemoval(DependencyType.Color, dependenciesArray);

        if (response is not MessageBoxResult.Yes) return DeletionResult.Failure(ErrorCode.None, "Deletion cancelled.");

        var result = await systemPresentationService.DeleteColorAsync(colorViewModel, cancellationToken);
        ShowDeleteResultMessage(result.IsSuccess, colorViewModel.Name);

        if (!result.IsSuccess) return result;
        SendEntityChangedMessage(DependencyType.Color, DataAction.Delete, result.Value);

        return result;
    }
}