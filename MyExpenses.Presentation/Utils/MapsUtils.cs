using Mapsui;
using Microsoft.Extensions.Logging;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.Services.Interfaces;

namespace MyExpenses.Presentation.Utils;

/// <summary>
/// Provides utility methods for interacting with mapping services such as Google Earth and Google Maps.
/// </summary>
public class MapsUtils(
    ILogger<MapsUtils> logger,
    INavigationWindowService navigationWindowService,
    ILocationDtoViewModelMapper locationDtoViewModelMapper)
{
    /// <summary>
    /// Navigates to Google Earth Web at the specified location and logs the action.
    /// </summary>
    /// <param name="point">The MPoint containing the geographic coordinates for the desired location.</param>
    public void GoToGoogleEarthWeb(MPoint point)
    {
        var uri = locationDtoViewModelMapper.GetGoogleHearthMapUri(point);
        OpenWebUriWithLog(uri, "Google Earth");
    }

    /// <summary>
    /// Navigates to Google Maps at the specified location and logs the action.
    /// </summary>
    /// <param name="point">The MPoint containing the geographic coordinates for the desired location.</param>
    public void GoToGoogleMaps(MPoint point)
    {
        var uri = locationDtoViewModelMapper.GetGoogleMapsUri(point);
        OpenWebUriWithLog(uri, "Google Maps");
    }

    /// <summary>
    /// Navigates to Google Street View at the specified location and logs the action.
    /// </summary>
    /// <param name="point">The MPoint containing the geographic coordinates for the desired location.</param>
    public void GoToGoogleStreetView(MPoint point)
    {
        var uri = locationDtoViewModelMapper.GetGoogleStreetViewUri(point);
        OpenWebUriWithLog(uri, "Google Street View");
    }

    /// <summary>
    /// Opens the specified web URI and logs the action.
    /// </summary>
    /// <param name="uri">The web URI to be opened.</param>
    /// <param name="webPageTitle">The title of the web page being accessed, used for logging purposes.</param>
    private void OpenWebUriWithLog(string uri, string webPageTitle)
    {
        if (logger.IsEnabled(LogLevel.Information)) logger.LogInformation("Opening for {WebPageTitle} with the following url: {Uri}", webPageTitle, uri);
        navigationWindowService.OpenUri(uri);
    }
}