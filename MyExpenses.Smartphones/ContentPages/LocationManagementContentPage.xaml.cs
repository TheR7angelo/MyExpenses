using System.Collections.ObjectModel;
using BruTile.Predefined;
using CommunityToolkit.Maui.Views;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Manipulations;
using Mapsui.Projections;
using Mapsui.Tiling.Layers;
using Microsoft.Data.Sqlite;
using MyExpenses.Maui.Utils;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Maui.CustomPopup;
using MyExpenses.Models.Sql.Bases.Groups;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.SharedUtils.Resources.Resx.LocationManagement;
using MyExpenses.Smartphones.ContentPages.CustomPopups;
using MyExpenses.Smartphones.ContentPages.LocationManagement;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using MyExpenses.Utils.Maps;
using Serilog;
using Point = NetTopologySuite.Geometries.Point;

namespace MyExpenses.Smartphones.ContentPages;

public partial class LocationManagementContentPage
{
    public static readonly BindableProperty ComboBoxBasemapHintAssistProperty =
        BindableProperty.Create(nameof(ComboBoxBasemapHintAssist), typeof(string),
            typeof(LocationManagementContentPage));

    public string ComboBoxBasemapHintAssist
    {
        get => (string)GetValue(ComboBoxBasemapHintAssistProperty);
        set => SetValue(ComboBoxBasemapHintAssistProperty, value);
    }

    public static readonly BindableProperty SelectedTreeViewNodeProperty =
        BindableProperty.Create(nameof(SelectedTreeViewNode), typeof(TreeViewNode),
            typeof(LocationManagementContentPage), propertyChanged: SelectedTreeViewNode_OnChanged);

    public TreeViewNode? SelectedTreeViewNode
    {
        get => (TreeViewNode?)GetValue(SelectedTreeViewNodeProperty);
        set => SetValue(SelectedTreeViewNodeProperty, value);
    }

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
    // private WritableLayer PlaceLayer { get; } = new() { Tag = typeof(TPlace) };
    private IEnumerable<ILayer> InfoLayers { get; }

    private View[] Views { get; }

    private TPlace? ClickTPlace { get; set; }
    private Point ClickPoint { get; set; } = Point.Empty;
    private PointFeature? PointFeature { get; set; }

    public ObservableCollection<TreeViewNode> TreeViewNodes { get; }

    public LocationManagementContentPage()
    {
        KnownTileSources = [..MapsuiMapExtensions.GetAllKnowTileSource()];
        InfoLayers = [PlaceLayer];

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

        UpdateLanguage();
        InitializeComponent();

        Views = [ScrollViewTreeView, MapControl, PickerFieldKnownTileSource];
        MapControl.Map = map;
        MapControl.Map.Navigator.SetZoom(PlaceLayer);
        UpdateDisplay();

        // ReSharper disable once HeapView.ObjectAllocation.Possible
        // ReSharper disable once HeapView.DelegateAllocation
        DeviceDisplay.MainDisplayInfoChanged += DeviceDisplay_OnMainDisplayInfoChanged;
        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    #region Action

    private void DeviceDisplay_OnMainDisplayInfoChanged(object? sender, DisplayInfoChangedEventArgs e)
        => UpdateDisplay();

    private void Interface_OnLanguageChanged()
        => UpdateLanguage();

    private void MapControl_OnLoaded(object? sender, EventArgs e)
        => UpdateTileLayer();

    public void MapControl_OnInfo(object? sender, MapInfoEventArgs e)
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

    private void PickerFieldKnownTileSource_OnSelectedItemChanged(object? sender, object o)
        => UpdateTileLayer();

    private static void SelectedTreeViewNode_OnChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var sender = (LocationManagementContentPage)bindable;
        sender.UpdateMapZoom();
    }

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

