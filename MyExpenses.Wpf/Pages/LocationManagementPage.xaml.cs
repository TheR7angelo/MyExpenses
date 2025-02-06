using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using BruTile.Predefined;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Manipulations;
using Mapsui.Projections;
using Mapsui.Tiling.Layers;
using Microsoft.Data.Sqlite;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Groups;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using MyExpenses.Utils.Maps;
using MyExpenses.WebApi.Maps;
using MyExpenses.Wpf.Resources.Resx.Pages.LocationManagementPage;
using MyExpenses.Wpf.Windows.LocationManagementWindows;
using MyExpenses.Wpf.Windows.MsgBox;
using Serilog;
using Point = NetTopologySuite.Geometries.Point;

namespace MyExpenses.Wpf.Pages;

public partial class LocationManagementPage
{
    public static readonly DependencyProperty ComboBoxBasemapHintAssistProperty =
        DependencyProperty.Register(nameof(ComboBoxBasemapHintAssist), typeof(string), typeof(LocationManagementPage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public object ComboBoxBasemapHintAssist
    {
        get => (string)GetValue(ComboBoxBasemapHintAssistProperty);
        set => SetValue(ComboBoxBasemapHintAssistProperty, value);
    }

    public static readonly DependencyProperty MenuItemHeaderAddPointProperty =
        DependencyProperty.Register(nameof(MenuItemHeaderAddPoint), typeof(string), typeof(LocationManagementPage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string MenuItemHeaderAddPoint
    {
        get => (string)GetValue(MenuItemHeaderAddPointProperty);
        set => SetValue(MenuItemHeaderAddPointProperty, value);
    }

    public static readonly DependencyProperty MenuItemHeaderEditFeatureProperty =
        DependencyProperty.Register(nameof(MenuItemHeaderEditFeature), typeof(string), typeof(LocationManagementPage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string MenuItemHeaderEditFeature
    {
        get => (string)GetValue(MenuItemHeaderEditFeatureProperty);
        set => SetValue(MenuItemHeaderEditFeatureProperty, value);
    }

    public static readonly DependencyProperty MenuItemHeaderDeleteFeatureProperty =
        DependencyProperty.Register(nameof(MenuItemHeaderDeleteFeature), typeof(string), typeof(LocationManagementPage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string MenuItemHeaderDeleteFeature
    {
        get => (string)GetValue(MenuItemHeaderDeleteFeatureProperty);
        set => SetValue(MenuItemHeaderDeleteFeatureProperty, value);
    }

    public static readonly DependencyProperty MenuItemHeaderMapsProperty =
        DependencyProperty.Register(nameof(MenuItemHeaderMaps), typeof(string), typeof(LocationManagementPage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string MenuItemHeaderMaps
    {
        get => (string)GetValue(MenuItemHeaderMapsProperty);
        set => SetValue(MenuItemHeaderMapsProperty, value);
    }

    public static readonly DependencyProperty MenuItemHeaderGoogleEarthWebProperty =
        DependencyProperty.Register(nameof(MenuItemHeaderGoogleEarthWeb), typeof(string),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            typeof(LocationManagementPage), new PropertyMetadata(default(string)));

    public string MenuItemHeaderGoogleEarthWeb
    {
        get => (string)GetValue(MenuItemHeaderGoogleEarthWebProperty);
        set => SetValue(MenuItemHeaderGoogleEarthWebProperty, value);
    }

    public static readonly DependencyProperty MenuItemHeaderGoogleMapsProperty =
        DependencyProperty.Register(nameof(MenuItemHeaderGoogleMaps), typeof(string), typeof(LocationManagementPage),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            new PropertyMetadata(default(string)));

    public string MenuItemHeaderGoogleMaps
    {
        get => (string)GetValue(MenuItemHeaderGoogleMapsProperty);
        set => SetValue(MenuItemHeaderGoogleMapsProperty, value);
    }

    public static readonly DependencyProperty MenuItemHeaderGoogleStreetViewProperty =
        DependencyProperty.Register(nameof(MenuItemHeaderGoogleStreetView), typeof(string),
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            typeof(LocationManagementPage), new PropertyMetadata(default(string)));

    public string MenuItemHeaderGoogleStreetView
    {
        get => (string)GetValue(MenuItemHeaderGoogleStreetViewProperty);
        set => SetValue(MenuItemHeaderGoogleStreetViewProperty, value);
    }

    public ObservableCollection<CountryGroup> CountryGroups { get; }
    public List<KnownTileSource> KnownTileSources { get; }
    public KnownTileSource KnownTileSourceSelected { get; set; }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    // The PlaceLayer instance is used to store the features of the places.
    private WritableLayer PlaceLayer { get; } = new() { Style = null, Tag = typeof(TPlace) };
    private IEnumerable<ILayer> InfoLayers { get; }

    private TPlace? ClickTPlace { get; set; }
    private Point ClickPoint { get; set; } = Point.Empty;
    private PointFeature? PointFeature { get; set; }

    public LocationManagementPage()
    {
        KnownTileSources = [..MapsuiMapExtensions.GetAllKnowTileSource()];
        InfoLayers = [PlaceLayer];

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Necessary instantiation of DataBaseContext to interact with the database.
        // This creates a scoped database context for performing queries and modifications in the database.
        using var context = new DataBaseContext();
        var places = context.TPlaces.OrderBy(s => s.Country).ThenBy(s => s.City).ThenBy(s => s.Name).ToList();
        var groups = places.GetGroups();

        CountryGroups = [..groups];

        var features = places
            .Where(s => s.Latitude is not null && s.Latitude is not 0 && s.Longitude is not null &&
                        s.Longitude is not 0)
            .Select(feature => feature.IsOpen
                ? feature.ToFeature(MapsuiStyleExtensions.RedMarkerStyle)
                : feature.ToFeature(MapsuiStyleExtensions.BlueMarkerStyle));

        PlaceLayer.AddRange(features);

        var backColor = Utils.Resources.GetMaterialDesignPaperMapsUiStylesColor();
        var map = MapsuiMapExtensions.GetMap(true, backColor);
        map.Layers.Add(PlaceLayer);

        UpdateLanguage();

        InitializeComponent();

        MapControl.Map = map;

        SetInitialZoom();

        // ReSharper disable HeapView.DelegateAllocation
        Interface.ThemeChanged += Interface_OnThemeChanged;
        Interface.LanguageChanged += Interface_OnLanguageChanged;
        // ReSharper restore HeapView.DelegateAllocation
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

    private void Interface_OnLanguageChanged()
        => UpdateLanguage();

    private void Interface_OnThemeChanged()
        => UpdateMapBackColor();

    private void MapControl_OnLoaded(object sender, RoutedEventArgs e)
        => UpdateTileLayer();

    private void MapControl_OnContextMenuOpening(object sender, ContextMenuEventArgs e)
    {
        var position = Mouse.GetPosition(MapControl);
        var worldPosition = MapControl.Map.Navigator.Viewport.ScreenToWorld(position.X, position.Y);

        var lonLat = SphericalMercator.ToLonLat(worldPosition.X, worldPosition.Y);

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The ClickPoint instance is used to store the coordinates of the point clicked on the map.
        ClickPoint = new Point(lonLat.lon, lonLat.lat);

        var screenPosition = new ScreenPosition(position.X, position.Y);
        var mapInfo = MapControl.GetMapInfo(screenPosition, InfoLayers);
        SetClickTPlace(mapInfo);
    }

    private void MapControl_OnInfo(object? sender, MapInfoEventArgs e)
    {
        var mapInfo = e.GetMapInfo(InfoLayers);
        SetClickTPlace(mapInfo);
    }

    private void MenuItemAddFeature_OnClick(object sender, RoutedEventArgs e)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The AddEditLocationWindow instance is created to allow the user to add or edit a location.
        // The SetPlace method is called with the current ClickPoint to initialize the dialog with the relevant location data.
        // ShowDialog() displays the window modally, pausing execution until the user has interacted with and closed the dialog.
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
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The AddEditLocationWindow instance is created to manage the addition or editing of a location.
        // The SetPlace method is called with the current ClickTPlace and an additional parameter to configure the dialog appropriately.
        // ShowDialog() is used to display the window modally, halting execution until the user closes the dialog.
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
        var uri = ClickPoint.ToGoogleEarthWeb();
        Log.Information("{Uri}", uri);
    }


    private void MenuItemToGoogleMaps_OnClick(object sender, RoutedEventArgs e)
    {
        var log = GetLogAction("Google Maps");

        Log.Information("{Log}", log);
        var uri = ClickPoint.ToGoogleMaps();
        Log.Information("{Uri}", uri);
    }

    private void MenuItemToGoogleStreetView_OnClick(object sender, RoutedEventArgs e)
    {
        var log = GetLogAction("Google Street View");

        Log.Information("{Log}", log);
        var uri = ClickPoint.ToGoogleStreetView();
        Log.Information("{Uri}", uri);
    }

    private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        => UpdateTileLayer();

    private void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        if (sender is not TreeView treeView) return;

        var points = treeView.SelectedItem switch
        {
            CountryGroup countryGroup => countryGroup.CityGroups?
                .SelectMany(cityGroup => GetPoints(cityGroup.Places)) ?? [],
            CityGroup cityGroup => GetPoints(cityGroup.Places),
            _ => []
        };

        SetZoom(points.ToArray());
        return;

        IEnumerable<MPoint> GetPoints(IEnumerable<TPlace>? places)
        {
            return places?
                .Where(s => (s.Geometry as Point)?.X is not 0 && (s.Geometry as Point)?.Y is not 0)
                .Select(s => s.ToMPoint()) ?? [];
        }
    }

    #endregion

    #region Function

    private void AddPlaceTreeViewCountryGroup(TPlace placeToAdd)
    {
        // ReSharper disable HeapView.DelegateAllocation
        var cityGroup = CountryGroups.FirstOrDefault(s => s.Country == placeToAdd.Country)?.CityGroups
            ?.FirstOrDefault(s => s.City == placeToAdd.City);
        // ReSharper restore HeapView.DelegateAllocation

        if (cityGroup is null)
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            // The newCityGroup instance is used to store the information of the city.
            var newCityGroup = new CityGroup { City = placeToAdd.City, Places = [placeToAdd] };

            // ReSharper disable once HeapView.DelegateAllocation
            var countryGroup = CountryGroups.FirstOrDefault(s => s.Country == placeToAdd.Country);
            if (countryGroup is null)
            {
                // ReSharper disable once HeapView.ObjectAllocation.Evident
                // The newGroupCountry instance is used to store the information of the country.
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
            var feature = newPlace.IsOpen
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
                                                // ReSharper disable once HeapView.DelegateAllocation
                                                cityGroup.Places.Any(place => place.Id == placeToDelete.Id)));

        var cityToRemove = countryToRemove?
            .CityGroups?.FirstOrDefault(cityGroup => cityGroup.Places is not null &&
                                                     // ReSharper disable once HeapView.DelegateAllocation
                                                     cityGroup.Places.Any(place => place.Id == placeToDelete.Id));

        var placeToRemove = cityToRemove?.Places?
            // ReSharper disable once HeapView.DelegateAllocation
            .FirstOrDefault(place => place.Id.Equals(placeToDelete.Id));
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
                var mRect = points.ToMRect();
                MapControl.Map.Navigator.ZoomToBox(mRect);
                break;
        }
    }

    private void SetInitialZoom()
    {
        var points = PlaceLayer.GetFeatures().Select(s => ((PointFeature)s).Point).ToArray();

        switch (points.Length)
        {
            case 0:
                break;
            case 1:
                MapControl.Map.Navigator.CenterOnAndZoomTo(points[0], 1);
                break;
            case > 1:
                var mRect = points.ToMRect();
                MapControl.Map.Navigator.ZoomToBox(mRect);
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

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // A new instance of TileLayer is created here using the specified httpTileSource.
        // This layer is responsible for rendering map tiles from the configured tile source,
        // allowing the application to display background maps or other geographic data dynamically
        // based on the selected tile provider.
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