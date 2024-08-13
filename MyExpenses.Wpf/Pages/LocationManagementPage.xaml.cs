using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using BruTile.Predefined;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Tiling.Layers;
using Microsoft.Data.Sqlite;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Groups;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using MyExpenses.Utils.Collection;
using MyExpenses.Utils.Maps;
using MyExpenses.WebApi.Maps;
using MyExpenses.Wpf.Resources.Resx.Pages.LocationManagementPage;
using MyExpenses.Wpf.Utils.Maps;
using MyExpenses.Wpf.Windows.LocationManagementWindows;
using MyExpenses.Wpf.Windows.MsgBox;
using Serilog;

namespace MyExpenses.Wpf.Pages;

public partial class LocationManagementPage
{
    public static readonly DependencyProperty ComboBoxBasemapHintAssistProperty =
        DependencyProperty.Register(nameof(ComboBoxBasemapHintAssist), typeof(string), typeof(LocationManagementPage),
            new PropertyMetadata(default(string)));

    public object ComboBoxBasemapHintAssist
    {
        get => (string)GetValue(ComboBoxBasemapHintAssistProperty);
        set => SetValue(ComboBoxBasemapHintAssistProperty, value);
    }

    public static readonly DependencyProperty MenuItemHeaderAddPointProperty =
        DependencyProperty.Register(nameof(MenuItemHeaderAddPoint), typeof(string), typeof(LocationManagementPage),
            new PropertyMetadata(default(string)));

    public string MenuItemHeaderAddPoint
    {
        get => (string)GetValue(MenuItemHeaderAddPointProperty);
        set => SetValue(MenuItemHeaderAddPointProperty, value);
    }

    public static readonly DependencyProperty MenuItemHeaderEditFeatureProperty =
        DependencyProperty.Register(nameof(MenuItemHeaderEditFeature), typeof(string), typeof(LocationManagementPage),
            new PropertyMetadata(default(string)));

    public string MenuItemHeaderEditFeature
    {
        get => (string)GetValue(MenuItemHeaderEditFeatureProperty);
        set => SetValue(MenuItemHeaderEditFeatureProperty, value);
    }

    public static readonly DependencyProperty MenuItemHeaderDeleteFeatureProperty =
        DependencyProperty.Register(nameof(MenuItemHeaderDeleteFeature), typeof(string), typeof(LocationManagementPage),
            new PropertyMetadata(default(string)));

    public string MenuItemHeaderDeleteFeature
    {
        get => (string)GetValue(MenuItemHeaderDeleteFeatureProperty);
        set => SetValue(MenuItemHeaderDeleteFeatureProperty, value);
    }

    public static readonly DependencyProperty MenuItemHeaderMapsProperty =
        DependencyProperty.Register(nameof(MenuItemHeaderMaps), typeof(string), typeof(LocationManagementPage),
            new PropertyMetadata(default(string)));

    public string MenuItemHeaderMaps
    {
        get => (string)GetValue(MenuItemHeaderMapsProperty);
        set => SetValue(MenuItemHeaderMapsProperty, value);
    }

    public static readonly DependencyProperty MenuItemHeaderGoogleEarthWebProperty =
        DependencyProperty.Register(nameof(MenuItemHeaderGoogleEarthWeb), typeof(string),
            typeof(LocationManagementPage), new PropertyMetadata(default(string)));

    public string MenuItemHeaderGoogleEarthWeb
    {
        get => (string)GetValue(MenuItemHeaderGoogleEarthWebProperty);
        set => SetValue(MenuItemHeaderGoogleEarthWebProperty, value);
    }

    public static readonly DependencyProperty MenuItemHeaderGoogleMapsProperty =
        DependencyProperty.Register(nameof(MenuItemHeaderGoogleMaps), typeof(string), typeof(LocationManagementPage),
            new PropertyMetadata(default(string)));

    public string MenuItemHeaderGoogleMaps
    {
        get => (string)GetValue(MenuItemHeaderGoogleMapsProperty);
        set => SetValue(MenuItemHeaderGoogleMapsProperty, value);
    }

    public static readonly DependencyProperty MenuItemHeaderGoogleStreetViewProperty =
        DependencyProperty.Register(nameof(MenuItemHeaderGoogleStreetView), typeof(string),
            typeof(LocationManagementPage), new PropertyMetadata(default(string)));

