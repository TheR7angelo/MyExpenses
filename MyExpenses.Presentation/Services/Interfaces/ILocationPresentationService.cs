using BruTile.Predefined;
using Domain.Models.Validation;
using Mapsui;
using Mapsui.Styles;
using MyExpenses.Presentation.ViewModels.Locations;

namespace MyExpenses.Presentation.Services.Interfaces;

public interface ILocationPresentationService
{
    /// <summary>
    /// Retrieves a collection of all known tile sources, excluding the ones present in a predefined blacklist.
    /// </summary>
    /// <returns>
    /// A <see cref="Result{T}"/> containing an enumerable of <see cref="KnownTileSource"/> if successful,
    /// or an error code and message otherwise.
    /// </returns>
    public Result<IEnumerable<KnownTileSource>> GetAllKnowTitleSource();

    /// <summary>
    /// Retrieves the default map configuration based on provided parameters, such as whether additional widgets are enabled and the desired background color.
    /// </summary>
    /// <param name="widgetEnabled">Indicates whether widgets are enabled on the map view.</param>
    /// <param name="backgroundColor">Specifies the background color to be applied to the map. If null, default background color is used.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing a <see cref="Map"/> instance if successful,
    /// or an error code and message otherwise.
    /// </returns>
    public Result<Map> GetDefaultMap(bool widgetEnabled, Color? backgroundColor = null);

    /// <summary>
    /// Retrieves a <see cref="PlaceViewModel"/> associated with the specified place ID.
    /// </summary>
    /// <param name="placeId">The unique identifier of the place for which the view model is being retrieved.</param>
    /// <param name="cancellationToken">An optional cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A <see cref="Task"/> that represents the asynchronous operation.
    /// The task result contains the <see cref="PlaceViewModel"/> if found, or <c>null</c> if no matching place is available.
    /// </returns>
    public Task<PlaceViewModel?> GetPlaceViewModel(int placeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a collection of all place groups available for mapping purposes.
    /// </summary>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests during the operation.
    /// </param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> containing a <see cref="Result{T}"/> with an enumerable of
    /// <see cref="CountryGroupViewModel"/> if successful, or an error code and message otherwise.
    /// </returns>
    public Task<Result<IEnumerable<CountryGroupViewModel>>> GetAllPlaceGroup(CancellationToken cancellationToken = default);
}