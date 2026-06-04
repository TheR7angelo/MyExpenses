using Domain.Models.Dependencies;
using Domain.Models.Validation;
using Microsoft.Extensions.Logging;
using MyExpenses.Presentation.Enums;
using MyExpenses.Presentation.Messages;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.Validations.Validator;
using MyExpenses.Presentation.ViewModels.Locations;

namespace MyExpenses.Presentation.Services;

/// <summary>
/// Service handling user actions related to locations (places): create, update and delete operations.
/// Delegates persistence and business logic to the presentation services and manages user confirmations
/// and result messages through dialog services.
/// </summary>
/// <param name="dialogService">Dialog service used to show confirmations and messages to the user.</param>
/// <param name="serviceProvider">Service provider used to resolve additional services when needed.</param>
/// <param name="locationPresentationService">Presentation service responsible for CRUD operations on places.</param>
/// <param name="systemPresentationService">Presentation service used to retrieve dependencies before deletion.</param>
/// <param name="logger">Logger instance for logging actions and errors.</param>
public class LocationActionService(
    IDialogService dialogService,
    IServiceProvider serviceProvider,
    ILocationPresentationService locationPresentationService,
    ISystemPresentationService systemPresentationService,
    ILogger<LocationActionService> logger) : AActionService(dialogService, logger, serviceProvider), ILocationActionService
{
    private readonly IDialogService _dialogService = dialogService;

    /// <summary>
    /// Creates a new place asynchronously after validating the provided <see cref="PlaceViewModel"/>.
    /// Shows a confirmation dialog before creation and notifies other parts of the application when the entity is added.
    /// </summary>
    /// <param name="placeViewModel">The place view model to create.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A <see cref="Result{PlaceViewModel}"/> containing the created place on success or an error code on failure.</returns>
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

    /// <summary>
    /// Updates an existing place asynchronously. Validates the view model and asks the user for confirmation
    /// if changes are present. On success notifies other components about the update.
    /// </summary>
    /// <param name="placeViewModel">The place view model containing updated data.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A <see cref="Result{PlaceViewModel}"/> with the updated place on success or an error on failure.</returns>
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

    /// <summary>
    /// Deletes the specified place after checking for system dependencies and asking for user confirmation.
    /// If dependencies exist the dialog service will prompt to confirm removal of related data.
    /// </summary>
    /// <param name="placeViewModel">The place view model to delete.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A <see cref="DeletionResult"/> indicating success or failure and information about deleted items.</returns>
    public async Task<DeletionResult> DeletePlaceAsync(PlaceViewModel placeViewModel, CancellationToken cancellationToken = default)
    {
        var resultDependency = await systemPresentationService.GetAllDependenciesAsync(placeViewModel, cancellationToken);
        if (!resultDependency.IsSuccess)
        {
            ShowDeleteResultMessage(false, placeViewModel.Name);
            return DeletionResult.Failure(ErrorCode.DatabaseError, "Failed to retrieve dependencies.");
        }

        var dependenciesArray = resultDependency.Value!.ToArray();

        var response = dependenciesArray.Length is 0
            ? AskDeleteConfirmation(placeViewModel.Name)
            : _dialogService.AskConfirmationOfDependenciesRemoval(DependencyType.Place, dependenciesArray);

        if (response is not MessageBoxResult.Yes) return DeletionResult.Failure(ErrorCode.None, "Deletion cancelled.");

        var deleteResult = await locationPresentationService.DeletePlaceAsync(placeViewModel, cancellationToken);
        ShowDeleteResultMessage(deleteResult.IsSuccess, placeViewModel.Name);

        if (!deleteResult.IsSuccess) return deleteResult;

        SendDeletedMessageIfNeeded(deleteResult.DeletedItems);
        SendEntityChangedMessage(DependencyType.Place, DataAction.Delete, new[] { placeViewModel.Id });

        return deleteResult;
    }
}