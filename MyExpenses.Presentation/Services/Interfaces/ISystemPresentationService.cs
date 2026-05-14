using Domain.Models.Dependencies;
using Domain.Models.Validation;
using MyExpenses.Presentation.ViewModels.Accounts;
using MyExpenses.Presentation.ViewModels.Expenses;
using MyExpenses.Presentation.ViewModels.Systems;

namespace MyExpenses.Presentation.Services.Interfaces;

public interface ISystemPresentationService
{
    /// <summary>
    /// Retrieves all deletion dependencies for the specified account type.
    /// </summary>
    /// <param name="accountTypeViewModel">The account type view model for which dependencies are to be retrieved.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <return>A collection of <see cref="DeletionDependency"/> representing the dependencies associated with the specified account type.</return>
    public Task<IEnumerable<DeletionDependency>> GetAllDependenciesAsync(AccountTypeViewModel accountTypeViewModel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all dependencies for a given category asynchronously.
    /// </summary>
    /// <param name="categoryTypeViewModel">The view model representing the category for which dependencies are being retrieved.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <return>A task representing the asynchronous operation. The task result contains an enumerable collection of <see cref="DeletionDependency"/> objects.</return>
    public Task<IEnumerable<DeletionDependency>> GetAllDependenciesAsync(CategoryTypeViewModel categoryTypeViewModel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all deletion dependencies for the specified currency.
    /// </summary>
    /// <param name="currencyViewModel">The currency view model for which dependencies are to be retrieved.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <return>A collection of <see cref="DeletionDependency"/> representing the dependencies associated with the specified currency.</return>
    public Task<IEnumerable<DeletionDependency>> GetAllDependenciesAsync(CurrencyViewModel currencyViewModel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all deletion dependencies associated with the specified account view model asynchronously.
    /// </summary>
    /// <param name="accountViewModel">The account view model whose dependencies are to be retrieved.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a collection of <see cref="DeletionDependency"/> objects indicating the dependencies.</returns>
    public Task<IEnumerable<DeletionDependency>> GetAllDependenciesAsync(AccountViewModel accountViewModel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a random color view model asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <return>A task representing the asynchronous operation. The task result contains a <see cref="Result"/>
    /// indicating the operation's success or failure.</return>
    public Task<ColorViewModel> GetRandomColorViewModel(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all available color view models.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <return>A collection of <see cref="ColorViewModel"/> representing all available colors.</return>
    public Task<IEnumerable<ColorViewModel>> GetAllColorViewModelAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new color based on the provided color model.
    /// </summary>
    /// <param name="colorViewModel">The color view model containing information about the color to be created.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <return>A <see cref="Result{T}"/> containing the created <see cref="ColorViewModel"/> if the operation is successful.</return>
    public Task<Result<ColorViewModel>> CreateColorAsync(ColorViewModel colorViewModel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the specified color data and saves the changes.
    /// </summary>
    /// <param name="colorViewModel">The color view model containing the data to be updated.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Result{ColorViewModel}"/> indicating the outcome of the update operation, including the updated color data if successful.</returns>
    public Task<Result<ColorViewModel>> UpdateColorAsync(ColorViewModel colorViewModel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the place view model associated with the specified place identifier.
    /// </summary>
    /// <param name="placeId">The identifier of the place to retrieve.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <return>A <see cref="PlaceViewModel"/> representing the place, or null if no place is found.</return>
    public Task<PlaceViewModel?> GetPlaceViewModel(int placeId, CancellationToken cancellationToken = default);
}