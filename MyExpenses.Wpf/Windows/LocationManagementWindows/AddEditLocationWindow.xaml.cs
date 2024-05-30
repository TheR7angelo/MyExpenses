using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BruTile.Predefined;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Styles;
using Mapsui.Tiling.Layers;
using Microsoft.Data.Sqlite;
using MyExpenses.Models.AutoMapper;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Models.WebApi.Nominatim;
using MyExpenses.Sql.Context;
using MyExpenses.WebApi.Nominatim;
using MyExpenses.Wpf.Resources.Resx.Windows.AddEditLocationWindow;
using MyExpenses.Wpf.Utils;
using MyExpenses.Wpf.Utils.Maps;
using MyExpenses.Wpf.Windows.MsgBox;
using Serilog;
using Point = NetTopologySuite.Geometries.Point;

namespace MyExpenses.Wpf.Windows.LocationManagementWindows;

public partial class AddEditLocationWindow
{
    public static readonly DependencyProperty EditPlaceProperty = DependencyProperty.Register(nameof(EditPlace),
        typeof(bool), typeof(AddEditLocationWindow), new PropertyMetadata(default(bool)));

    public bool EditPlace
    {
        get => (bool)GetValue(EditPlaceProperty);
        set => SetValue(EditPlaceProperty, value);
    }

    public string TextBoxCityHintAssist { get; } = AddEditLocationWindowResources.TextBoxCityHintAssist;
    public string TextBoxCountryHintAssist { get; } = AddEditLocationWindowResources.TextBoxCountryHintAssist;
    public string TextBoxLatitudeHintAssist { get; } = AddEditLocationWindowResources.TextBoxLatitudeHintAssist;
    public string TextBoxLongitudeHintAssist { get; } = AddEditLocationWindowResources.TextBoxLongitudeHintAssist;
    public string TextBoxNameHintAssist { get; } = AddEditLocationWindowResources.TextBoxNameHintAssist;
    public string TextBoxNumberHintAssist { get; } = AddEditLocationWindowResources.TextBoxNumberHintAssist;
    public string TextBoxPostalCodeHintAssist { get; } = AddEditLocationWindowResources.TextBoxPostalCodeHintAssist;
    public string TextBoxStreetHintAssist { get; } = AddEditLocationWindowResources.TextBoxStreetHintAssist;
    public string ButtonContentValidNewPoint { get; } = AddEditLocationWindowResources.ButtonContentValidNewPoint;
    public string ButtonContentZoomToPoint { get; } = AddEditLocationWindowResources.ButtonContentZoomToPoint;
    public string ButtonContentSearchByAddress { get; } = AddEditLocationWindowResources.ButtonContentSearchByAddress;

    public string ButtonContentSearchByCoordinate { get; } = AddEditLocationWindowResources.ButtonContentSearchByCoordinate;

    public string ButtonContentCancel { get; } = AddEditLocationWindowResources.ButtonContentCancel;
    public string ButtonContentDelete { get; } = AddEditLocationWindowResources.ButtonContentDelete;
    public string ButtonContentValid { get; } = AddEditLocationWindowResources.ButtonContentValid;

    #region Properties

    private const string ColumnTemp = "temp";
    public TPlace Place { get; } = new();
    public bool PlaceDeleted { get; set; }
    private WritableLayer WritableLayer { get; } = new() { Style = null };
    public List<KnownTileSource> KnownTileSources { get; }

    public KnownTileSource KnownTileSourceSelected { get; set; }

    #endregion

    public AddEditLocationWindow()
    {
        KnownTileSources = [..MapsuiMapExtensions.GetAllKnowTileSource()];

        // TODO add listener color change
        var brush = (SolidColorBrush)FindResource("MaterialDesignPaper");
        var backColor = brush.ToMapsuiColor();
        var map = MapsuiMapExtensions.GetMap(true, backColor);
        map.Layers.Add(WritableLayer);

        InitializeComponent();

        MapControl.Map = map;
    }

    #region Action

    #region Button

    private void ButtonSearchByAddress_OnClick(object sender, RoutedEventArgs e)
    {
        var address = Place.ToString();
        Log.Information("Using the nominatim API to search via an address : \"{Address}\"", address);

        var nominatimSearchResults = address.ToNominatim()?.ToList() ?? [];

        Log.Information("The API returned \"{Count}\" result(s)", nominatimSearchResults.Count);
        HandleNominatimResult(nominatimSearchResults);
    }

