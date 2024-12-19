using System.Collections.ObjectModel;
using System.Windows.Input;
using BruTile.Predefined;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Tiling.Layers;
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

    public ICommand MapControlLongPressCommand { get; }

    public LocationManagementContentPage()
    {
        MapControlLongPressCommand = new Command<object>(MapControlLong_OnLongPress);

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
        foreach (var view in new List<View> { ScrollViewTreeView, GridMapControl, PickerFieldKnownTileSource })
        {
            if (view.Parent is Grid grid) grid.Children.Remove(view);
        }

        var orientation = DeviceDisplay.MainDisplayInfo.Orientation;
        if (orientation is DisplayOrientation.Landscape)
        {
            AddToGrid(GridLandscape, ScrollViewTreeView, 0, 0, 2);
            AddToGrid(GridLandscape, GridMapControl, 0, 1);
            AddToGrid(GridLandscape, PickerFieldKnownTileSource, 1, 1);
        }
        else
        {
            AddToGrid(GridPortrait, PickerFieldKnownTileSource, 0, 0);
            AddToGrid(GridPortrait, GridMapControl, 1, 0);
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

    private void MapControl_OnInfo(object? sender, MapInfoEventArgs e)
    {
        var mapInfo = e.MapInfo;
        SetClickTPlace(mapInfo);
    }

    private void SetClickTPlace(MapInfo mapInfo)
    {
        var feature = mapInfo.Feature as PointFeature;
        var layer = mapInfo.Layer;

        if (feature is null || layer is null)
        {
            // MenuItemAddFeature.Visibility = Visibility.Visible;
            // MenuItemEditFeature.Visibility = Visibility.Collapsed;
            // MenuItemDeleteFeature.Visibility = Visibility.Collapsed;
            ClickTPlace = null;
            return;
        }

        // MenuItemAddFeature.Visibility = Visibility.Collapsed;
        // MenuItemEditFeature.Visibility = Visibility.Visible;
        // MenuItemDeleteFeature.Visibility = Visibility.Visible;

        var type = (Type)layer.Tag!;
        if (type != typeof(TPlace)) return;

        PointFeature = feature;
        var place = feature.ToTPlace();
        ClickTPlace = place;
    }

    private async void MapControlLong_OnLongPress(object e)
    {
        // if (e.Type == TouchActionType.Pressed || e.Type == TouchActionType.Moved)
        // {
        //     // Si le mouvement ou le clic se produit sur le MapControl, laissez-le gérer l'interaction
        //     var mapControlBounds = MapControl.GetBoundingBox(); // Extension pour obtenir les limites
        //     if (mapControlBounds.Contains(e.Location))
        //     {
        //         // Transférer les événements au MapControl
        //         MapControl.OnTouch(e); // Appel natif à la carte pour déplacer/zoomer
        //         return;
        //     }
        // }

        await DisplayAlert("Long press", "Long press", "OK");
    }
}