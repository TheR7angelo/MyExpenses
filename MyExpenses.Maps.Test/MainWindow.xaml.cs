using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Styles;
using MyExpenses.Maps.Test.Utils;
using MyExpenses.Models.AutoMapper;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Sql.Context;
using MyExpenses.WebApi.Nominatim;

namespace MyExpenses.Maps.Test;

public partial class MainWindow
{
    private WritableLayer WritableLayer { get; }

    public MainWindow()
    {
        InitializeComponent();

        var map = MapStyle.GetMap(true);

        MapControl.Map = map;
        MapControl.Map.Layers.Add(Mapsui.Tiling.OpenStreetMap.CreateTileLayer());

        using var context = new DataBaseContext();
        var places = context.TPlaces
            .Where(s => s.Latitude != null && s.Latitude != 0 && s.Longitude != null && s.Longitude != 0)
            .ToList();
        context.Dispose();

        var features = new List<IFeature>();
        var mapper = Mapping.Mapper;
        foreach (var place in places)
        {
            var feature = mapper.Map<PointFeature>(place);

            feature.Styles = new List<IStyle>
            {
                MapStyle.RedMarkerStyle,
                new LabelStyle
                {
                    Text = place.Name, Offset = new Offset { X = 0, Y = 11 },
                    Font = new Font { FontFamily = "Arial", Size = 12 },
                    Halo = new Pen { Color = Color.White, Width = 2 }
                }
            };
            features.Add(feature);
        }

        // TODO after set point merge point to polygon then zoom to it

        WritableLayer = new WritableLayer { IsMapInfoLayer = true, Tag = typeof(TPlace) };
        WritableLayer.AddRange(features);
        WritableLayer.Style = null;

        MapControl.Map.Layers.Add(WritableLayer);

        context.Dispose();
    }

    private void MapControl_OnInfo(object? sender, MapInfoEventArgs e)
    {
        var mapInfo = e.MapInfo!;
        SetClickTPlace(mapInfo);
    }

    private TPlace? ClickTPlace { get; set; }
    private NetTopologySuite.Geometries.Point ClickPoint { get; set; } = NetTopologySuite.Geometries.Point.Empty;

    private PointFeature? PointFeature { get; set; }

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

    private void Option1_Click(object sender, RoutedEventArgs e)
    {
        Console.WriteLine(ClickPoint);
    }

    private void Option2_Click(object sender, RoutedEventArgs e)
    {
        var s = ClickPoint.ToNominatim();
        Console.WriteLine(s);
    }

    private void ProcessNewPlace(TPlace newPlace)
    {
        var success = newPlace.AddOrEditPlace();
        if (success)
        {
            var mapper = Mapping.Mapper;
            var feature = mapper.Map<PointFeature>(newPlace);
            feature.Styles = new List<IStyle>
            {
                MapStyle.RedMarkerStyle,
                new LabelStyle
                {
                    Text = newPlace.Name, Offset = new Offset { X = 0, Y = 11 },
                    Font = new Font { FontFamily = "Arial", Size = 12 },
                    Halo = new Pen { Color = Color.White, Width = 2 }
                }
            };

            WritableLayer.TryRemove(PointFeature!);
            WritableLayer.Add(feature);
            MapControl.Refresh();

            MessageBox.Show("Operation successful", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        else MessageBox.Show("Operation failed", "Failure", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    private void MenuItemAddFeature_OnClick(object sender, RoutedEventArgs e)
    {
        var windowEdit = new WindowEdit();
        windowEdit.SetTplace(ClickPoint);
        windowEdit.ShowDialog();

        if (windowEdit.DialogResult != true) return;

        ProcessNewPlace(windowEdit.Place);
    }

    private void MenuItemEditFeature_OnClick(object sender, RoutedEventArgs e)
    {
        var windowEdit = new WindowEdit();
        windowEdit.SetTplace(ClickTPlace!, false);
        windowEdit.ShowDialog();

        if (windowEdit.DialogResult != true) return;

        ProcessNewPlace(windowEdit.Place);
    }

    private void MenuItemDeleteFeature_OnClick(object sender, RoutedEventArgs e)
    {
        var feature = PointFeature;
        if (feature is null) return;

        try
        {
            WritableLayer.TryRemove(feature);

            var mapper = Mapping.Mapper;
            var placeToDelete = mapper.Map<TPlace>(feature);

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
}