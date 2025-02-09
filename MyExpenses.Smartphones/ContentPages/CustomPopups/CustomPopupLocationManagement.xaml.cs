using MyExpenses.Models.Maui.CustomPopup;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Models.WebApi.Authenticator;
using MyExpenses.SharedUtils.Resources.Resx.LocationManagement;
using MyExpenses.WebApi.Maps;
using Serilog;

namespace MyExpenses.Smartphones.ContentPages.CustomPopups;

public partial class CustomPopupLocationManagement
{
    public static readonly BindableProperty MenuItemHeaderGoogleStreetViewProperty =
        BindableProperty.Create(nameof(MenuItemHeaderGoogleStreetView), typeof(string), typeof(CustomPopupLocationManagement));

    public string MenuItemHeaderGoogleStreetView
    {
        get => (string)GetValue(MenuItemHeaderGoogleStreetViewProperty);
        set => SetValue(MenuItemHeaderGoogleStreetViewProperty, value);
    }

    public static readonly BindableProperty MenuItemHeaderGoogleMapsProperty =
        BindableProperty.Create(nameof(MenuItemHeaderGoogleMaps), typeof(string), typeof(CustomPopupLocationManagement));

    public string MenuItemHeaderGoogleMaps
    {
        get => (string)GetValue(MenuItemHeaderGoogleMapsProperty);
        set => SetValue(MenuItemHeaderGoogleMapsProperty, value);
    }

    public static readonly BindableProperty MenuItemHeaderGoogleEarthWebProperty =
        BindableProperty.Create(nameof(MenuItemHeaderGoogleEarthWeb), typeof(string), typeof(CustomPopupLocationManagement));

    public string MenuItemHeaderGoogleEarthWeb
    {
        get => (string)GetValue(MenuItemHeaderGoogleEarthWebProperty);
        set => SetValue(MenuItemHeaderGoogleEarthWebProperty, value);
    }

    public static readonly BindableProperty MenuItemHeaderDeleteFeatureProperty =
        BindableProperty.Create(nameof(MenuItemHeaderDeleteFeature), typeof(string), typeof(CustomPopupLocationManagement));

    public string MenuItemHeaderDeleteFeature
    {
        get => (string)GetValue(MenuItemHeaderDeleteFeatureProperty);
        set => SetValue(MenuItemHeaderDeleteFeatureProperty, value);
    }

    public static readonly BindableProperty MenuItemHeaderEditFeatureProperty =
        BindableProperty.Create(nameof(MenuItemHeaderEditFeature), typeof(string), typeof(CustomPopupLocationManagement));

    public string MenuItemHeaderEditFeature
    {
        get => (string)GetValue(MenuItemHeaderEditFeatureProperty);
        set => SetValue(MenuItemHeaderEditFeatureProperty, value);
    }

    public static readonly BindableProperty MenuItemHeaderPointProperty =
        BindableProperty.Create(nameof(MenuItemHeaderAddPoint), typeof(string), typeof(CustomPopupLocationManagement));

    public string MenuItemHeaderAddPoint
    {
        get => (string)GetValue(MenuItemHeaderPointProperty);
        set => SetValue(MenuItemHeaderPointProperty, value);
    }

    public MenuItemVisibility MenuItemVisibility { get; }
    private NetTopologySuite.Geometries.Point Point { get; }
    private TPlace? Place { get; }

    public CustomPopupLocationManagement(MenuItemVisibility menuItemVisibility, NetTopologySuite.Geometries.Point point, TPlace? place)
    {
        MenuItemVisibility = menuItemVisibility;
        Point = point;
        Place = place;

        UpdateLanguage();
        InitializeComponent();
    }

    private void UpdateLanguage()
    {
        MenuItemHeaderAddPoint = LocationManagementResources.MenuItemHeaderAddPoint;
        MenuItemHeaderEditFeature = LocationManagementResources.MenuItemHeaderEditFeature;
        MenuItemHeaderDeleteFeature = LocationManagementResources.MenuItemHeaderDeleteFeature;
        MenuItemHeaderGoogleStreetView = LocationManagementResources.MenuItemHeaderGoogleStreetView;
        MenuItemHeaderGoogleMaps = LocationManagementResources.MenuItemHeaderGoogleMaps;
        MenuItemHeaderGoogleEarthWeb = LocationManagementResources.MenuItemHeaderGoogleEarthWeb;
    }

    private void ButtonGoogleEarthWeb_OnClicked(object? sender, EventArgs e)
    {
        var log = Place.GetLogForGoogleEarthWeb(Point);

        Log.Information("{Log}", log);
        var uri = Point.ToGoogleEarthWeb(ProjectSystem.Maui);
        Log.Information("{Uri}", uri);
    }

    private void ButtonToGoogleMaps_OnClick(object sender, EventArgs e)
    {
        var log = Place.GetLogForGoogleMaps(Point);

        Log.Information("{Log}", log);
        var uri = Point.ToGoogleMaps(ProjectSystem.Maui);
        Log.Information("{Uri}", uri);
    }

    private void ButtonToGoogleStreetView_OnClick(object sender, EventArgs e)
    {
        var log = Place.GetLogForGoogleStreetView(Point);

        Log.Information("{Log}", log);
        var uri = Point.ToGoogleStreetView(ProjectSystem.Maui);
        Log.Information("{Uri}", uri);
    }
}