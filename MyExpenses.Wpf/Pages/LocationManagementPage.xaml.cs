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
using MyExpenses.Models.AutoMapper;
using MyExpenses.Models.Sql.Groups;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using MyExpenses.WebApi.Nominatim;
using MyExpenses.Wpf.Utils.Maps;

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
        KnownTileSources = MapsuiMapExtensions.GetAllKnowTileSource().ToList();

        using var context = new DataBaseContext();
        var places = context.TPlaces.OrderBy(s => s.Country).ThenBy(s => s.City).ToList();
        var groups = places.GetGroups();

        CountryGroups = new ObservableCollection<CountryGroup>(groups);

        var features = places
            .Where(s => s.Latitude != null && s.Latitude != 0 && s.Longitude != null && s.Longitude != 0)
            .ToFeature(MapsuiStyleExtensions.RedMarkerStyle);
        PlaceLayer.AddRange(features);

        // TODO add listener color change
        var brush = (SolidColorBrush)FindResource("MaterialDesignPaper");
        var backColor = brush.ToColor();

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

    private void MenuItemAddFeature_OnClick(object sender, RoutedEventArgs e)
    {
        // var windowEdit = new WindowEdit();
        // windowEdit.SetTplace(ClickPoint);
        // windowEdit.ShowDialog();
        //
        // if (windowEdit.DialogResult != true) return;
        //
        // ProcessNewPlace(windowEdit.Place);
    }

    private void MenuItemDeleteFeature_OnClick(object sender, RoutedEventArgs e)
    {
        var feature = PointFeature;
        if (feature is null) return;

        try
        {
            PlaceLayer.TryRemove(feature);

            var placeToDelete = feature.ToTPlace();

            using var context = new DataBaseContext();
            context.TPlaces.Remove(placeToDelete);
            context.SaveChanges();

            MapControl.Refresh();

            MessageBox.Show("Operation successful", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            MessageBox.Show("Operation failed", "Failure", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void MenuItemEditFeature_OnClick(object sender, RoutedEventArgs e)
    {
        // var windowEdit = new WindowEdit();
        // windowEdit.SetTplace(ClickTPlace!, false);
        // windowEdit.ShowDialog();
        //
        // if (windowEdit.DialogResult != true) return;
        //
        // ProcessNewPlace(windowEdit.Place);
    }

    private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        => UpdateTileLayer();

    private void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        if (sender is not TreeView treeView) return;

        if (treeView.SelectedItem is not TPlace place) return;

        Console.WriteLine(place.Name);
    }

    #endregion

    #region Function

    private void ProcessNewPlace(TPlace newPlace)
    {
        // var (success, _) = newPlace.AddOrEdit();
        // if (success)
        // {
        //     var mapper = Mapping.Mapper;
        //     var feature = mapper.Map<PointFeature>(newPlace);
        //     feature.Styles = new List<IStyle>
        //     {
        //         MapsuiStyleExtensions.RedMarkerStyle,
        //         new LabelStyle
        //         {
        //             Text = newPlace.Name, Offset = new Offset { X = 0, Y = 11 },
        //             Font = new Font { FontFamily = "Arial", Size = 12 },
        //             Halo = new Pen { Color = Color.White, Width = 2 }
        //         }
        //     };
        //
        //     PlaceLayer.TryRemove(PointFeature!);
        //     PlaceLayer.Add(feature);
        //     MapControl.Refresh();
        //
        //     MessageBox.Show("Operation successful", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        // }
        // else MessageBox.Show("Operation failed", "Failure", MessageBoxButton.OK, MessageBoxImage.Error);
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
        var mapper = Mapping.Mapper;
        var place = mapper.Map<TPlace>(feature);
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