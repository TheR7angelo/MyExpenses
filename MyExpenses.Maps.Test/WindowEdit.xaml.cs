using System.Windows;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Styles;
using MyExpenses.Maps.Test.SelectNominatimSearchResult;
using MyExpenses.Maps.Test.Utils;
using MyExpenses.Models.AutoMapper;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Models.WebApi.Nominatim;
using MyExpenses.Utils;
using MyExpenses.WebApi.Nominatim;

namespace MyExpenses.Maps.Test;

public partial class WindowEdit
{
    #region Properties

    private const string ColumnTemp = "temp";
    public TPlace Place { get; } = new();
    private WritableLayer WritableLayer { get; } = new() { Style = null };

    #endregion

    public WindowEdit()
    {
        var map = MapStyle.GetMap(false);
        map.Layers.Add(Mapsui.Tiling.OpenStreetMap.CreateTileLayer());
        map.Layers.Add(WritableLayer);

        InitializeComponent();

        MapControl.Map = map;
    }

    #region Function

    private void HandleNominatimResult(List<NominatimSearchResult> nominatimSearchResults)
    {
        TPlace? place = null;

        var mapper = Mapping.Mapper;
        switch (nominatimSearchResults.Count)
        {
            case 0:
                MessageBox.Show("No results found.");
                break;
            case 1:
                MessageBox.Show("One results found.");
                var nominatimSearchResult = nominatimSearchResults.First();
                place = mapper.Map<TPlace>(nominatimSearchResult);
                break;
            case > 1:
                MessageBox.Show("Multiple results found. Please select one.");

                var places = nominatimSearchResults.Select(s => mapper.Map<TPlace>(s));
                var selectNominatimSearchResult = new WindowSelectNominatimSearchResult();
                selectNominatimSearchResult.AddRange(places);
                selectNominatimSearchResult.ShowDialog();

                if (!selectNominatimSearchResult.DialogResult.Equals(true)) return;

                place = mapper.Map<TPlace>(selectNominatimSearchResult.CurrentPlace);
                break;
        }

        if (place is null) return;
        SetTplace(place, true);
    }

    public void SetTplace(TPlace newTPlace, bool clear = false)
    {
        if (clear) WritableLayer.Clear();

        PropertyCopyHelper.CopyProperties(newTPlace, Place);
        var feature = Place.ToPointFeature();
        feature.Styles = new List<IStyle> { MapStyle.RedMarkerStyle };
        feature[ColumnTemp] = false;

        WritableLayer.Add(feature);

        MapControl.Map.Home = n => { n.CenterOnAndZoomTo(feature.Point, 1); };
        MapControl.Map.Navigator.CenterOn(feature.Point);
        MapControl.Map.Navigator.ZoomTo(1);
        MapControl.Refresh();
    }

    #endregion

    #region Action

    #region Button

    private void ButtonSearchByAddress_OnClick(object sender, RoutedEventArgs e)
    {
        var address = Place.ToString();
        var nominatimSearchResults = address.ToNominatim()?.ToList() ?? [];
        HandleNominatimResult(nominatimSearchResults);
    }

    private void ButtonSearchByCoordinate_OnClick(object sender, RoutedEventArgs e)
    {
        var point = Place.Geometry;
        var nominatimSearchResult = point.ToNominatim();

        var mapper = Mapping.Mapper;
        var newPlace = mapper.Map<TPlace>(nominatimSearchResult);
        if (newPlace is null)
        {
            MessageBox.Show("No results found.");
            return;
        }

        PropertyCopyHelper.CopyProperties(newPlace, Place);
    }

    private void ButtonValidNewPoint_OnClick(object sender, RoutedEventArgs e)
    {
        // TODO Valid new point
    }

    private void ButtonZoomToPoint_OnClick(object sender, RoutedEventArgs e)
    {
        // TODO Zoom to point
    }

    private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void ButtonValid_OnClick(object sender, RoutedEventArgs e)
    {
        // TODO valid action
    }

    #endregion

    private void MapControl_OnInfo(object? sender, MapInfoEventArgs e)
    {
        var worldPosition = e.MapInfo!.WorldPosition!;
        var feature = new PointFeature(worldPosition) { Styles = new List<IStyle> { MapStyle.GreenMarkerStyle } };
        feature[ColumnTemp] = true;

        var oldFeature = WritableLayer.GetFeatures().FirstOrDefault(f => f[ColumnTemp]!.Equals(true));
        if (oldFeature is not null) WritableLayer.TryRemove(oldFeature);

        WritableLayer.Add(feature);
        MapControl.Map.Refresh();
    }

    #endregion
}