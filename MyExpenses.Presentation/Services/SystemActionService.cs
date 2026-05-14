using Domain.Models.Dependencies;
using Domain.Models.Validation;
using Microsoft.Extensions.Logging;
using MyExpenses.Presentation.Messages;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.Validations;
using MyExpenses.Presentation.ViewModels.Systems;

namespace MyExpenses.Presentation.Services;

public class SystemActionService(ISystemPresentationService systemPresentationService,
    IDialogService dialogService, ILogger<AActionService> logger, IServiceProvider serviceProvider)
    : AActionService(dialogService, logger, serviceProvider), ISystemActionService
{
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
}