    public string MenuItemHeaderGoogleStreetView
    {
        get => (string)GetValue(MenuItemHeaderGoogleStreetViewProperty);
        set => SetValue(MenuItemHeaderGoogleStreetViewProperty, value);
    }

    public ObservableCollection<CountryGroup> CountryGroups { get; }
    public List<KnownTileSource> KnownTileSources { get; }
    public KnownTileSource KnownTileSourceSelected { get; set; }

    private WritableLayer PlaceLayer { get; } = new() { Style = null, IsMapInfoLayer = true, Tag = typeof(TPlace) };

    private TPlace? ClickTPlace { get; set; }
    private NetTopologySuite.Geometries.Point ClickPoint { get; set; } = NetTopologySuite.Geometries.Point.Empty;
    private PointFeature? PointFeature { get; set; }

    public LocationManagementPage()
    {
        KnownTileSources = [..MapsuiMapExtensions.GetAllKnowTileSource()];

        using var context = new DataBaseContext();
        var places = context.TPlaces.OrderBy(s => s.Country).ThenBy(s => s.City).ThenBy(s => s.Name).ToList();
        var groups = places.GetGroups();

        CountryGroups = [..groups];

        var features = places
            .Where(s => s.Latitude is not null && s.Latitude is not 0 && s.Longitude is not null &&
                        s.Longitude is not 0)
            .Select(feature => feature.IsOpen is true
                ? feature.ToFeature(MapsuiStyleExtensions.RedMarkerStyle)
                : feature.ToFeature(MapsuiStyleExtensions.BlueMarkerStyle));

        PlaceLayer.AddRange(features);

        var backColor = Utils.Resources.GetMaterialDesignPaperMapsUiStylesColor();
        var map = MapsuiMapExtensions.GetMap(true, backColor);
        map.Layers.Add(PlaceLayer);

        Interface.ThemeChanged += Interface_OnThemeChanged;
        Interface.LanguageChanged += Interface_OnLanguageChanged;
        UpdateLanguage();

        InitializeComponent();

        MapControl.Map = map;

        SetInitialZoom();
    }

    #region Action

