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
        var bitmapId = BitmapRegistry.Instance.Register(
            @"C:\Users\ZP6177\Documents\Programmation\C#\MyExpenses\MyExpenses.Maps.Test\Ressources\Sans titre - 1.png");

        var pointStyle = new SymbolStyle
        {
            BitmapId = bitmapId,
            SymbolScale = 1.0
        };

        var mapControl = new Mapsui.UI.Wpf.MapControl();
        var map = new Map { CRS = "EPSG:3857", BackColor = Color.Gray };
        mapControl.Map = map;
        mapControl.Map.Layers.Add(Mapsui.Tiling.OpenStreetMap.CreateTileLayer());

        using var context = new DataBaseContext();
        var places = context.TPlaces.ToList();

        var features = new List<IFeature>();
        foreach (var place in places)
        {
            var point = new PointFeature(place.Longitude ?? 0, place.Latitude ?? 0)
            {
                ["Label"] = place.Name,
                Styles = new List<IStyle> { pointStyle }
            };
            features.Add(point);
        }

        var memoryProvider = new MemoryProvider(features) { CRS = "EPSG:4326" };
        var dataSource = new ProjectingProvider(memoryProvider) { CRS = "EPSG:3857" };

        var layer = new Layer { DataSource = dataSource };

        mapControl.Map.Layers.Add(layer);

        InitializeComponent();
        Content = mapControl;
    }
}