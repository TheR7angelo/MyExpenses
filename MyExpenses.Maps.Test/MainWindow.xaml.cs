using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Styles;
using Mapsui.Widgets;
using Mapsui.Widgets.ScaleBar;
using Mapsui.Widgets.Zoom;
using MyExpenses.Models.AutoMapper;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Sql.Context;
using MyExpenses.WebApi.Nominatim;

namespace MyExpenses.Maps.Test;

public partial class MainWindow
{
    private WritableLayer WritableLayer { get; }

    private SymbolStyle PointStyle { get; }

    public MainWindow()
    {
        InitializeComponent();

        const string icon =
            @"C:\Users\ZP6177\Documents\Programmation\C#\MyExpenses\MyExpenses.Maps.Test\Ressources\Sans titre - 1.png";
        var fileStream = new FileStream(icon, FileMode.Open);
        var bitmapId = BitmapRegistry.Instance.Register(fileStream);

        PointStyle = new SymbolStyle
        {
            BitmapId = bitmapId,
            SymbolScale = 0.02
        };

        var map = new Map { CRS = "EPSG:3857", BackColor = Color.Gray };
        map.Widgets.AddRange(new List<IWidget>
        {
            new MapInfoWidget(map),
            new ZoomInOutWidget(),
            new ScaleBarWidget(map)
        });

        MapControl.Map = map;
        MapControl.Map.Layers.Add(Mapsui.Tiling.OpenStreetMap.CreateTileLayer());

        using var context = new DataBaseContext();
        var places = context.TPlaces
            .Where(s => s.Latitude != null && s.Latitude != 0 && s.Longitude != null && s.Longitude != 0)
            .ToList();

        var features = new List<IFeature>();
        var properties = typeof(TPlace).GetProperties();
        foreach (var place in places)
        {
            var point = SphericalMercator.FromLonLat(place.Longitude ?? 0, place.Latitude ?? 0);
            var feature = new PointFeature(point.x, point.y);

            foreach (var property in properties)
            {
                var columnName = property.GetCustomAttribute<ColumnAttribute>()?.Name;
                if (string.IsNullOrEmpty(columnName)) continue;

                feature[columnName] = property.GetValue(place);
            }

            feature.Styles = new List<IStyle>
            {
                PointStyle,
                new LabelStyle
                {
                    Text = place.Name, Offset = new Offset { X = 0, Y = 30 },
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
    }

    private void MapControl_OnInfo(object? sender, MapInfoEventArgs e)
    {
        var mapInfo = e.MapInfo!;
        SetClickTPlace(mapInfo);
    }

    private TPlace? ClickTPlace { get; set; }
    private NetTopologySuite.Geometries.Point ClickPoint { get; set; } = NetTopologySuite.Geometries.Point.Empty;

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
        var feature = mapInfo.Feature;
        var layer = mapInfo.Layer;

        if (feature is null || layer is null)
        {
            EditFeature.Visibility = Visibility.Collapsed;
            ClickTPlace = null;
            return;
        }
        if (layer.Tag is not Type type)
        {
            EditFeature.Visibility = Visibility.Collapsed;
            ClickTPlace = null;
            return;
        }

        EditFeature.Visibility = Visibility.Visible;
        if (type != typeof(TPlace)) return;

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
        var nominatim = new Nominatim("Test");
        var s = nominatim.PointToNominatim(ClickPoint);
        Console.WriteLine(s);
    }

    private void Option3_OnClick(object sender, RoutedEventArgs e)
    {
        Console.WriteLine(ClickTPlace?.Id);
    }
}