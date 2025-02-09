using MyExpenses.Models.Maui.CustomPopup;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Models.WebApi.Authenticator;
using MyExpenses.WebApi.Maps;
using Serilog;

namespace MyExpenses.Smartphones.ContentPages.CustomPopups;

public partial class CustomPopupLocationManagement
{
    public MenuItemVisibility MenuItemVisibility { get; }
    private NetTopologySuite.Geometries.Point Point { get; }
    private TPlace? Place { get; }

    public CustomPopupLocationManagement(MenuItemVisibility menuItemVisibility, NetTopologySuite.Geometries.Point point, TPlace? place)
    {
        MenuItemVisibility = menuItemVisibility;
        Point = point;
        Place = place;

        InitializeComponent();
    }

    private void ButtonGoogleEarthWeb_OnClicked(object? sender, EventArgs e)
    {
        var log = GetLogAction("Google Earth web");

        Log.Information("{Log}", log);
        var uri = Point.ToGoogleEarthWeb(ProjectSystem.Maui);
        Log.Information("{Uri}", uri);
    }

    private void ButtonToGoogleMaps_OnClick(object sender, EventArgs e)
    {
        var log = GetLogAction("Google Maps");

        Log.Information("{Log}", log);
        var uri = Point.ToGoogleMaps(ProjectSystem.Maui);
        Log.Information("{Uri}", uri);
    }

    private void ButtonToGoogleStreetView_OnClick(object sender, EventArgs e)
    {
        var log = GetLogAction("Google Street View");

        Log.Information("{Log}", log);
        var uri = Point.ToGoogleStreetView(ProjectSystem.Maui);
        Log.Information("{Uri}", uri);
    }

    private string GetLogAction(string action)
    {
        var log = Place is not null
            ? $"Launch to {action} at \"{Place}\", Latitude={Place.Latitude} Longitude={Place.Longitude}"
            : $"Launch to {action}, Latitude={Point.Y} Longitude={Point.X}";

        return log;
    }
}