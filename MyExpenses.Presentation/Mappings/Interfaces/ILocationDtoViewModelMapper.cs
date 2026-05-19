using Mapsui.Layers;
using Mapsui.Styles;
using MyExpenses.Application.Dtos.Systems;
using MyExpenses.Presentation.ViewModels.Locations;

namespace MyExpenses.Presentation.Mappings.Interfaces;

public interface ILocationDtoViewModelMapper
{
    /// <summary>
    /// Maps a PlaceViewModel instance to a PlaceDto instance.
    /// </summary>
    /// <param name="src">The PlaceViewModel to map from.</param>
    /// <returns>A PlaceDto containing the mapped data.</returns>
    public PlaceDto MapToDto(PlaceViewModel src);

    /// <summary>
    /// Maps a place DTO to a place view model.
    /// </summary>
    /// <param name="src">The place DTO to map from.</param>
    /// <returns>A place view model containing the mapped data.</returns>
    public PlaceViewModel MapToViewModel(PlaceDto src);

    /// <summary>
    /// Maps the given <see cref="PlaceViewModel"/> instance to a <see cref="PointFeature"/> object.
    /// </summary>
    /// <param name="placeViewModel">The PlaceViewModel instance containing location data to be mapped.</param>
    /// <param name="imageStyle">An optional ImageStyle to be applied to the PointFeature.</param>
    /// <returns>A PointFeature representation of the provided PlaceViewModel.</returns>
    public PointFeature MapToPointFeature(PlaceViewModel placeViewModel, ImageStyle? imageStyle = null);

    /// <summary>
    /// Converts the specified <see cref="PlaceViewModel"/> instance into a <see cref="TemporaryPointFeature"/> object.
    /// </summary>
    /// <param name="place">The PlaceViewModel instance containing the location data to be transformed.</param>
    /// <param name="symbolStyle">An optional ImageStyle to be applied to the TemporaryPointFeature.</param>
    /// <returns>A TemporaryPointFeature representation of the provided PlaceViewModel.</returns>
    public TemporaryPointFeature ToTemporaryFeature(PlaceViewModel place, ImageStyle? symbolStyle = null);

    /// <summary>
    /// Maps the given <see cref="PointFeature"/> instance to a <see cref="PlaceViewModel"/> object.
    /// </summary>
    /// <param name="pointFeature">The PointFeature instance containing data to be mapped to a PlaceViewModel.</param>
    /// <returns>A PlaceViewModel representation of the provided PointFeature, or null if the mapping fails.</returns>
    public PlaceViewModel? MapToPlaceViewModel(PointFeature pointFeature);

    /// <summary>
    /// Maps the given coordinates to a <see cref="PointFeature"/> object.
    /// </summary>
    /// <param name="coordinates">A tuple containing the x and y coordinates to be mapped.</param>
    /// <returns>A PointFeature representation of the provided coordinates.</returns>
    public PointFeature MapToPointFeature((double x, double y) coordinates);

    /// <summary>
    /// Maps a collection of PlaceViewModel instances to a collection of CountryGroupViewModel instances.
    /// </summary>
    /// <param name="placeViewModels">The collection of PlaceViewModel objects to map from.</param>
    /// <returns>A collection of CountryGroupViewModel objects containing the grouped and mapped data.</returns>
    public IEnumerable<CountryGroupViewModel> MapToGroup(IEnumerable<PlaceViewModel> placeViewModels);
}