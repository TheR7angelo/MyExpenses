using System.Collections.ObjectModel;
using System.Reflection;
using BruTile.Predefined;
using CommunityToolkit.Maui.Views;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Manipulations;
using Mapsui.Projections;
using Mapsui.Tiling.Layers;
using MyExpenses.Maui.Utils;
using MyExpenses.Maui.Utils.Maps;
using MyExpenses.Models.Maui.CustomPopup;
using MyExpenses.Models.Sql.Bases.Groups;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Smartphones.ContentPages.CustomPopups;
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

    private View[] Views { get; }

    private TPlace? ClickTPlace { get; set; }
    private Point ClickPoint { get; set; } = Point.Empty;
    private PointFeature? PointFeature { get; set; }

    public ObservableCollection<TreeViewNode> TreeViewNodes { get; }

    public LocationManagementContentPage()
    {
        KnownTileSources = [..MapsuiMapExtensions.GetAllKnowTileSource()];
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        InfoLayers = new List<ILayer> { PlaceLayer };

        var (treeViewNodes, places) = GenerateTreeViewNodes();
        TreeViewNodes = [..treeViewNodes];

        var assembly = Assembly.GetExecutingAssembly()!;
        var resources = assembly.GetManifestResourceNames().Where(s => s.EndsWith(".svg"));

        foreach (var resource in resources)
        {
            _ = DisplayAlert("Resource", resource, "OK");
        }

        try
        {
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

            Views = [ScrollViewTreeView, MapControl, PickerFieldKnownTileSource];

            // MapControl.Map = map;
            MapControl.SetZoom(PlaceLayer);
            UpdateDisplay();
        }
        catch (Exception e)
        {
            _ = DisplayAlert("Error", e.Message, "OK");
        }

        // ReSharper disable HeapView.DelegateAllocation
        // MapControl.Map.Tapped += MapControl_OnTapped;
        DeviceDisplay.MainDisplayInfoChanged += DeviceDisplay_OnMainDisplayInfoChanged;
        // ReSharper restore HeapView.DelegateAllocation
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
        var groups = places.GetGroups().ToArray();
        var treeViewNodes = groups.ToTreeViewNode();

        return (treeViewNodes, places);
    }

    private void UpdateDisplay()
    {
        foreach (var view in Views)
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

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        var tileLayer = new TileLayer(httpTileSource) { Name = layerName };

        var layers = MapControl?.Map.Layers.FindLayer(layerName);
        if (layers is not null) MapControl?.Map.Layers.Remove(layers.ToArray());

        MapControl?.Map.Layers.Insert(0, tileLayer);
    }

    #endregion

    // private bool MapControl_OnTapped(Mapsui.Map sender, MapEventArgs e)
    // {
    //     var mapInfo = e.GetMapInfo(InfoLayers);
    //     SetClickTPlace(mapInfo);
    //
    //     var worldPosition = e.WorldPosition;
    //     var lonLat = SphericalMercator.ToLonLat(worldPosition.X, worldPosition.Y);
    //
    //     // ReSharper disable once HeapView.ObjectAllocation.Evident
    //     // The ClickPoint instance is used to store the coordinates of the point clicked on the map.
    //     ClickPoint = new Point(lonLat.lon, lonLat.lat);
    //
    //     if (e.GestureType is GestureType.LongPress) _ = HandleLongTap();
    //     return true;
    // }

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

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        var customPopupLocationManagement = new CustomPopupLocationManagement(menuItemVisibility, ClickPoint, ClickTPlace);
        await this.ShowPopupAsync(customPopupLocationManagement);
    }

    private void MapControl_OnInfo(object? sender, MapInfoEventArgs e)
    {
        var mapInfo = e.GetMapInfo(InfoLayers);
        SetClickTPlace(mapInfo);

        var worldPosition = e.WorldPosition;
        var lonLat = SphericalMercator.ToLonLat(worldPosition.X, worldPosition.Y);

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The ClickPoint instance is used to store the coordinates of the point clicked on the map.
        ClickPoint = new Point(lonLat.lon, lonLat.lat);

        if (e.GestureType is GestureType.LongPress) _ = HandleLongTap();
    }
}