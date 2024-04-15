using System.Windows;
using Mapsui.Layers;
using Mapsui.Styles;
using MyExpenses.Maps.Test.Utils;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Utils;
using MyExpenses.WebApi.Nominatim;
using Point = NetTopologySuite.Geometries.Point;

namespace MyExpenses.Maps.Test;

public partial class WindowEdit
{
    public TPlace TPlace { get; } = new();

    private WritableLayer WritableLayer { get; } = new WritableLayer { Style = null };

    public WindowEdit()
    {
        var map = MapStyle.GetMap(false);
        map.Layers.Add(Mapsui.Tiling.OpenStreetMap.CreateTileLayer());

        map.Layers.Add(WritableLayer);

        InitializeComponent();

        MapControl.Map = map;
    }

    public void SetTplace(TPlace newTPlace)
    {
        PropertyCopyHelper.CopyProperties(newTPlace, TPlace);
        var feature = TPlace.ToPointFeature();
        feature.Styles = new List<IStyle> { MapStyle.RedMarkerStyle };

        WritableLayer.Add(feature);

        MapControl.Map.Home = n => { n.CenterOnAndZoomTo(feature.Point, 1); };
    }

    private void ButtonSearchByCoordinate_OnClick(object sender, RoutedEventArgs e)
    {
        var latitude = TPlace.Latitude;
        var longitude = TPlace.Longitude;
        var point = new Point(latitude ?? 0, longitude ?? 0);

        var result = Nominatim.PointToNominatim(point);
        Console.WriteLine(result);
    }
}