    private void CheckBoxPlaceIsOpen_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is not ToggleButton checkBox) return;
        if (checkBox.DataContext is not TPlace place) return;

        if (place.Longitude is null || place.Longitude == 0 || place.Latitude is null || place.Latitude == 0) return;

        var pointFeature = place.ToFeature().Point;
        SetZoom(pointFeature);
    }

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
        => UpdateLanguage();

    private void Interface_OnThemeChanged(object sender, ConfigurationThemeChangedEventArgs e)
        => UpdateMapBackColor();

    private void MapControl_OnLoaded(object sender, RoutedEventArgs e)
        => UpdateTileLayer();

    private void MapControl_OnContextMenuOpening(object sender, ContextMenuEventArgs e)
    {
        var screenPosition = Mouse.GetPosition(MapControl);
        var worldPosition = MapControl.Map.Navigator.Viewport.ScreenToWorld(screenPosition.X, screenPosition.Y);

        var lonLat = SphericalMercator.ToLonLat(worldPosition.X, worldPosition.Y);
        ClickPoint = new NetTopologySuite.Geometries.Point(lonLat.lat, lonLat.lon);

        var mPoint = new MPoint(screenPosition.X, screenPosition.Y);
        var mapInfo = MapControl.GetMapInfo(mPoint);
        SetClickTPlace(mapInfo!);
    }

    private void MapControl_OnInfo(object? sender, MapInfoEventArgs e)
    {
        var mapInfo = e.MapInfo!;
        SetClickTPlace(mapInfo);
    }

    private void MenuItemAddFeature_OnClick(object sender, RoutedEventArgs e)
    {
        var addEditLocationWindow = new AddEditLocationWindow();
        addEditLocationWindow.SetPlace(ClickPoint);
        addEditLocationWindow.ShowDialog();

        if (addEditLocationWindow.DialogResult is not true) return;

        var newPlace = addEditLocationWindow.Place;
        ProcessNewPlace(newPlace, add: true);
        AddPlaceTreeViewCountryGroup(newPlace);
    }

    private void MenuItemDeleteFeature_OnClick(object sender, RoutedEventArgs e)
    {
        var feature = PointFeature;
        if (feature is null) return;

        var placeToDelete = feature.ToTPlace();
        var response =
            MsgBox.Show(string.Format(LocationManagementPageResources.MessageBoxDeleteQuestion, placeToDelete.Name),
                MsgBoxImage.Question, MessageBoxButton.YesNoCancel);
        if (response is not MessageBoxResult.Yes) return;

        Log.Information("Attempting to remove the place \"{PlaceToDeleteName}\"", placeToDelete.Name);
        PlaceLayer.TryRemove(feature);

        var (success, exception) = placeToDelete.Delete();

        if (success)
        {
            MapControl.Refresh();

            Log.Information("Place was successfully removed");
            MsgBox.Show(LocationManagementPageResources.MessageBoxMenuItemDeleteFeatureNoUseSuccess, MsgBoxImage.Check);

            return;
        }

        if (exception!.InnerException is SqliteException
            {
                SqliteExtendedErrorCode: SQLitePCL.raw.SQLITE_CONSTRAINT_FOREIGNKEY
            })
        {
            Log.Error("Foreign key constraint violation");

            response =
                MsgBox.Show(LocationManagementPageResources.MessageBoxMenuItemDeleteFeatureUseQuestion,
                    MsgBoxImage.Question, MessageBoxButton.YesNoCancel);

            if (response is not MessageBoxResult.Yes) return;

            Log.Information("Attempting to remove the place \"{PlaceToDeleteName}\" with all relative element",
                placeToDelete.Name);
            placeToDelete.Delete(true);
            Log.Information("Place and all relative element was successfully removed");
            MsgBox.Show(LocationManagementPageResources.MessageBoxMenuItemDeleteFeatureUseSuccess, MsgBoxImage.Check);

            RemovePlaceTreeViewCountryGroup(placeToDelete);

            MapControl.Refresh();

            return;
        }

        Log.Error(exception, "An error occurred please retry");
        MsgBox.Show(LocationManagementPageResources.MessageBoxMenuItemDeleteFeatureError, MsgBoxImage.Error);
    }

    private void MenuItemEditFeature_OnClick(object sender, RoutedEventArgs e)
    {
        var addEditLocationWindow = new AddEditLocationWindow();
        addEditLocationWindow.SetPlace(ClickTPlace!, false);
        addEditLocationWindow.ShowDialog();

        if (addEditLocationWindow.DialogResult is not true) return;

        var editedPlace = addEditLocationWindow.Place;
        ProcessNewPlace(editedPlace, edit: true);

        RemovePlaceTreeViewCountryGroup(editedPlace);
        AddPlaceTreeViewCountryGroup(editedPlace);
    }

    private void MenuItemToGoogleEarthWeb_OnClick(object sender, RoutedEventArgs e)
    {
        var log = GetLogAction("Google Earth web");

        Log.Information("{Log}", log);
        ClickPoint.ToGoogleEarthWeb();
    }


    private void MenuItemToGoogleMaps_OnClick(object sender, RoutedEventArgs e)
    {
        var log = GetLogAction("Google Maps");

        Log.Information("{Log}", log);
        ClickPoint.ToGoogleMaps();
    }

    private void MenuItemToGoogleStreetView_OnClick(object sender, RoutedEventArgs e)
    {
        var log = GetLogAction("Google Street View");

        Log.Information("{Log}", log);
        ClickPoint.ToGoogleStreetView();
    }

    private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        => UpdateTileLayer();

    private void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        if (sender is not TreeView treeView) return;

        var points = new List<MPoint>();
        switch (treeView.SelectedItem)
        {
            case CountryGroup countryGroup:
                var ps1 = countryGroup.CityGroups?
                    .SelectMany(s => s.Places!)
                    .Where(s => s.Geometry?.X is not 0 && s.Geometry?.Y is not 0)
                    .Select(s => s.ToMPoint());
                if (ps1 is not null) points.AddRange(ps1);
                break;

            case CityGroup cityGroup:
                var ps2 = cityGroup.Places?
                    .Where(s => s.Geometry?.X is not 0 && s.Geometry?.Y is not 0)
                    .Select(s => s.ToMPoint());
                if (ps2 is not null) points.AddRange(ps2);
                break;
        }

        SetZoom(points.ToArray());
    }

    #endregion

    #region Function

    private void AddPlaceTreeViewCountryGroup(TPlace placeToAdd)
    {
        var cityGroup = CountryGroups.FirstOrDefault(s => s.Country == placeToAdd.Country)?.CityGroups
            ?.FirstOrDefault(s => s.City == placeToAdd.City);

        if (cityGroup is null)
        {
            var newCityGroup = new CityGroup { City = placeToAdd.City, Places = [placeToAdd] };

            var countryGroup = CountryGroups.FirstOrDefault(s => s.Country == placeToAdd.Country);
            if (countryGroup is null)
            {
                var newGroupCountry = new CountryGroup { Country = placeToAdd.Country, CityGroups = [newCityGroup] };
                CountryGroups.AddAndSort(newGroupCountry, s => s.Country ?? string.Empty);
            }
            else
            {
                countryGroup.CityGroups?.AddAndSort(newCityGroup, s => s.City ?? string.Empty);
            }
        }
        else
        {
            cityGroup.Places?.AddAndSort(placeToAdd, s => s.Name ?? string.Empty);
        }
    }

    private string GetLogAction(string action)
    {
        var log = ClickTPlace is not null
            ? $"Launch to {action} at \"{ClickTPlace}\", Latitude={ClickTPlace.Latitude} Longitude={ClickTPlace.Longitude}"
            : $"Launch to {action}, Latitude={ClickPoint.Y} Longitude={ClickPoint.X}";

        return log;
    }

    private void ProcessNewPlace(TPlace newPlace, bool add = false, bool edit = false)
    {
        var (success, _) = newPlace.AddOrEdit();
        if (success)
        {
            var feature = newPlace.IsOpen is true
                ? newPlace.ToFeature(MapsuiStyleExtensions.RedMarkerStyle)
                : newPlace.ToFeature(MapsuiStyleExtensions.BlueMarkerStyle);

            PlaceLayer.TryRemove(PointFeature!);
            PlaceLayer.Add(feature);
            MapControl.Refresh();

            // string json;
            switch (add)
            {
                case true when !edit:
                    MsgBox.Show(LocationManagementPageResources.MessageBoxProcessNewPlaceAddSuccess, MsgBoxImage.Check);

                    Log.Information("The new place was successfully added");

                    // Loop crash
                    // json = newPlace.ToJsonString();
                    // Log.Information("{Json}", json);

                    break;
                case false when edit:
                    MsgBox.Show(LocationManagementPageResources.MessageBoxProcessNewPlaceEditSuccess,
                        MsgBoxImage.Check);

                    Log.Information("The new place was successfully edited");

                    // Loop crash
                    // json = newPlace.ToJsonString();
                    // Log.Information("{Json}", json);

                    break;
            }
        }
        else MsgBox.Show(LocationManagementPageResources.MessageBoxProcessNewPlaceError, MsgBoxImage.Error);
    }

    private void RemovePlaceTreeViewCountryGroup(TPlace placeToDelete)
    {
        var countryToRemove = CountryGroups
            .FirstOrDefault(countryGroup => countryGroup.CityGroups is not null &&
                                            countryGroup.CityGroups.Any(cityGroup => cityGroup.Places is not null &&
                                                cityGroup.Places.Any(place => place.Id == placeToDelete.Id)));

        var cityToRemove = countryToRemove?
            .CityGroups?.FirstOrDefault(cityGroup => cityGroup.Places is not null &&
                                                     cityGroup.Places.Any(place => place.Id == placeToDelete.Id));

        var placeToRemove = cityToRemove?.Places?
            .FirstOrDefault(place => place.Id == placeToDelete.Id);
        if (placeToRemove is null) return;

        cityToRemove?.Places?.Remove(placeToRemove);

        if (cityToRemove?.Places?.Count == 0) countryToRemove?.CityGroups?.Remove(cityToRemove);

        if (countryToRemove?.CityGroups?.Count == 0) CountryGroups.Remove(countryToRemove);
    }

    private void SetClickTPlace(MapInfo mapInfo)
    {
        var feature = mapInfo.Feature as PointFeature;
        var layer = mapInfo.Layer;

        if (feature is null || layer is null)
        {
            MenuItemAddFeature.Visibility = Visibility.Visible;
            MenuItemEditFeature.Visibility = Visibility.Collapsed;
            MenuItemDeleteFeature.Visibility = Visibility.Collapsed;
            ClickTPlace = null;
            return;
        }

        MenuItemAddFeature.Visibility = Visibility.Collapsed;
        MenuItemEditFeature.Visibility = Visibility.Visible;
        MenuItemDeleteFeature.Visibility = Visibility.Visible;

        var type = (Type)layer.Tag!;
        if (type != typeof(TPlace)) return;

        PointFeature = feature;
        var place = feature.ToTPlace();
        ClickTPlace = place;
    }

    private void SetZoom(params MPoint[] points)
    {
        switch (points.Length)
        {
            case 0:
                break;
            case 1:
                MapControl.Map.Navigator.CenterOn(points[0]);
                MapControl.Map.Navigator.ZoomTo(1);
                break;

            case > 1:
                double minX = points.Min(p => p.X), maxX = points.Max(p => p.X);
                double minY = points.Min(p => p.Y), maxY = points.Max(p => p.Y);

                var width = maxX - minX;
                var height = maxY - minY;

                const double marginPercentage = 10; // Change this value to suit your needs
                var marginX = width * marginPercentage / 100;
                var marginY = height * marginPercentage / 100;

                var mRect = new MRect(minX - marginX, minY - marginY, maxX + marginX, maxY + marginY);

                MapControl.Map.Navigator.ZoomToBox(mRect);
                break;
        }
    }

    private void SetInitialZoom()
    {
        var points = PlaceLayer.GetFeatures().Select(s => ((PointFeature)s).Point).ToList();

        switch (points.Count)
        {
            case 0:
                break;
            case 1:
                MapControl.Map.Home = navigator =>
                {
                    navigator.CenterOn(points[0]);
                    navigator.ZoomTo(1);
                };
                break;
            case > 1:
                double minX = points.Min(p => p.X), maxX = points.Max(p => p.X);
                double minY = points.Min(p => p.Y), maxY = points.Max(p => p.Y);

                var width = maxX - minX;
                var height = maxY - minY;

                const double marginPercentage = 10; // Change this value to suit your needs
                var marginX = width * marginPercentage / 100;
                var marginY = height * marginPercentage / 100;

                var mRect = new MRect(minX - marginX, minY - marginY, maxX + marginX, maxY + marginY);

                MapControl.Map.Home = navigator => { navigator.ZoomToBox(mRect); };
                break;
        }
    }

    private void UpdateLanguage()
    {
        ComboBoxBasemapHintAssist = LocationManagementPageResources.ComboBoxBasemapHintAssist;

        MenuItemHeaderAddPoint = LocationManagementPageResources.MenuItemHeaderAddPoint;
        MenuItemHeaderEditFeature = LocationManagementPageResources.MenuItemHeaderEditFeature;
        MenuItemHeaderDeleteFeature = LocationManagementPageResources.MenuItemHeaderDeleteFeature;

        MenuItemHeaderMaps = LocationManagementPageResources.MenuItemHeaderMaps;
        MenuItemHeaderGoogleEarthWeb = LocationManagementPageResources.MenuItemHeaderGoogleEarthWeb;
        MenuItemHeaderGoogleMaps = LocationManagementPageResources.MenuItemHeaderGoogleMaps;
        MenuItemHeaderGoogleStreetView = LocationManagementPageResources.MenuItemHeaderGoogleStreetView;
    }

    private void UpdateMapBackColor()
    {
        var backColor = Utils.Resources.GetMaterialDesignPaperMapsUiStylesColor();
        MapControl.Map.BackColor = backColor;
    }

    private void UpdateTileLayer()
    {
        const string layerName = "Background";

        var httpTileSource = BruTile.Predefined.KnownTileSources.Create(KnownTileSourceSelected);
        var tileLayer = new TileLayer(httpTileSource);
        tileLayer.Name = layerName;

        var layers = MapControl?.Map.Layers.FindLayer(layerName);
        if (layers is not null) MapControl?.Map.Layers.Remove(layers.ToArray());

        MapControl?.Map.Layers.Insert(0, tileLayer);
    }

    #endregion

    // private void Option1_Click(object sender, RoutedEventArgs e)
    // {
    //     Console.WriteLine(ClickPoint);
    // }
    //
    // private void Option2_Click(object sender, RoutedEventArgs e)
    // {
    //     var s = ClickPoint.ToNominatim();
    //     Console.WriteLine(s);
    // }
}