using System.Windows;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Styles;
using MyExpenses.Maps.Test.SelectNominatimSearchResult;
using MyExpenses.Maps.Test.Utils;
using MyExpenses.Models.AutoMapper;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Models.WebApi.Nominatim;
using MyExpenses.Utils;
using MyExpenses.WebApi.Nominatim;
using Point = NetTopologySuite.Geometries.Point;

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
        var mapper = Mapping.Mapper;
        var feature = mapper.Map<PointFeature>(Place);
        feature.Styles = new List<IStyle> { MapStyle.RedMarkerStyle };
        feature[ColumnTemp] = false;

        WritableLayer.Add(feature);

        MapControl.Map.Home = n => { n.CenterOnAndZoomTo(feature.Point, 1); };
        MapControl.Map.Navigator.CenterOn(feature.Point);
        MapControl.Map.Navigator.ZoomTo(1);
        MapControl.Refresh();
    }

    private void ZoomToMPoint(MPoint mPoint)
    {
        MapControl.Map.Navigator.CenterOn(mPoint);
        MapControl.Map.Navigator.ZoomTo(1);
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
        var pointsFeatures = WritableLayer.GetFeatures().Select(s => (PointFeature)s).ToList();
        if (pointsFeatures.Count < 2) return;

        var newFeature = pointsFeatures.FirstOrDefault(f => f[ColumnTemp]!.Equals(true))!;
        foreach (var pointFeature in pointsFeatures)
        {
            WritableLayer.TryRemove(pointFeature);
        }

        var coordinate = SphericalMercator.ToLonLat(newFeature.Point);
        Place.Geometry = new Point(coordinate.Y, coordinate.X);

        newFeature[ColumnTemp] = false;
        newFeature.Styles = new List<IStyle> { MapStyle.RedMarkerStyle };
        WritableLayer.Add(newFeature);

        ZoomToMPoint(newFeature.Point);
    }

    private void ButtonZoomToPoint_OnClick(object sender, RoutedEventArgs e)
    {
        var pointsFeatures = WritableLayer.GetFeatures();
        var points = pointsFeatures.Select(s => ((PointFeature)s).Point).ToList();

        if (points.Count > 1)
        {
            double minX = points.Min(p => p.X), maxX = points.Max(p => p.X);
            double minY = points.Min(p => p.Y), maxY = points.Max(p => p.Y);

            var width = maxX - minX;
            var height = maxY - minY;

            const double marginPercentage = 10; // Change this value to suit your needs
            var marginX = width * marginPercentage / 100;
            var marginY = height * marginPercentage / 100;

            var mRect = new MRect(minX - marginX, minY - marginY, maxX + marginX, maxY + marginY);

            MapControl.Map.Navigator.ZoomToBox(mRect);
        }
        else ZoomToMPoint(points[0]);
    }

    private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void ButtonValid_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
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