using BruTile.Predefined;
using Domain.Models.Validation;
using Mapsui;
using Mapsui.Layers;
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
    /// Retrieves a collection of all places as view models.
    /// </summary>
    /// <param name="cancellationToken">
    /// A cancellation token that can be used to signal the operation should be canceled.
    /// </param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a <see cref="Result{T}"/>
    /// with an enumerable of <see cref="PlaceViewModel"/> if successful, or an error code and message otherwise.
    /// </returns>
    public Task<Result<IEnumerable<PlaceViewModel>>> GetAllPlaces(CancellationToken cancellationToken = default);

    /// <summary>
    /// Groups a collection of places into country-specific groupings.
    /// </summary>
    /// <param name="placeViewModels">
    /// A collection of <see cref="PlaceViewModel"/> instances representing the places to be grouped.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> that can be used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation, containing an enumerable of <see cref="CountryGroupViewModel"/>
    /// with grouped place data.
    /// </returns>
    public IEnumerable<CountryGroupViewModel> GetAllPlaceGroup(IEnumerable<PlaceViewModel> placeViewModels, CancellationToken cancellationToken = default);

    /// <summary>
    /// Maps a Nominatim search result to a PointFeature.
    /// </summary>
    /// <param name="currentSearchResult">The current search result from Nominatim.</param>
    /// <returns>
    /// A <see cref="PointFeature"/> representing the mapped point or an empty point if the GeoJson center is null.
    /// </returns>
    public PointFeature MapToPointFeature(NominatimSearchResultViewModel currentSearchResult);

    /// <summary>
    /// Creates a new place asynchronously using the provided place view model.
    /// </summary>
    /// <param name="placeViewModel">The view model containing the details of the place to be created.</param>
    /// <param name="cancellationToken">A token that allows for cancellation of the operation.</param>
    /// <returns>A <see cref="Result{PlaceViewModel}"/> indicating the success or failure of the operation, along with the newly created place if successful.</returns>
    public Task<Result<PlaceViewModel>> CreatePlaceAsync(PlaceViewModel placeViewModel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously updates an existing place with the provided details.
    /// </summary>
    /// <param name="placeViewModel">The view model containing updated details of the place.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A <see cref="Result{PlaceViewModel}"/> indicating success or failure along with an optional error message and the updated place ViewModel.</returns>
    public Task<Result<PlaceViewModel>> UpdatePlaceAsync(PlaceViewModel placeViewModel, CancellationToken cancellationToken = default);
}