using Microsoft.Maui.ApplicationModel;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Models.WebApi.Authenticator;
using MyExpenses.SharedUtils.Utils;
using MyExpenses.Utils.Maps;
using NetTopologySuite.Geometries;

namespace MyExpenses.WebApi.Maps;

public static class GoogleEarth
{
    /// <summary>
    /// Generates a Google Earth Web URL for the provided TPlace object, allowing it to be opened in a specific project system.
    /// </summary>
    /// <param name="place">The geographical place object (TPlace) to generate the Google Earth Web URL for.</param>
    /// <param name="projectSystem">The project system that determines how the URL should be handled (e.g., WPF or MAUI).</param>
    /// <param name="altitudeLevel">The altitude level to use in the URL, defaulting to 200 if not specified.</param>
    /// <returns>The generated Google Earth Web URL as a string, or null if the place's geometry is not a valid point.</returns>
    public static string ToGoogleEarthWeb(this TPlace place, ProjectSystem projectSystem, int altitudeLevel = 200)
    {
        var point = place.Geometry as Point;
        return point?.ToGoogleEarthWeb(projectSystem, altitudeLevel)!;
    }

    /// <summary>
    /// Generates a Google Earth Web URL for the provided Point object, allowing it to be opened in either a WPF or MAUI project system.
    /// </summary>
    /// <param name="point">The geographical point to generate the Google Earth Web URL for.</param>
    /// <param name="projectSystem">The project system that determines how the URL should be handled (WPF or MAUI).</param>
    /// <param name="altitudeLevel">The altitude level to use in the URL, defaulting to 200 if not specified.</param>
    /// <returns>The generated Google Earth Web URL as a string.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the provided project system is not supported.</exception>
    public static string ToGoogleEarthWeb(this Point point, ProjectSystem projectSystem, int altitudeLevel = 200)
    {
        var (xInvariant, yInvariant) = point.ToInvariantCoordinate();

        var googleEarthUrl = $"https://earth.google.com/web/@{xInvariant},{yInvariant},{altitudeLevel}a,0d,30y,0h,0t,0r";

        switch (projectSystem)
        {
            case ProjectSystem.Wpf:
                googleEarthUrl.StartProcess();
                break;
            case ProjectSystem.Maui:
                _ = Launcher.OpenAsync(googleEarthUrl);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(projectSystem), projectSystem, null);
        }

        return googleEarthUrl;
    }
}