    private void ButtonSearchByCoordinate_OnClick(object sender, RoutedEventArgs e)
    {
        var point = Place.Geometry;
        Log.Information("Using the nominatim API to search via a point : {Point}", point);

        var nominatimSearchResult = point.ToNominatim();

        var mapper = Mapping.Mapper;
        var newPlace = mapper.Map<TPlace>(nominatimSearchResult);
        if (newPlace is null)
        {
            Log.Information("The API returned no result(s)");

            MsgBox.MsgBox.Show(AddEditLocationWindowResources.ButtonSearchByCoordinateMessageBoxError,
                MsgBoxImage.Error);
            return;
        }

        Log.Information("The API returned one result");

        newPlace.Id = Place.Id;
        newPlace.DateAdded = Place.DateAdded ?? newPlace.DateAdded;
        SetPlace(newPlace, true);
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
        newFeature.Styles = new List<IStyle> { MapsuiStyleExtensions.RedMarkerStyle };
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

    private void ButtonDelete_OnClick(object sender, RoutedEventArgs e)
    {
        var response = MsgBox.MsgBox.Show(AddEditLocationWindowResources.MessageBoxDeleteQuestion,
            MsgBoxImage.Question, MessageBoxButton.YesNoCancel);
        if (response is not MessageBoxResult.Yes) return;

        Log.Information("Attempting to remove the place \"{PlaceToDeleteName}\"", Place.Name);
        var (success, exception) = Place.Delete();
        if (success)
        {
            Log.Information("Place was successfully removed");
            MsgBox.MsgBox.Show(AddEditLocationWindowResources.MessageBoxDeletePlaceNoUseSuccess, MsgBoxImage.Check);

            PlaceDeleted = true;
            DialogResult = true;

            Close();
            return;
        }

        if (exception!.InnerException is SqliteException
            {
                SqliteExtendedErrorCode: SQLitePCL.raw.SQLITE_CONSTRAINT_FOREIGNKEY
            })
        {
            Log.Error("Foreign key constraint violation");

            response = MsgBox.MsgBox.Show(AddEditLocationWindowResources.MessageBoxDeletePlaceUseQuestion,
                MsgBoxImage.Question, MessageBoxButton.YesNoCancel);

            if (response != MessageBoxResult.Yes) return;

            Log.Information("Attempting to remove the place \"{PlaceToDeleteName}\" with all relative element",
                Place.Name);
            Place.Delete(true);
            Log.Information("Place and all relative element was successfully removed");
            MsgBox.MsgBox.Show(AddEditLocationWindowResources.MessageBoxDeletePlaceUseSuccess, MsgBoxImage.Check);

            PlaceDeleted = true;
            DialogResult = true;
            Close();

            return;
        }

        Log.Error(exception, "An error occurred please retry");
        MsgBox.MsgBox.Show(AddEditLocationWindowResources.MessageBoxDeletePlaceError, MsgBoxImage.Error);
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
        var feature = new PointFeature(worldPosition)
            { Styles = new List<IStyle> { MapsuiStyleExtensions.GreenMarkerStyle } };
        feature[ColumnTemp] = true;

        var oldFeature = WritableLayer.GetFeatures().FirstOrDefault(f => f[ColumnTemp]!.Equals(true));
        if (oldFeature is not null) WritableLayer.TryRemove(oldFeature);

        WritableLayer.Add(feature);
        MapControl.Map.Refresh();
    }

    private void MapControl_OnLoaded(object sender, RoutedEventArgs e)
        => UpdateTileLayer();

    private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        => UpdateTileLayer();

    #endregion

    #region Function

    private void HandleNominatimResult(IReadOnlyCollection<NominatimSearchResult> nominatimSearchResults)
    {
        TPlace? place = null;

        var mapper = Mapping.Mapper;
        switch (nominatimSearchResults.Count)
        {
            case 0:
                MsgBox.MsgBox.Show(AddEditLocationWindowResources.HandleNominatimResultZeroResult,
                    MsgBoxImage.Exclamation);
                break;
            case 1:
                MsgBox.MsgBox.Show(AddEditLocationWindowResources.HandleNominatimResultOneResult,
                    MsgBoxImage.Check);
                var nominatimSearchResult = nominatimSearchResults.First();
                place = mapper.Map<TPlace>(nominatimSearchResult);
                break;
            case > 1:
                MsgBox.MsgBox.Show(AddEditLocationWindowResources.HandleNominatimResultMultipleResult,
                    MsgBoxImage.Information);

                var places = nominatimSearchResults.Select(s => mapper.Map<TPlace>(s));
                var nominatimSearchWindows = new NominatimSearchWindows();
                nominatimSearchWindows.AddRange(places);
                nominatimSearchWindows.ShowDialog();

                if (!nominatimSearchWindows.DialogResult.Equals(true)) return;

                place = mapper.Map<TPlace>(nominatimSearchWindows.CurrentPlace);
                break;
        }

        if (place is null) return;
        SetPlace(place, true);
    }

    public void SetPlace(TPlace newTPlace, bool clear)
    {
        if (clear) WritableLayer.Clear();

        newTPlace.CopyPropertiesTo(Place);
        UpdateMiniMap();
        EditPlace = true;
    }

    public void SetPlace(Point point)
    {
        var nominatim = point.ToNominatim();
        if (nominatim is not null)
        {
            var mapper = Mapping.Mapper;
            var place = mapper.Map<TPlace>(nominatim);
            place.CopyPropertiesTo(Place);
        }
        else
        {
            Place.Geometry = point;
        }

        UpdateMiniMap();
    }

    private void UpdateMiniMap()
    {
        var feature = Place.ToFeature();
        feature.Styles = new List<IStyle> { MapsuiStyleExtensions.RedMarkerStyle };
        feature[ColumnTemp] = false;

        WritableLayer.Add(feature);

        MapControl.Map.Home = n => { n.CenterOnAndZoomTo(feature.Point, 1); };
        MapControl.Map.Navigator.CenterOn(feature.Point);
        MapControl.Map.Navigator.ZoomTo(1);
        MapControl.Refresh();
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

    private void ZoomToMPoint(MPoint mPoint)
    {
        MapControl.Map.Navigator.CenterOn(mPoint);
        MapControl.Map.Navigator.ZoomTo(1);
    }

    #endregion
}