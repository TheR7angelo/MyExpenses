using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BruTile.Predefined;
using Mapsui.Tiling.Layers;
using MyExpenses.Models.Sql.Groups;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using MyExpenses.Wpf.Utils;

namespace MyExpenses.Wpf.Pages;

public partial class LocationManagementPage
{
    public ObservableCollection<CountryGroup> CountryGroup { get; }
    public ObservableCollection<CountryGroup> CountryGroups { get; }
    public List<KnownTileSource> KnownTileSources { get; }
    public KnownTileSource KnownTileSourceSelected { get; set; }

    public LocationManagementPage()
    {
        var blackList = new List<KnownTileSource>
        {
            KnownTileSource.OpenCycleMap, KnownTileSource.OpenCycleMapTransport,
            KnownTileSource.StamenToner, KnownTileSource.StamenTonerLite, KnownTileSource.StamenWatercolor, KnownTileSource.StamenTerrain,
            KnownTileSource.EsriWorldReferenceOverlay, KnownTileSource.EsriWorldBoundariesAndPlaces,
            KnownTileSource.HereHybrid, KnownTileSource.HereTerrain
        };
        KnownTileSources = Enum.GetValues<KnownTileSource>().Where(s => !blackList.Contains(s)).ToList();

        using var context = new DataBaseContext();
        var places = context.TPlaces.OrderBy(s => s.Country).ThenBy(s => s.City).ToList();
        var groups = places.GetGroups();

        CountryGroup = new ObservableCollection<CountryGroup>(groups);

        // TODO add listener color change
        var brush = (SolidColorBrush)FindResource("MaterialDesignPaper");
        var backColor = brush.ToColor();

        var map = MapsuiExtensions.GetMap(true, backColor);
        InitializeComponent();

        MapControl.Map = map;
    }

    private void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        if (sender is not TreeView treeView) return;

        if(treeView.SelectedItem is not TPlace place) return;

        Console.WriteLine(place.Name);
    }

    private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        => UpdateTileLayer();

    private void MapControl_OnLoaded(object sender, RoutedEventArgs e)
        => UpdateTileLayer();

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
}