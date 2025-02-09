using System.Collections.ObjectModel;
using BruTile.Predefined;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Manipulations;
using Mapsui.Projections;
using Mapsui.Tiling.Layers;
using MyExpenses.Models.Maui.CustomPopup;
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

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    // A new WritableLayer instance is intentionally allocated here to represent the layer
    // dedicated to places (TPlace). This layer acts as a container for displaying map features
    // related to places and provides the flexibility to dynamically add or remove features
    // as needed. By creating a unique instance for each `DetailedRecordContentPage`, we
    // ensure that map layers remain properly isolated and do not interfere with layers
    // managed by other pages or components in the application.
    private WritableLayer PlaceLayer { get; } = new() { Style = null, Tag = typeof(TPlace) };
    private IEnumerable<ILayer> InfoLayers { get; }

    private TPlace? ClickTPlace { get; set; }
    private Point ClickPoint { get; set; } = Point.Empty;
    private PointFeature? PointFeature { get; set; }

    public ObservableCollection<TreeViewNode> TreeViewNodes { get; }

    public LocationManagementContentPage()
    {
        KnownTileSources = [..MapsuiMapExtensions.GetAllKnowTileSource()];
        InfoLayers = new List<ILayer> { PlaceLayer };

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

        // ReSharper disable once HeapView.DelegateAllocation
        DeviceDisplay.MainDisplayInfoChanged += DeviceDisplay_OnMainDisplayInfoChanged;
    }

    #region Action

    private void DeviceDisplay_OnMainDisplayInfoChanged(object? sender, DisplayInfoChangedEventArgs e)
        => UpdateDisplay();

    private void MapControl_OnLoaded(object? sender, EventArgs e)
        => UpdateTileLayer();

    private void PickerFieldKnownTileSource_OnSelectedItemChanged(object? sender, object o)
        => UpdateTileLayer();

    #endregion

    #region Function

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
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The creation of a new DataBaseContext instance (via `new DataBaseContext()`) is necessary to interact with the database.
        // This context provides the connection to the database and allows querying or updating data.
        // The `using` statement ensures that the context is disposed of properly after its use, freeing up resources like database connections.
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

    private void UpdateDisplay()
    {
        foreach (var view in new List<View> { ScrollViewTreeView, MapControl, PickerFieldKnownTileSource })
        {
            if (view.Parent is Grid grid) grid.Children.Remove(view);
        }

        var orientation = DeviceDisplay.MainDisplayInfo.Orientation;
        if (orientation is DisplayOrientation.Landscape)
        {
            AddToGrid(GridLandscape, ScrollViewTreeView, 0, 0, 2);
            AddToGrid(GridLandscape, MapControl, 0, 1);
            AddToGrid(GridLandscape, PickerFieldKnownTileSource, 1, 1);
        }
        else
        {
            AddToGrid(GridPortrait, PickerFieldKnownTileSource, 0, 0);
            AddToGrid(GridPortrait, MapControl, 1, 0);
            AddToGrid(GridPortrait, ScrollViewTreeView, 2, 0);
        }
    }

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

    #endregion

    // Before beta 8
    // https://mapsui.com/v5/nuget-of-latest-build/
    private void MapControl_OnInfo(object? sender, MapInfoEventArgs e)
    {
        var mapInfo = e.GetMapInfo(InfoLayers);
        SetClickTPlace(mapInfo);

        var worldPosition = e.WorldPosition;
        var lonLat = SphericalMercator.ToLonLat(worldPosition.X, worldPosition.Y);

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The ClickPoint instance is used to store the coordinates of the point clicked on the map.
        ClickPoint = new Point(lonLat.lon, lonLat.lat);

        if (e.TapType is TapType.Long) _ = HandleLongTap();
    }

    private void SetClickTPlace(MapInfo mapInfo)
    {
        if (mapInfo.Feature is not PointFeature pointFeature || mapInfo.Layer?.Tag is not Type layerType || layerType != typeof(TPlace))
        {
            ClickTPlace = null;
            return;
        }

        PointFeature = pointFeature;
        var place = pointFeature.ToTPlace();
        ClickTPlace = place;
    }

    private async Task HandleLongTap()
    {
        var menuItemVisibility = ClickTPlace is null
            ? new MenuItemVisibility { MenuItemAddFeature = true }
            : new MenuItemVisibility { MenuItemEditFeature = true, MenuItemDeleteFeature = true };

        var content = $"{nameof(MenuItemVisibility.MenuItemAddFeature)}: {menuItemVisibility.MenuItemAddFeature}, " +
                      $"{nameof(MenuItemVisibility.MenuItemEditFeature)}: {menuItemVisibility.MenuItemEditFeature}, " +
                      $"{nameof(MenuItemVisibility.MenuItemDeleteFeature)}: {menuItemVisibility.MenuItemDeleteFeature})";
        await DisplayAlert("Long tap", content, "OK");
    }
}