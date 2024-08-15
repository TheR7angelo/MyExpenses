using System.Windows;
using System.Windows.Controls;
using BruTile.Predefined;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Styles;
using Mapsui.Tiling.Layers;
using Microsoft.Data.Sqlite;
using MyExpenses.Models.AutoMapper;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Tables;
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
    #region Properties

    public static readonly DependencyProperty EditPlaceProperty = DependencyProperty.Register(nameof(EditPlace),
        typeof(bool), typeof(AddEditLocationWindow), new PropertyMetadata(default(bool)));

    public bool EditPlace
    {
        get => (bool)GetValue(EditPlaceProperty);
        set => SetValue(EditPlaceProperty, value);
    }

    public static readonly DependencyProperty TextBoxNameHintAssistProperty =
        DependencyProperty.Register(nameof(TextBoxNameHintAssist), typeof(string), typeof(AddEditLocationWindow),
            new PropertyMetadata(default(string)));

    public string TextBoxNameHintAssist
    {
        get => (string)GetValue(TextBoxNameHintAssistProperty);
        set => SetValue(TextBoxNameHintAssistProperty, value);
    }

    public static readonly DependencyProperty TextBoxNumberHintAssistProperty =
        DependencyProperty.Register(nameof(TextBoxNumberHintAssist), typeof(string), typeof(AddEditLocationWindow),
            new PropertyMetadata(default(string)));

    public string TextBoxNumberHintAssist
    {
        get => (string)GetValue(TextBoxNumberHintAssistProperty);
        set => SetValue(TextBoxNumberHintAssistProperty, value);
    }

    public static readonly DependencyProperty CheckBoxContentIsOpenProperty =
        DependencyProperty.Register(nameof(CheckBoxContentIsOpen), typeof(string), typeof(AddEditLocationWindow),
            new PropertyMetadata(default(string)));

    public string CheckBoxContentIsOpen
    {
        get => (string)GetValue(CheckBoxContentIsOpenProperty);
        set => SetValue(CheckBoxContentIsOpenProperty, value);
    }

    public static readonly DependencyProperty TextBoxStreetHintAssistProperty =
        DependencyProperty.Register(nameof(TextBoxStreetHintAssist), typeof(string), typeof(AddEditLocationWindow),
            new PropertyMetadata(default(string)));

    public string TextBoxStreetHintAssist
    {
        get => (string)GetValue(TextBoxStreetHintAssistProperty);
        set => SetValue(TextBoxStreetHintAssistProperty, value);
    }

    public static readonly DependencyProperty TextBoxPostalCodeHintAssistProperty =
        DependencyProperty.Register(nameof(TextBoxPostalCodeHintAssist), typeof(string), typeof(AddEditLocationWindow),
            new PropertyMetadata(default(string)));

    public string TextBoxPostalCodeHintAssist
    {
        get => (string)GetValue(TextBoxPostalCodeHintAssistProperty);
        set => SetValue(TextBoxPostalCodeHintAssistProperty, value);
    }

    public static readonly DependencyProperty TextBoxCityHintAssistProperty =
        DependencyProperty.Register(nameof(TextBoxCityHintAssist), typeof(string), typeof(AddEditLocationWindow),
            new PropertyMetadata(default(string)));

    public string TextBoxCityHintAssist
    {
        get => (string)GetValue(TextBoxCityHintAssistProperty);
        set => SetValue(TextBoxCityHintAssistProperty, value);
    }

    public static readonly DependencyProperty TextBoxCountryHintAssistProperty =
        DependencyProperty.Register(nameof(TextBoxCountryHintAssist), typeof(string), typeof(AddEditLocationWindow),
            new PropertyMetadata(default(string)));

    public string TextBoxCountryHintAssist
    {
        get => (string)GetValue(TextBoxCountryHintAssistProperty);
        set => SetValue(TextBoxCountryHintAssistProperty, value);
    }

    public static readonly DependencyProperty TextBoxLatitudeHintAssistProperty =
        DependencyProperty.Register(nameof(TextBoxLatitudeHintAssist), typeof(string), typeof(AddEditLocationWindow),
            new PropertyMetadata(default(string)));

    public string TextBoxLatitudeHintAssist
    {
        get => (string)GetValue(TextBoxLatitudeHintAssistProperty);
        set => SetValue(TextBoxLatitudeHintAssistProperty, value);
    }

    public static readonly DependencyProperty ButtonContentValidNewPointProperty =
        DependencyProperty.Register(nameof(ButtonContentValidNewPoint), typeof(string), typeof(AddEditLocationWindow),
            new PropertyMetadata(default(string)));

    public string ButtonContentValidNewPoint
    {
        get => (string)GetValue(ButtonContentValidNewPointProperty);
        set => SetValue(ButtonContentValidNewPointProperty, value);
    }

    public static readonly DependencyProperty ButtonContentZoomToPointProperty =
        DependencyProperty.Register(nameof(ButtonContentZoomToPoint), typeof(string), typeof(AddEditLocationWindow),
            new PropertyMetadata(default(string)));

    public string ButtonContentZoomToPoint
    {
        get => (string)GetValue(ButtonContentZoomToPointProperty);
        set => SetValue(ButtonContentZoomToPointProperty, value);
    }

    public static readonly DependencyProperty TextBoxLongitudeHintAssistProperty =
        DependencyProperty.Register(nameof(TextBoxLongitudeHintAssist), typeof(string), typeof(AddEditLocationWindow),
            new PropertyMetadata(default(string)));

    public string TextBoxLongitudeHintAssist
    {
        get => (string)GetValue(TextBoxLongitudeHintAssistProperty);
        set => SetValue(TextBoxLongitudeHintAssistProperty, value);
    }

    public static readonly DependencyProperty ComboBoxBackgroundHintAssistProperty =
        DependencyProperty.Register(nameof(ComboBoxBackgroundHintAssist), typeof(string), typeof(AddEditLocationWindow),
            new PropertyMetadata(default(string)));

    public string ComboBoxBackgroundHintAssist
    {
        get => (string)GetValue(ComboBoxBackgroundHintAssistProperty);
        set => SetValue(ComboBoxBackgroundHintAssistProperty, value);
    }

    public static readonly DependencyProperty ButtonContentSearchByAddressProperty =
        DependencyProperty.Register(nameof(ButtonContentSearchByAddress), typeof(string), typeof(AddEditLocationWindow),
            new PropertyMetadata(default(string)));

    public string ButtonContentSearchByAddress
    {
        get => (string)GetValue(ButtonContentSearchByAddressProperty);
        set => SetValue(ButtonContentSearchByAddressProperty, value);
    }

    public static readonly DependencyProperty ButtonContentSearchByCoordinateProperty =
        DependencyProperty.Register(nameof(ButtonContentSearchByCoordinate), typeof(string),
            typeof(AddEditLocationWindow), new PropertyMetadata(default(string)));

    public string ButtonContentSearchByCoordinate
    {
        get => (string)GetValue(ButtonContentSearchByCoordinateProperty);
        set => SetValue(ButtonContentSearchByCoordinateProperty, value);
    }

    public static readonly DependencyProperty ButtonContentValidProperty =
        DependencyProperty.Register(nameof(ButtonContentValid), typeof(string), typeof(AddEditLocationWindow),
            new PropertyMetadata(default(string)));

    public string ButtonContentValid
    {
        get => (string)GetValue(ButtonContentValidProperty);
        set => SetValue(ButtonContentValidProperty, value);
    }

    public static readonly DependencyProperty ButtonContentDeleteProperty =
        DependencyProperty.Register(nameof(ButtonContentDelete), typeof(string), typeof(AddEditLocationWindow),
            new PropertyMetadata(default(string)));

    public string ButtonContentDelete
    {
        get => (string)GetValue(ButtonContentDeleteProperty);
        set => SetValue(ButtonContentDeleteProperty, value);
    }

    public static readonly DependencyProperty ButtonContentCancelProperty =
        DependencyProperty.Register(nameof(ButtonContentCancel), typeof(string), typeof(AddEditLocationWindow),
            new PropertyMetadata(default(string)));

    public string ButtonContentCancel
    {
        get => (string)GetValue(ButtonContentCancelProperty);
        set => SetValue(ButtonContentCancelProperty, value);
    }

    public static readonly DependencyProperty TitleWindowProperty = DependencyProperty.Register(nameof(TitleWindow),
        typeof(string), typeof(AddEditLocationWindow), new PropertyMetadata(default(string)));

    public string TitleWindow
    {
        get => (string)GetValue(TitleWindowProperty);
        set => SetValue(TitleWindowProperty, value);
    }

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

        var backColor = Utils.Resources.GetMaterialDesignPaperMapsUiStylesColor();
        var map = MapsuiMapExtensions.GetMap(true, backColor);
        map.Layers.Add(WritableLayer);

        Interface.ThemeChanged += Interface_OnThemeChanged;
        Interface.LanguageChanged += Interface_OnLanguageChanged;

        UpdateLanguage();
        InitializeComponent();

        this.SetWindowCornerPreference();

        MapControl.Map = map;
    }

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
        => UpdateLanguage();

    private void UpdateLanguage()
    {
        TitleWindow = AddEditLocationWindowResources.TitleWindow;

        TextBoxCityHintAssist = AddEditLocationWindowResources.TextBoxCityHintAssist;
        TextBoxCountryHintAssist = AddEditLocationWindowResources.TextBoxCountryHintAssist;
        TextBoxLatitudeHintAssist = AddEditLocationWindowResources.TextBoxLatitudeHintAssist;
        TextBoxLongitudeHintAssist = AddEditLocationWindowResources.TextBoxLongitudeHintAssist;
        TextBoxNameHintAssist = AddEditLocationWindowResources.TextBoxNameHintAssist;
        TextBoxNumberHintAssist = AddEditLocationWindowResources.TextBoxNumberHintAssist;
        TextBoxPostalCodeHintAssist = AddEditLocationWindowResources.TextBoxPostalCodeHintAssist;
        TextBoxStreetHintAssist = AddEditLocationWindowResources.TextBoxStreetHintAssist;
        ButtonContentValidNewPoint = AddEditLocationWindowResources.ButtonContentValidNewPoint;
        ButtonContentZoomToPoint = AddEditLocationWindowResources.ButtonContentZoomToPoint;
        ButtonContentSearchByAddress = AddEditLocationWindowResources.ButtonContentSearchByAddress;

        ButtonContentSearchByCoordinate = AddEditLocationWindowResources.ButtonContentSearchByCoordinate;

        ButtonContentCancel = AddEditLocationWindowResources.ButtonContentCancel;
        ButtonContentDelete = AddEditLocationWindowResources.ButtonContentDelete;
        ButtonContentValid = AddEditLocationWindowResources.ButtonContentValid;

        CheckBoxContentIsOpen = AddEditLocationWindowResources.CheckBoxContentIsOpen;
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

        var nominatimSearchResult = point?.ToNominatim();

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

            if (response is not MessageBoxResult.Yes) return;

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

    private void Interface_OnThemeChanged(object sender, ConfigurationThemeChangedEventArgs e)
    {
        var backColor = Utils.Resources.GetMaterialDesignPaperMapsUiStylesColor();
        MapControl.Map.BackColor = backColor;
    }

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
                var nominatimSearchWindows = new NominatimSearchWindow();
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