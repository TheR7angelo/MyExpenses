using System.IO;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Styles;
using MyExpenses.Sql.Context;

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
        MapControl.Map = map;
        MapControl.Map.Layers.Add(Mapsui.Tiling.OpenStreetMap.CreateTileLayer());

        using var context = new DataBaseContext();
        var places = context.TPlaces
            .Where(s => s.Latitude != null && s.Latitude != 0 && s.Longitude != null && s.Longitude != 0)
            .ToList();

        var features = new List<IFeature>();
        foreach (var place in places)
        {
            var point = SphericalMercator.FromLonLat(place.Longitude ?? 0, place.Latitude ?? 0);
            var feature = new PointFeature(point.x, point.y);
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

        WritableLayer = new WritableLayer();
        WritableLayer.AddRange(features);
        WritableLayer.Style = null;

        MapControl.Map.Layers.Add(WritableLayer);
    }

    void AddPointAndRefreshMap(double longitude, double latitude, string name)
    {

    }
}