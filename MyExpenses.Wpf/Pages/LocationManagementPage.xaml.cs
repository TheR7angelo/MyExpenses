using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using BruTile.Predefined;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Tiling.Layers;
using Microsoft.Data.Sqlite;
using MyExpenses.Models.Sql.Groups;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using MyExpenses.WebApi.Nominatim;
using MyExpenses.Wpf.Resources.Resx.Pages.LocationManagementPage;
using MyExpenses.Wpf.Utils.Maps;
using MyExpenses.Wpf.Windows.LocationManagementWindows;
using Serilog;

namespace MyExpenses.Wpf.Pages;

public partial class LocationManagementPage
{
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
            .Where(s => s.Latitude != null && s.Latitude != 0 && s.Longitude != null && s.Longitude != 0)
            .ToFeature(MapsuiStyleExtensions.RedMarkerStyle);
        PlaceLayer.AddRange(features);

        // TODO add listener color change
        var brush = (SolidColorBrush)FindResource("MaterialDesignPaper");
        var backColor = brush.ToMapsuiColor();

        var map = MapsuiMapExtensions.GetMap(true, backColor);
        map.Layers.Add(PlaceLayer);

        InitializeComponent();

        MapControl.Map = map;

        SetInitialZoom();
    }

    #region Action

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

    //TODO add feature to treeView
    private void MenuItemAddFeature_OnClick(object sender, RoutedEventArgs e)
    {
        var addEditLocationWindow = new AddEditLocationWindow();
        addEditLocationWindow.SetTplace(ClickPoint);
        addEditLocationWindow.ShowDialog();

        if (addEditLocationWindow.DialogResult != true) return;

        var newPlace = addEditLocationWindow.Place;
        ProcessNewPlace(newPlace);
        AddPlaceTreeViewCountryGroup(newPlace);
    }

    private void MenuItemDeleteFeature_OnClick(object sender, RoutedEventArgs e)
    {
        var feature = PointFeature;
        if (feature is null) return;

        var placeToDelete = feature.ToTPlace();
        Log.Information("Attempting to remove the place \"{PlaceToDeleteName}\"", placeToDelete.Name);
        PlaceLayer.TryRemove(feature);

        var (success, exception) = placeToDelete.Delete();

        if (success)
        {
            MapControl.Refresh();

            Log.Information("Place was successfully removed");
            MessageBox.Show(LocationManagementPageResources.MessageBoxMenuItemDeleteFeatureNoUseSuccess);
        }
        else
        {
            if (exception!.InnerException is SqliteException
                {
                    SqliteExtendedErrorCode: SQLitePCL.raw.SQLITE_CONSTRAINT_FOREIGNKEY
                })
            {
                Log.Error("Foreign key constraint violation");

                var response =
                    MessageBox.Show(LocationManagementPageResources.MessageBoxMenuItemDeleteFeatureUseQuestion,
                        "Question", MessageBoxButton.YesNoCancel);

                if (response != MessageBoxResult.Yes) return;

                Log.Information("Attempting to remove the place \"{PlaceToDeleteName}\" with all relative element",
                    placeToDelete.Name);
                placeToDelete.Delete(true);
                Log.Information("Place and all relative element was successfully removed");
                MessageBox.Show(LocationManagementPageResources.MessageBoxMenuItemDeleteFeatureUseSuccess);

                RemovePlaceTreeViewCountryGroup(placeToDelete);

                MapControl.Refresh();
            }
            else
            {
                Log.Error(exception, "An error occurred please retry");
                MessageBox.Show(LocationManagementPageResources.MessageBoxMenuItemDeleteFeatureError);
            }
        }
    }

    //TODO remove feature and insert new feature to treeView
    private void MenuItemEditFeature_OnClick(object sender, RoutedEventArgs e)
    {
        var addEditLocationWindow = new AddEditLocationWindow();
        addEditLocationWindow.SetTplace(ClickTPlace!, false);
        addEditLocationWindow.ShowDialog();

        if (addEditLocationWindow.DialogResult != true) return;

        var editedPlace = addEditLocationWindow.Place;
        ProcessNewPlace(editedPlace);

        RemovePlaceTreeViewCountryGroup(editedPlace);
        AddPlaceTreeViewCountryGroup(editedPlace);
    }

    private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        => UpdateTileLayer();

    private void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        if (sender is not TreeView treeView) return;

        if (treeView.SelectedItem is not TPlace place) return;

        if (place.Longitude is null || place.Longitude == 0 || place.Latitude is null || place.Latitude == 0) return;

        var pointFeature = place.ToFeature();
        MapControl.Map.Navigator.CenterOn(pointFeature.Point);
        MapControl.Map.Navigator.ZoomTo(0);
    }

    #endregion

    #region Function

    private void AddPlaceTreeViewCountryGroup(TPlace placeToAdd)
    {
        placeToAdd.Country ??= "Unknown";
        placeToAdd.City ??= "Unknown";

        var cityGroup = CountryGroups.FirstOrDefault(s => s.Country == placeToAdd.Country)?.CityGroups
            ?.FirstOrDefault(s => s.City == placeToAdd.City);

        if (cityGroup is null)
        {
            var newCityGroup = new CityGroup { City = placeToAdd.City, Places = [placeToAdd] };

            var countryGroup = CountryGroups.FirstOrDefault(s => s.Country == placeToAdd.Country);
            if (countryGroup is null)
                CountryGroups.Add(new CountryGroup { Country = placeToAdd.Country, CityGroups = [newCityGroup] });
            else countryGroup.CityGroups?.Add(newCityGroup);
        }
        else
        {
            cityGroup.Places?.Add(placeToAdd);
        }
    }

    // TODO work
    private void ProcessNewPlace(TPlace newPlace)
    {
        var (success, _) = newPlace.AddOrEdit();
        if (success)
        {
            var feature = newPlace.ToFeature(MapsuiStyleExtensions.RedMarkerStyle);

            PlaceLayer.TryRemove(PointFeature!);
            PlaceLayer.Add(feature);
            MapControl.Refresh();

            MessageBox.Show("Operation successful");
        }
        else MessageBox.Show("Operation failed");
    }

    private void RemovePlaceTreeViewCountryGroup(TPlace placeToDelete)
    {
        var countryToRemove = CountryGroups
            .FirstOrDefault(countryGroup => countryGroup.CityGroups != null &&
                                            countryGroup.CityGroups.Any(cityGroup => cityGroup.Places != null &&
                                                cityGroup.Places.Any(place => place.Id == placeToDelete.Id)));

        var cityToRemove = countryToRemove?
            .CityGroups?.FirstOrDefault(cityGroup => cityGroup.Places != null &&
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

    private void Option1_Click(object sender, RoutedEventArgs e)
    {
        Console.WriteLine(ClickPoint);
    }

    private void Option2_Click(object sender, RoutedEventArgs e)
    {
        var s = ClickPoint.ToNominatim();
        Console.WriteLine(s);
    }
}