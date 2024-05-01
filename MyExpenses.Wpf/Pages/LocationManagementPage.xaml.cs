using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BruTile.Predefined;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Tiling.Layers;
using MyExpenses.Models.Sql.Groups;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using MyExpenses.Wpf.Utils.Maps;

namespace MyExpenses.Wpf.Pages;

public partial class LocationManagementPage
{
    public ObservableCollection<CountryGroup> CountryGroups { get; }
    public List<KnownTileSource> KnownTileSources { get; }
    public KnownTileSource KnownTileSourceSelected { get; set; }

    private WritableLayer PlaceLayer { get; } = new() { Style = null, IsMapInfoLayer = true, Tag = typeof(TPlace) };

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

    private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        => UpdateTileLayer();
    
    private void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        if (sender is not TreeView treeView) return;

        if(treeView.SelectedItem is not TPlace place) return;

        Console.WriteLine(place.Name);
    }

    #endregion

    #region Function

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

                MapControl.Map.Home = navigator => {navigator.ZoomToBox(mRect);  };
                break;
        }
    }

    #endregion
}