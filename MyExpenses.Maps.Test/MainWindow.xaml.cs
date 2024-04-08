using System.IO;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Styles;
using MyExpenses.Sql.Context;

namespace MyExpenses.Maps.Test;

public partial class MainWindow
{
    public MainWindow()
    {
        const string icon =
            @"C:\Users\ZP6177\Documents\Programmation\C#\MyExpenses\MyExpenses.Maps.Test\Ressources\Sans titre - 1.png";
        var fileStream = new FileStream(icon, FileMode.Open);
        var bitmapId = BitmapRegistry.Instance.Register(fileStream);

        var pointStyle = new SymbolStyle
        {
            BitmapId = bitmapId,
            SymbolScale = 0.02
        };

        var mapControl = new Mapsui.UI.Wpf.MapControl();
        var map = new Map { CRS = "EPSG:3857", BackColor = Color.Gray };
        mapControl.Map = map;
        mapControl.Map.Layers.Add(Mapsui.Tiling.OpenStreetMap.CreateTileLayer());

        using var context = new DataBaseContext();
        var places = context.TPlaces
            .Where(s => s.Latitude != null && s.Latitude != 0 && s.Longitude != null && s.Longitude != 0)
            .ToList();

        var features = places.Select(place => new PointFeature(place.Longitude ?? 0, place.Latitude ?? 0)
        {
            Styles = new List<IStyle>
            {
                pointStyle,
                new LabelStyle
                {
                    Text = place.Name, Offset = new Offset { X = 0, Y = 30 },
                    Font = new Font { FontFamily = "Arial", Size = 12 },
                    Halo = new Pen { Color = Color.White, Width = 2 }
                }
            }
        }).Cast<IFeature>().ToList();

        var memoryProvider = new MemoryProvider(features) { CRS = "EPSG:4326" };
        var dataSource = new ProjectingProvider(memoryProvider) { CRS = "EPSG:3857" };

        var layer = new Layer { DataSource = dataSource };
        layer.Style = null;

        mapControl.Map.Layers.Add(layer);

        InitializeComponent();
        Content = mapControl;
    }
}