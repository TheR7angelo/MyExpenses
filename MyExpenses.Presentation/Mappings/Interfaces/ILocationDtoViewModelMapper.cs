using Mapsui;
using Mapsui.Layers;
using Mapsui.Styles;
using MyExpenses.Application.Dtos.Systems;
using MyExpenses.Presentation.ViewModels.Locations;

namespace MyExpenses.Presentation.Mappings.Interfaces;

public interface ILocationDtoViewModelMapper
{
    /// <summary>
    /// The key used to identify the point feature associated with a PlaceViewModel instance.
    /// </summary>
    public string PlaceViewModelPointFeatureKey { get; }

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
    /// Maps a PlaceViewModel instance to a PointFeature.
    /// </summary>
    /// <param name="placeViewModel">The PlaceViewModel to map from.</param>
    /// <param name="imageStyle">Optional ImageStyle to apply to the point feature.</param>
    /// <param name="addLabelStyle">Flag indicating whether to add a label style to the point feature.</param>
    /// <returns>A PointFeature containing the mapped data.</returns>
    public PointFeature MapToPointFeature(PlaceViewModel placeViewModel, ImageStyle? imageStyle = null, bool addLabelStyle = true);

    /// <summary>
    /// Maps a MPoint to a PointFeature.
    /// </summary>
    /// <param name="point">The MPoint to map from.</param>
    /// <param name="imageStyle">Optional ImageStyle for the point feature.</param>
    /// <returns>A PointFeature with the mapped data.</returns>
    public PointFeature MapToPointFeature(MPoint point, ImageStyle? imageStyle = null);

    /// <summary>
    /// Maps a point to a temporary feature.
    /// </summary>
    /// <param name="point">The point to map.</param>
    /// <param name="symbolStyle">Optional symbol style for the temporary feature.</param>
    /// <returns>A TemporaryPointFeature instance representing the mapped point.</returns>
    public TemporaryPointFeature MapToTemporaryFeature(MPoint point, ImageStyle? symbolStyle = null);

    /// <summary>
    /// Maps the given <see cref="PointFeature"/> instance to a <see cref="PlaceViewModel"/> object.
    /// </summary>
    /// <param name="pointFeature">The PointFeature instance containing data to be mapped to a PlaceViewModel.</param>
    /// <returns>A PlaceViewModel representation of the provided PointFeature, or null if the mapping fails.</returns>
    public PlaceViewModel? MapToPlaceViewModel(PointFeature pointFeature);

    /// <summary>
    /// Maps a MPoint instance to a TemporaryPointFeature instance.
    /// </summary>
    /// <param name="point">The MPoint to map from.</param>
    /// <returns>A TemporaryPointFeature containing the mapped data.</returns>
    public TemporaryPointFeature MapToTemporaryPointFeature(MPoint point);

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

    /// <summary>
    /// Calculates the bounding rectangle (MRect) that encloses the given points, with an optional margin added to each side.
    /// </summary>
    /// <param name="points">An array of MPoint instances to calculate the bounding rectangle from.</param>
    /// <param name="margin">An optional margin to add around the bounding rectangle. Default is 10.</param>
    /// <returns>An MRect instance representing the bounding rectangle that contains the points, including the specified margin.</returns>
    public MRect MapToMRect(MPoint[] points, double margin = 10d);

    /// <summary>
    /// Creates an MRect instance that fits the provided collection of points, with an optional margin.
    /// </summary>
    /// <param name="points">The collection of MPoint instances to calculate the bounding rectangle for.</param>
    /// <param name="margin">An optional margin to expand the calculated rectangle by. Defaults to 10.</param>
    /// <returns>An MRect instance that encompasses the given points with the specified margin applied.</returns>
    public MRect MapToMRect(IEnumerable<MPoint> points, double margin = 10d);

    /// <summary>
    /// Converts a PlaceViewModel instance into an MPoint instance.
    /// </summary>
    /// <param name="place">The PlaceViewModel object to be converted.</param>
    /// <returns>An MPoint object representing the geographical coordinates of the provided PlaceViewModel.</returns>
    public MPoint MapToMPoint(PlaceViewModel place);

    public MPoint MapToMPoint((double lon, double lat) coordinates);

    /// <summary>
    /// Merges properties from one PlaceViewModel instance into another.
    /// </summary>
    /// <param name="src">The source PlaceViewModel containing the data to merge.</param>
    /// <param name="dst">The destination PlaceViewModel which will receive the merged data.</param>
    public void Merge(PlaceViewModel src, PlaceViewModel dst);


    /// <summary>
    /// Generates a Google Earth Map URI based on the provided PlaceViewModel and altitude level.
    /// </summary>
    /// <param name="placeViewModel">The PlaceViewModel containing location data.</param>
    /// <param name="altitudeLevel">Optional parameter for the altitude level of the map, default is 200.</param>
    /// <returns>A string representing the Google Earth Map URI.</returns>
    public string GetGoogleHearthMapUri(PlaceViewModel placeViewModel, int altitudeLevel = 200);

    /// <summary>
    /// Generates a Google Earth URI for the specified point and altitude level.
    /// </summary>
    /// <param name="point">The geographic coordinates to generate the URI for.</param>
    /// <param name="altitudeLevel">The altitude level at which the map should be centered. Default is 200 meters.</param>
    /// <returns>A string representing the Google Earth URI.</returns>
    public string GetGoogleHearthMapUri(MPoint point, int altitudeLevel = 200);


    /// <summary>
    /// Generates a Google Maps URI for the given PlaceViewModel.
    /// </summary>
    /// <param name="placeViewModel">The PlaceViewModel representing the location.</param>
    /// <returns>A string containing the Google Maps URI.</returns>
    public string GetGoogleMapsUri(PlaceViewModel placeViewModel);

    /// <summary>
    /// Gets the Google Maps URI for a given point.
    /// </summary>
    /// <param name="point">The point to get the URI for.</param>
    /// <returns>A string representing the Google Maps URI.</returns>
    public string GetGoogleMapsUri(MPoint point);

    /// <summary>
    /// Generates a Google Street View URI for a given PlaceViewModel.
    /// </summary>
    /// <param name="placeViewModel">The PlaceViewModel representing the location.</param>
    /// <param name="zoomLevel">The zoom level of the Street View image, default is 0.</param>
    /// <returns>A string containing the Google Street View URI.</returns>
    public string GetGoogleStreetViewUri(PlaceViewModel placeViewModel, int zoomLevel = 0);

    /// <summary>
    /// Generates a URI for Google Street View based on the provided coordinates and zoom level.
    /// </summary>
    /// <param name="point">The MPoint containing the geographic coordinates.</param>
    /// <param name="zoomLevel">The zoom level for the Google Street View. Default is 0.</param>
    /// <returns>A string representing the URI for accessing Google Street View at the specified location and zoom level.</returns>
    public string GetGoogleStreetViewUri(MPoint point, int zoomLevel = 0);
}