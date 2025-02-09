using MyExpenses.Models.Sql.Bases.Tables;
using NetTopologySuite.Geometries;

namespace MyExpenses.WebApi.Maps;

public static class LogUtils
{
    /// <summary>
    /// Generates a log message for launching to Google Earth Web, using the location data from a specified place or point.
    /// </summary>
    /// <param name="place">The place object containing location information. Can be null.</param>
    /// <param name="point">The geographical point used if the place object is null.</param>
    /// <returns>A log message indicating the launch to Google Earth Web with the associated location details.</returns>
    public static string GetLogForGoogleEarthWeb(this TPlace? place, Point point)
        => "Google Earth web".GetLogAction(place, point);

    /// <summary>
    /// Generates a log message for launching to Google Maps, using the location data from a specified place or geographical point.
    /// </summary>
    /// <param name="place">The place object containing location information. Can be null.</param>
    /// <param name="point">The geographical point used if the place object is null.</param>
    /// <returns>A log message indicating the launch to Google Maps with the associated location details.</returns>
    public static string GetLogForGoogleMaps(this TPlace? place, Point point)
        => "Google Maps".GetLogAction(place, point);

    /// <summary>
    /// Generates a log message for launching to Google Street View, using the location data from a specified place or point.
    /// </summary>
    /// <param name="place">The place object containing location information. Can be null.</param>
    /// <param name="point">The geographical point used if the place object is null.</param>
    /// <returns>A log message indicating the launch to Google Street View with the associated location details.</returns>
    public static string GetLogForGoogleStreetView(this TPlace? place, Point point)
        => "Google Street View".GetLogAction(place, point);

    /// <summary>
    /// Generates a log message for mapping-related actions, based on a specified action name, location data from a place, or geographical coordinates.
    /// </summary>
    /// <param name="action">The name of the action to be logged, such as "Google Earth Web", "Google Maps", or "Google Street View".</param>
    /// <param name="place">The place object containing location information. Can be null.</param>
    /// <param name="point">The geographical point used if the place object is null.</param>
    /// <returns>A log message describing the specific mapping action and its associated location details.</returns>
    private static string GetLogAction(this string action, TPlace? place, Point point)
    {
        var log = place is not null
            ? $"Launch to {action} at \"{place}\", Latitude={place.Latitude} Longitude={place.Longitude}"
            : $"Launch to {action}, Latitude={point.Y} Longitude={point.X}";

        return log;
    }
}