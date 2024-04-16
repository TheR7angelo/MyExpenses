using System.Windows;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Styles;
using MyExpenses.Maps.Test.Utils;
using MyExpenses.Models.AutoMapper;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Utils;
using MyExpenses.WebApi.Nominatim;

namespace MyExpenses.Maps.Test;

public partial class WindowEdit
{
    public TPlace Place { get; } = new();

    private const string ColumnTemp = "temp";
    private WritableLayer WritableLayer { get; } = new() { Style = null };

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
        PropertyCopyHelper.CopyProperties(newTPlace, Place);
        var feature = Place.ToPointFeature();
        feature.Styles = new List<IStyle> { MapStyle.RedMarkerStyle };
        feature[ColumnTemp] = false;

        WritableLayer.Add(feature);

        MapControl.Map.Home = n => { n.CenterOnAndZoomTo(feature.Point, 1); };
    }

    private void ButtonSearchByCoordinate_OnClick(object sender, RoutedEventArgs e)
    {
        var point = Place.Geometry;
        var nominatimSearchResult = point.ToNominatim();

        var mapper = Mapping.Mapper;
        var newTPlace = mapper.Map<TPlace>(nominatimSearchResult);
        PropertyCopyHelper.CopyProperties(newTPlace, Place);
    }

    private void MapControl_OnInfo(object? sender, MapInfoEventArgs e)
    {
        var worldPosition = e.MapInfo!.WorldPosition!;
        var feature = new PointFeature(worldPosition) { Styles = new List<IStyle> { MapStyle.GreenMarkerStyle } };
        feature[ColumnTemp] = true;

        var oldFeature = WritableLayer.GetFeatures().FirstOrDefault(f => f[ColumnTemp]!.Equals(true));
        if (oldFeature is not null)  WritableLayer.TryRemove(oldFeature);

        WritableLayer.Add(feature);
        MapControl.Map.Refresh();
    }

    private void ButtonSearchByAddress_OnClick(object sender, RoutedEventArgs e)
    {
        var partAddress = new List<string>();
        if (!string.IsNullOrEmpty(Place.Number)) partAddress.Add(Place.Number);
        if (!string.IsNullOrEmpty(Place.Street)) partAddress.Add(Place.Street);
        if (!string.IsNullOrEmpty(Place.Postal)) partAddress.Add(Place.Postal);
        if (!string.IsNullOrEmpty(Place.City)) partAddress.Add(Place.City);
        if (!string.IsNullOrEmpty(Place.Country)) partAddress.Add(Place.Country);
        var address = string.Join(", ", partAddress);

        var nominatimSearchResults = address.ToNominatim()?.ToList() ?? [];
        switch (nominatimSearchResults.Count)
        {
            case 0:
                MessageBox.Show("No results found.");
                break;
            case 1:
                // TODO Update UI
                break;
            case >1:
                MessageBox.Show("Multiple results found. Please select one.");
                nominatimSearchResults.ForEach(Console.WriteLine);
                break;
        }
    }
}