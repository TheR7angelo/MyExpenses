using System.Collections.ObjectModel;
using BruTile.Predefined;
using Mapsui.Layers;
using MyExpenses.Models.Sql.Bases.Groups;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Smartphones.Converters;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using MyExpenses.Utils.Maps;
using Point = NetTopologySuite.Geometries.Point;

namespace MyExpenses.Smartphones.ContentPages;

public partial class LocationManagementContentPage
{
    public List<KnownTileSource> KnownTileSources { get; }
    public KnownTileSource KnownTileSourceSelected { get; set; }

    private WritableLayer PlaceLayer { get; } = new() { Style = null, IsMapInfoLayer = true, Tag = typeof(TPlace) };

    private TPlace? ClickTPlace { get; set; }
    private Point ClickPoint { get; set; } = Point.Empty;
    private PointFeature? PointFeature { get; set; }

    public ObservableCollection<TreeViewNode> TreeViewNodes { get; }

    public LocationManagementContentPage()
    {
        KnownTileSources = [..MapsuiMapExtensions.GetAllKnowTileSource()];

        var (treeViewNodes, places) = GenerateTreeViewNodes();
        TreeViewNodes = [..treeViewNodes];

        var features = places
            .Where(s => s.Latitude is not null && s.Latitude is not 0 && s.Longitude is not null &&
                        s.Longitude is not 0)
            .Select(feature => feature.IsOpen
                ? feature.ToFeature(MapsuiStyleExtensions.RedMarkerStyle)
                : feature.ToFeature(MapsuiStyleExtensions.BlueMarkerStyle));

        PlaceLayer.AddRange(features);
        var backColor = AppInfo.RequestedTheme is AppTheme.Dark
            ? Mapsui.Styles.Color.Black
            : Mapsui.Styles.Color.White;

        var map = MapsuiMapExtensions.GetMap(true, backColor);
        map.Layers.Add(PlaceLayer);

        InitializeComponent();

        MapControl.Map = map;
        UpdateDisplay();

        DeviceDisplay.MainDisplayInfoChanged += DeviceDisplay_OnMainDisplayInfoChanged;
    }

    private void DeviceDisplay_OnMainDisplayInfoChanged(object? sender, DisplayInfoChangedEventArgs e)
        => UpdateDisplay();

    private void UpdateDisplay()
    {
        foreach (var view in new List<View> { ScrollViewTreeView, MapControl, CustomPickerKnownTileSource })
        {
            if (view.Parent is Grid grid) grid.Children.Remove(view);
        }

        var orientation = DeviceDisplay.MainDisplayInfo.Orientation;
        if (orientation is DisplayOrientation.Landscape)
        {
            AddToGrid(GridLandscape, ScrollViewTreeView, 0, 0, 2);
            AddToGrid(GridLandscape, MapControl, 0, 1);
            AddToGrid(GridLandscape, CustomPickerKnownTileSource, 1, 1);
        }
        else
        {
            AddToGrid(GridPortrait, CustomPickerKnownTileSource, 0, 0);
            AddToGrid(GridPortrait, MapControl, 1, 0);
            AddToGrid(GridPortrait, ScrollViewTreeView, 2, 0);
        }
    }

    private static void AddToGrid(Grid grid, View control, int row, int column, int rowSpan = 1, int columnSpan = 1)
    {
        grid.Children.Add(control);
        Grid.SetRow(control, row);
        Grid.SetColumn(control, column);

        Grid.SetRowSpan(control, rowSpan);
        Grid.SetColumnSpan(control, columnSpan);
    }

    private static (IEnumerable<TreeViewNode> TreeViewNodes, IEnumerable<TPlace> Places) GenerateTreeViewNodes()
    {
        using var context = new DataBaseContext();
        var places = context.TPlaces.OrderBy(s => s.Country).ThenBy(s => s.City).ThenBy(s => s.Name).ToList();
        var groups = places.GetGroups();

        var treeViewNodes = new List<TreeViewNode>();
        foreach (var group in groups)
        {
            var firstChildren = new List<TreeViewNode>();
            foreach (var cityGroup in group.CityGroups!)
            {
                var secondChildren = new List<TreeViewNode>();
                foreach (var place in cityGroup.Places!)
                {
                    var placeName = EmptyStringTreeViewConverter.ToUnknown(place.Name);
                    var placeNode = new TreeViewNode { Name = placeName, AdditionalData = place };
                    secondChildren.Add(placeNode);
                }

                var cityName = EmptyStringTreeViewConverter.ToUnknown(cityGroup.City);
                cityName = $"{cityName} [{secondChildren.Count}]";

                var cityNode = new TreeViewNode { Name = cityName, Children = secondChildren };
                firstChildren.Add(cityNode);
            }

            var countryName = EmptyStringTreeViewConverter.ToUnknown(group.Country);
            countryName = $"{countryName} [{firstChildren.Count}]";

            var groupNode = new TreeViewNode { Name = countryName, Children = firstChildren };
            treeViewNodes.Add(groupNode);
        }

        return (treeViewNodes, places);
    }
}