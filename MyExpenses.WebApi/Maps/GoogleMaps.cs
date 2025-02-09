using Microsoft.Maui.ApplicationModel;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Models.WebApi.Authenticator;
using MyExpenses.SharedUtils.Utils;
using MyExpenses.Utils.Maps;
using NetTopologySuite.Geometries;

namespace MyExpenses.WebApi.Maps;

public static class GoogleMaps
{
    /// <summary>
    /// Converts a TPlace object to a Google Street View URL using its geographic location and the specified project system. An optional zoom level can be provided.
    /// </summary>
    /// <param name="place">The TPlace object containing geographic information to be translated into a Google Street View URL.</param>
    /// <param name="projectSystem">The project system context determining how the URL should be handled (e.g., Wpf or Maui).</param>
    /// <param name="zoomLevel">An optional parameter representing the zoom level for the view. Defaults to 0.</param>
    /// <returns>A string representing the Google Street View URL for the specified TPlace object.</returns>
    public static string ToGoogleStreetView(this TPlace place, ProjectSystem projectSystem, int zoomLevel = 0)
    {
        var point = place.Geometry as Point;
        return point?.ToGoogleStreetView(projectSystem, zoomLevel)!;
    }

    /// <summary>
    /// Converts a point object's geographic location into a Google Street View URL. This method uses the provided project system and an optional zoom level.
    /// </summary>
    /// <param name="point">The TPlace object containing the geographic information to convert into a Google Street View URL.</param>
    /// <param name="projectSystem">Specifies the project system (e.g., Wpf or Maui) to determine how the URL should be handled.</param>
    /// <param name="zoomLevel">An optional zoom level parameter for the Street View. Defaults to 0 if not specified.</param>
    /// <returns>A string representing the Google Street View URL corresponding to the location of the TPlace object.</returns>
    public static string ToGoogleStreetView(this Point point, ProjectSystem projectSystem, int zoomLevel = 0)
    {
        var (xInvariant, yInvariant) = point.ToInvariantCoordinate();

        var url = $"https://www.google.com/maps/@?api=1&map_action=pano&viewpoint={xInvariant}, {yInvariant}&zoom={zoomLevel}";

        switch (projectSystem)
        {
            case ProjectSystem.Wpf:
                url.StartProcess();
                break;
            case ProjectSystem.Maui:
                _ = Launcher.OpenAsync(url);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(projectSystem), projectSystem, null);
        }

        return url;
    }

    /// <summary>
    /// Converts a TPlace object to a Google Maps URL based on its geographic location and the specified project system.
    /// </summary>
    /// <param name="place">The TPlace object containing geographic location information to be converted to a Google Maps URL.</param>
    /// <param name="projectSystem">The project system context, which determines how the URL is handled (e.g., launched or returned).</param>
    /// <returns>A string containing the Google Maps URL corresponding to the geographic location of the TPlace object.</returns>
    public static string ToGoogleMaps(this TPlace place, ProjectSystem projectSystem)
    {
        var point = place.Geometry as Point;
        return point?.ToGoogleMaps(projectSystem)!;
    }

    /// <summary>
    /// Converts a geographic point to a Google Maps URL and optionally launches the URL based on the project system.
    /// </summary>
    /// <param name="point">The geographic point to be converted to a Google Maps URL.</param>
    /// <param name="projectSystem">The project system context, which determines the handling of the URL (e.g., launching the URL or returning it).</param>
    /// <returns>A string containing the Google Maps URL for the specified point.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the provided project system is not supported.</exception>
    public static string ToGoogleMaps(this Point point, ProjectSystem projectSystem)
    {
        var (xInvariant, yInvariant) = point.ToInvariantCoordinate();

        var url = $"https://maps.google.com/maps?q={xInvariant}, {yInvariant}";

        switch (projectSystem)
        {
            case ProjectSystem.Wpf:
                url.StartProcess();
                break;
            case ProjectSystem.Maui:
                _ = Launcher.OpenAsync(url);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(projectSystem), projectSystem, null);
        }

        return url;
    }
}