using Domain.Models.Validation;
using MyExpenses.Presentation.ViewModels.Locations;

namespace MyExpenses.Presentation.Services.Interfaces;

public interface ILocationActionService
{
    /// <summary>
    /// Asynchronously creates a new place.
    /// </summary>
    /// <param name="placeViewModel">The view model representing the place to be created.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation and returns a Result containing the created PlaceViewModel or an error if the operation fails.</returns>
    public Task<Result<PlaceViewModel>> CreatePlaceAsync(PlaceViewModel placeViewModel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously updates an existing place.
    /// </summary>
    /// <param name="placeViewModel">The view model representing the place to be updated.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation and returns a Result containing the updated PlaceViewModel or an error if the operation fails.</returns>
    public Task<Result<PlaceViewModel>> UpdatePlaceAsync(PlaceViewModel placeViewModel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously deletes a place.
    /// </summary>
    /// <param name="placeViewModel">The view model representing the place to be deleted.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation and returns a DeletionResult containing information about the deletion result or an error if the operation fails.</returns>
    public Task<DeletionResult> DeletePlaceAsync(PlaceViewModel placeViewModel, CancellationToken cancellationToken = default);
}