    private void AddTreeViewNodePlace(TPlace place)
    {
        var countryName = Utils.Converters.EmptyStringTreeViewConverter.ToUnknown(place.Country);
        var cityName = Utils.Converters.EmptyStringTreeViewConverter.ToUnknown(place.City);

        var group = place.GetGroups();
        var cityNode = group.ToTreeViewNode().First();
        var countryNode = new TreeViewNode
        {
            Name = countryName,
            Children = [cityNode],
            AdditionalData = place.Country
        };

        var countryTreeViewNodes = TreeViewNodes.FirstOrDefault(s => Utils.Converters.EmptyStringTreeViewConverter.ToUnknown(s.AdditionalData).Equals(countryName));
        if (countryTreeViewNodes is null)
        {
            TreeViewNodes.AddAndSort(countryNode, s => s.Name!);
            return;
        }

        var cityTreeViewNodes = countryTreeViewNodes.Children.FirstOrDefault(s => Utils.Converters.EmptyStringTreeViewConverter.ToUnknown(s.AdditionalData).Equals(cityName));
        if (cityTreeViewNodes is null)
        {
            countryTreeViewNodes.Children.AddAndSort(cityNode, s => s.Name!);
            countryTreeViewNodes.Name = countryTreeViewNodes.ToFormatNodeName();
        }
        else
        {
            var placeNode = place.Name!.CreateTreeViewNode(additionalData: place);
            cityTreeViewNodes.Children.AddAndSort(placeNode, s=> s.Name!);
            cityTreeViewNodes.Name = cityTreeViewNodes.ToFormatNodeName();
        }


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

    private static List<TPlace> GetPlaces(TreeViewNode node)
    {
        var places = new List<TPlace>();
        foreach (var treeViewNode in node.Children)
        {
            var temp = GetPlaces(treeViewNode);
            places.AddRange(temp);
        }

        if (node.AdditionalData is TPlace place) places.Add(place);
        return places;
    }

    private async Task HandleDeleteFeature()
    {
        var message = string.Format(LocationManagementResources.MessageBoxDeleteQuestionMessage, ClickTPlace!.Name);
        var response = await DisplayAlert(LocationManagementResources.MessageBoxDeleteQuestionTitle, message,
            LocationManagementResources.MessageBoxDeleteQuestionYesButton,
            LocationManagementResources.MessageBoxDeleteQuestionCancelButton);

        if (!response) return;

        Log.Information("Attempting to remove the place \"{PlaceToDeleteName}\"", ClickTPlace.Name);
        var (success, exception) = ClickTPlace.Delete();

        if (success)
        {
            PlaceLayer.TryRemove(PointFeature!);
            MapControl.Refresh();
            RemoveTreeViewNodePlace(ClickTPlace);

            Log.Information("Place was successfully removed");
            await DisplayAlert(
                LocationManagementResources.MessageBoxMenuItemDeleteFeatureNoUseSuccessTitle,
                LocationManagementResources.MessageBoxMenuItemDeleteFeatureNoUseSuccessMessage,
                LocationManagementResources.MessageBoxMenuItemDeleteFeatureNoUseSuccessOkButton);
        }
        else if (exception!.InnerException is SqliteException
            {
                SqliteExtendedErrorCode: SQLitePCL.raw.SQLITE_CONSTRAINT_FOREIGNKEY
            })
        {
            Log.Error("Foreign key constraint violation");

            response = await DisplayAlert(LocationManagementResources.MessageBoxMenuItemDeleteFeatureUseQuestionTitle,
                LocationManagementResources.MessageBoxMenuItemDeleteFeatureUseQuestionMessage,
                LocationManagementResources.MessageBoxMenuItemDeleteFeatureUseQuestionYesButton,
                LocationManagementResources.MessageBoxMenuItemDeleteFeatureUseQuestionNoButton);

            if (!response) return;

            Log.Information("Attempting to remove the place \"{PlaceToDeleteName}\" with all relative element",
                ClickTPlace.Name);
            ClickTPlace.Delete(true);
            Log.Information("Place and all relative element was successfully removed");

            await DisplayAlert(LocationManagementResources.MessageBoxMenuItemDeleteFeatureUseSuccessTitle,
                LocationManagementResources.MessageBoxMenuItemDeleteFeatureUseSuccessMessage,
                LocationManagementResources.MessageBoxMenuItemDeleteFeatureUseSuccessOkButton);

            PlaceLayer.TryRemove(PointFeature!);
            MapControl.Refresh();
            RemoveTreeViewNodePlace(ClickTPlace);

            return;
        }

        Log.Error(exception, "An error occurred please retry");
        await DisplayAlert(LocationManagementResources.MessageBoxMenuItemDeleteFeatureErrorTitle,
            LocationManagementResources.MessageBoxMenuItemDeleteFeatureErrorMessage,
            LocationManagementResources.MessageBoxMenuItemDeleteFeatureErrorOkButton);
    }

    private async Task HandleLongTap()
    {
        var menuItemVisibility = ClickTPlace is null
            ? new MenuItemVisibility { MenuItemAddFeature = true }
            : new MenuItemVisibility { MenuItemEditFeature = true, MenuItemDeleteFeature = true };

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        var customPopupLocationManagement = new CustomPopupLocationManagement(menuItemVisibility, ClickPoint, ClickTPlace);
        await this.ShowPopupAsync(customPopupLocationManagement);

        AddEditLocationContentPage addEditLocationContentPage;
        bool success;
        var result = await customPopupLocationManagement.ResultDialog;
        switch (result)
        {
            case ECustomPopupLocationManagement.Delete:
                _ = HandleDeleteFeature();
                break;

            case ECustomPopupLocationManagement.Add:
                addEditLocationContentPage = new AddEditLocationContentPage();
                addEditLocationContentPage.SetPlace(ClickPoint);
                await addEditLocationContentPage.NavigateToAsync();
                success = await addEditLocationContentPage.ResultDialog;
                if (!success) return;
                success = await ProcessNewPlace(addEditLocationContentPage.Place, add: true);
                if (success) AddTreeViewNodePlace(addEditLocationContentPage.Place);
                break;

            case ECustomPopupLocationManagement.Edit:
                addEditLocationContentPage = new AddEditLocationContentPage();
                addEditLocationContentPage.SetPlace(ClickTPlace!, false);
                await addEditLocationContentPage.NavigateToAsync();
                success = await addEditLocationContentPage.ResultDialog;
                if (!success) return;

                var editedPlace = addEditLocationContentPage.Place;
                success = await ProcessNewPlace(editedPlace, edit: true);
                if (!success) return;

                RemoveTreeViewNodePlace(editedPlace);
                AddTreeViewNodePlace(editedPlace);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task<bool> ProcessNewPlace(TPlace newPlace, bool add = false, bool edit = false)
    {
        var (success, exception) = newPlace.AddOrEdit();
        if (success)
        {
            var feature = newPlace.IsOpen
                ? newPlace.ToFeature(MapsuiStyleExtensions.RedMarkerStyle)
                : newPlace.ToFeature(MapsuiStyleExtensions.BlueMarkerStyle);

            PlaceLayer.TryRemove(PointFeature!);
            PlaceLayer.Add(feature);
            MapControl.Refresh();

            // string json;
            switch (add)
            {
                case true when !edit:
                    await DisplayAlert("Success", LocationManagementResources.MessageBoxProcessNewPlaceAddSuccess, "Ok");
                    Log.Information("The new place was successfully added");

                    // Loop crash
                    // json = newPlace.ToJsonString();
                    // Log.Information("{Json}", json);

                    break;
                case false when edit:
                    await DisplayAlert("Success", LocationManagementResources.MessageBoxProcessNewPlaceEditSuccess, "Ok");
                    Log.Information("The new place was successfully edited");

                    // Loop crash
                    // json = newPlace.ToJsonString();
                    // Log.Information("{Json}", json);

                    break;
            }

            return true;
        }

        Log.Error(exception, "An error occurred please retry");
        await DisplayAlert("Error", LocationManagementResources.MessageBoxProcessNewPlaceError, "Ok");

        return false;
    }

    private void RemoveTreeViewNodePlace(TPlace place)
    {
        var countryName = Utils.Converters.EmptyStringTreeViewConverter.ToUnknown(place.Country);
        var cityName = Utils.Converters.EmptyStringTreeViewConverter.ToUnknown(place.City);

        var countryTreeViewNodes = TreeViewNodes.First(s => Utils.Converters.EmptyStringTreeViewConverter.ToUnknown(s.AdditionalData).Equals(countryName));
        var cityTreeViewNodes = countryTreeViewNodes.Children.First(s => Utils.Converters.EmptyStringTreeViewConverter.ToUnknown(s.AdditionalData).Equals(cityName));
        var placeTreeViewNodes = cityTreeViewNodes.Children.First(s => s.Name!.Equals(ClickTPlace!.Name));

        cityTreeViewNodes.Children.Remove(placeTreeViewNodes);
        if (cityTreeViewNodes.Children.Count is 0) countryTreeViewNodes.Children.Remove(cityTreeViewNodes);
        else cityTreeViewNodes.Name = cityTreeViewNodes.ToFormatNodeName();

        if (countryTreeViewNodes.Children.Count is 0) TreeViewNodes.Remove(countryTreeViewNodes);
        else countryTreeViewNodes.Name = countryTreeViewNodes.ToFormatNodeName();
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

    private void UpdateLanguage()
    {
        ComboBoxBasemapHintAssist = LocationManagementResources.ComboBoxBasemapHintAssist;
    }

    private void UpdateMapZoom()
    {
        if (SelectedTreeViewNode is null) return;

        var points = GetPlaces(SelectedTreeViewNode).Select(s => s.ToMPoint());
        MapControl.Map.Navigator.SetZoom(points.ToArray());
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
}