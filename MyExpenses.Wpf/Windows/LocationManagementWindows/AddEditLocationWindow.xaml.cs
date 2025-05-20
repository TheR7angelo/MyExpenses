using System.Windows;
using System.Windows.Controls;
using BruTile.Predefined;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Tiling.Layers;
using Microsoft.Data.Sqlite;
using MyExpenses.Models.AutoMapper;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Mapsui.PointFeatures;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Models.WebApi.Nominatim;
using MyExpenses.SharedUtils.Properties;
using MyExpenses.SharedUtils.Resources.Resx.AddEditLocation;
using MyExpenses.Sql.Context;
using MyExpenses.Utils.Maps;
using MyExpenses.WebApi.Nominatim;
using MyExpenses.Wpf.Utils;
using MyExpenses.Wpf.Windows.MsgBox;
using Serilog;
using Point = NetTopologySuite.Geometries.Point;

namespace MyExpenses.Wpf.Windows.LocationManagementWindows;

public partial class AddEditLocationWindow
{
    #region Properties

    // ReSharper disable once HeapView.BoxingAllocation
    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty EditPlaceProperty = DependencyProperty.Register(nameof(EditPlace),
        typeof(bool), typeof(AddEditLocationWindow), new PropertyMetadata(false));

    // ReSharper disable once HeapView.BoxingAllocation
    public bool EditPlace
    {
        get => (bool)GetValue(EditPlaceProperty);
        set => SetValue(EditPlaceProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty TextBoxNameHintAssistProperty =
        DependencyProperty.Register(nameof(TextBoxNameHintAssist), typeof(string), typeof(AddEditLocationWindow),
            new PropertyMetadata(default(string)));

    public string TextBoxNameHintAssist
    {
        get => (string)GetValue(TextBoxNameHintAssistProperty);
        set => SetValue(TextBoxNameHintAssistProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty TextBoxNumberHintAssistProperty =
        DependencyProperty.Register(nameof(TextBoxNumberHintAssist), typeof(string), typeof(AddEditLocationWindow),
            new PropertyMetadata(default(string)));

    public string TextBoxNumberHintAssist
    {
        get => (string)GetValue(TextBoxNumberHintAssistProperty);
        set => SetValue(TextBoxNumberHintAssistProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty CheckBoxContentIsOpenProperty =
        DependencyProperty.Register(nameof(CheckBoxContentIsOpen), typeof(string), typeof(AddEditLocationWindow),
            new PropertyMetadata(default(string)));

    public string CheckBoxContentIsOpen
    {
        get => (string)GetValue(CheckBoxContentIsOpenProperty);
        set => SetValue(CheckBoxContentIsOpenProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty TextBoxStreetHintAssistProperty =
        DependencyProperty.Register(nameof(TextBoxStreetHintAssist), typeof(string), typeof(AddEditLocationWindow),
            new PropertyMetadata(default(string)));

    public string TextBoxStreetHintAssist
    {
        get => (string)GetValue(TextBoxStreetHintAssistProperty);
        set => SetValue(TextBoxStreetHintAssistProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty TextBoxPostalCodeHintAssistProperty =
        DependencyProperty.Register(nameof(TextBoxPostalCodeHintAssist), typeof(string), typeof(AddEditLocationWindow),
            new PropertyMetadata(default(string)));

    public string TextBoxPostalCodeHintAssist
    {
        get => (string)GetValue(TextBoxPostalCodeHintAssistProperty);
        set => SetValue(TextBoxPostalCodeHintAssistProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty TextBoxCityHintAssistProperty =
        DependencyProperty.Register(nameof(TextBoxCityHintAssist), typeof(string), typeof(AddEditLocationWindow),
            new PropertyMetadata(default(string)));

    public string TextBoxCityHintAssist
    {
        get => (string)GetValue(TextBoxCityHintAssistProperty);
        set => SetValue(TextBoxCityHintAssistProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty TextBoxCountryHintAssistProperty =
        DependencyProperty.Register(nameof(TextBoxCountryHintAssist), typeof(string), typeof(AddEditLocationWindow),
            new PropertyMetadata(default(string)));

    public string TextBoxCountryHintAssist
    {
        get => (string)GetValue(TextBoxCountryHintAssistProperty);
        set => SetValue(TextBoxCountryHintAssistProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty TextBoxLatitudeHintAssistProperty =
        DependencyProperty.Register(nameof(TextBoxLatitudeHintAssist), typeof(string), typeof(AddEditLocationWindow),
            new PropertyMetadata(default(string)));

    public string TextBoxLatitudeHintAssist
    {
        get => (string)GetValue(TextBoxLatitudeHintAssistProperty);
        set => SetValue(TextBoxLatitudeHintAssistProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty ButtonContentValidNewPointProperty =
        DependencyProperty.Register(nameof(ButtonContentValidNewPoint), typeof(string), typeof(AddEditLocationWindow),
            new PropertyMetadata(default(string)));

    public string ButtonContentValidNewPoint
    {
        get => (string)GetValue(ButtonContentValidNewPointProperty);
        set => SetValue(ButtonContentValidNewPointProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty ButtonContentZoomToPointProperty =
        DependencyProperty.Register(nameof(ButtonContentZoomToPoint), typeof(string), typeof(AddEditLocationWindow),
            new PropertyMetadata(default(string)));

    public string ButtonContentZoomToPoint
    {
        get => (string)GetValue(ButtonContentZoomToPointProperty);
        set => SetValue(ButtonContentZoomToPointProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty TextBoxLongitudeHintAssistProperty =
        DependencyProperty.Register(nameof(TextBoxLongitudeHintAssist), typeof(string), typeof(AddEditLocationWindow),
            new PropertyMetadata(default(string)));

    public string TextBoxLongitudeHintAssist
    {
        get => (string)GetValue(TextBoxLongitudeHintAssistProperty);
        set => SetValue(TextBoxLongitudeHintAssistProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty ComboBoxBackgroundHintAssistProperty =
        DependencyProperty.Register(nameof(ComboBoxBackgroundHintAssist), typeof(string), typeof(AddEditLocationWindow),
            new PropertyMetadata(default(string)));

    public string ComboBoxBackgroundHintAssist
    {
        get => (string)GetValue(ComboBoxBackgroundHintAssistProperty);
        set => SetValue(ComboBoxBackgroundHintAssistProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty ButtonContentSearchByAddressProperty =
        DependencyProperty.Register(nameof(ButtonContentSearchByAddress), typeof(string), typeof(AddEditLocationWindow),
            new PropertyMetadata(default(string)));

    public string ButtonContentSearchByAddress
    {
        get => (string)GetValue(ButtonContentSearchByAddressProperty);
        set => SetValue(ButtonContentSearchByAddressProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty ButtonContentSearchByCoordinateProperty =
        DependencyProperty.Register(nameof(ButtonContentSearchByCoordinate), typeof(string),
            typeof(AddEditLocationWindow), new PropertyMetadata(default(string)));

    public string ButtonContentSearchByCoordinate
    {
        get => (string)GetValue(ButtonContentSearchByCoordinateProperty);
        set => SetValue(ButtonContentSearchByCoordinateProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty ButtonContentValidProperty =
        DependencyProperty.Register(nameof(ButtonContentValid), typeof(string), typeof(AddEditLocationWindow),
            new PropertyMetadata(default(string)));

    public string ButtonContentValid
    {
        get => (string)GetValue(ButtonContentValidProperty);
        set => SetValue(ButtonContentValidProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty ButtonContentDeleteProperty =
        DependencyProperty.Register(nameof(ButtonContentDelete), typeof(string), typeof(AddEditLocationWindow),
            new PropertyMetadata(default(string)));

    public string ButtonContentDelete
    {
        get => (string)GetValue(ButtonContentDeleteProperty);
        set => SetValue(ButtonContentDeleteProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty ButtonContentCancelProperty =
        DependencyProperty.Register(nameof(ButtonContentCancel), typeof(string), typeof(AddEditLocationWindow),
            new PropertyMetadata(default(string)));

    public string ButtonContentCancel
    {
        get => (string)GetValue(ButtonContentCancelProperty);
        set => SetValue(ButtonContentCancelProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty TitleWindowProperty = DependencyProperty.Register(nameof(TitleWindow),
        typeof(string), typeof(AddEditLocationWindow), new PropertyMetadata(default(string)));

    public string TitleWindow
    {
        get => (string)GetValue(TitleWindowProperty);
        set => SetValue(TitleWindowProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public TPlace Place { get; } = new();
    public bool PlaceDeleted { get; private set; }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
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

        UpdateLanguage();
        InitializeComponent();

        MapControl.Map = map;

        // ReSharper disable HeapView.DelegateAllocation
        // map.Tapped += Tapped;
        Interface.ThemeChanged += Interface_OnThemeChanged;
        Interface.LanguageChanged += Interface_OnLanguageChanged;
        // ReSharper restore HeapView.DelegateAllocation

        this.SetWindowCornerPreference();
    }

    private void Interface_OnLanguageChanged()
        => UpdateLanguage();

    private void UpdateLanguage()
    {
        TitleWindow = AddEditLocationResources.TitleWindow;

        TextBoxCityHintAssist = AddEditLocationResources.TextBoxCityHintAssist;
        TextBoxCountryHintAssist = AddEditLocationResources.TextBoxCountryHintAssist;
        TextBoxLatitudeHintAssist = AddEditLocationResources.TextBoxLatitudeHintAssist;
        TextBoxLongitudeHintAssist = AddEditLocationResources.TextBoxLongitudeHintAssist;
        TextBoxNameHintAssist = AddEditLocationResources.TextBoxNameHintAssist;
        TextBoxNumberHintAssist = AddEditLocationResources.TextBoxNumberHintAssist;
        TextBoxPostalCodeHintAssist = AddEditLocationResources.TextBoxPostalCodeHintAssist;
        TextBoxStreetHintAssist = AddEditLocationResources.TextBoxStreetHintAssist;
        ButtonContentValidNewPoint = AddEditLocationResources.ButtonContentValidNewPoint;
        ButtonContentZoomToPoint = AddEditLocationResources.ButtonContentZoomToPoint;
        ButtonContentSearchByAddress = AddEditLocationResources.ButtonContentSearchByAddress;

        ButtonContentSearchByCoordinate = AddEditLocationResources.ButtonContentSearchByCoordinate;

        ButtonContentCancel = AddEditLocationResources.ButtonContentCancel;
        ButtonContentDelete = AddEditLocationResources.ButtonContentDelete;
        ButtonContentValid = AddEditLocationResources.ButtonContentValid;

        CheckBoxContentIsOpen = AddEditLocationResources.CheckBoxContentIsOpen;
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
        var point = Place.Geometry as Point;
        Log.Information("Using the nominatim API to search via a point : {Point}", point);

        var nominatimSearchResult = point?.ToNominatim();

        var mapper = Mapping.Mapper;
        var newPlace = mapper.Map<TPlace>(nominatimSearchResult);
        if (newPlace is null)
        {
            Log.Information("The API returned no result(s)");

            MsgBox.MsgBox.Show(AddEditLocationResources.ButtonSearchByCoordinateMessageBoxErrorTitle,
                AddEditLocationResources.ButtonSearchByCoordinateMessageBoxErrorMessage,
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
        var pointsFeatures = WritableLayer.GetFeatures().Select(s => (TemporaryPointFeature)s).ToList();
        if (pointsFeatures.Count < 2) return;

        var newFeature = pointsFeatures.First(f => f.IsTemp.Equals(true));
        foreach (var pointFeature in pointsFeatures)
        {
            WritableLayer.TryRemove(pointFeature);
        }

        var coordinate = SphericalMercator.ToLonLat(newFeature.Point);
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        Place.Geometry = new Point(coordinate.X, coordinate.Y);

        newFeature.IsTemp = false;
        newFeature.Styles.Clear();
        newFeature.Styles.Add(MapsuiStyleExtensions.RedMarkerStyle);

        WritableLayer.Add(newFeature);

        MapControl.Map.Navigator.CenterOnAndZoomTo(newFeature.Point);
    }

    private void ButtonZoomToPoint_OnClick(object sender, RoutedEventArgs e)
        => MapControl.Map.Navigator.SetZoom(WritableLayer);

    private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void ButtonDelete_OnClick(object sender, RoutedEventArgs e)
    {
        var response = MsgBox.MsgBox.Show(AddEditLocationResources.MessageBoxDeleteQuestion,
            MsgBoxImage.Question, MessageBoxButton.YesNoCancel);
        if (response is not MessageBoxResult.Yes) return;

        Log.Information("Attempting to remove the place \"{PlaceToDeleteName}\"", Place.Name);
        var (success, exception) = Place.Delete();
        if (success)
        {
            Log.Information("Place was successfully removed");
            MsgBox.MsgBox.Show(AddEditLocationResources.MessageBoxDeletePlaceNoUseSuccess, MsgBoxImage.Check);

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

            response = MsgBox.MsgBox.Show(AddEditLocationResources.MessageBoxDeletePlaceUseQuestion,
                MsgBoxImage.Question, MessageBoxButton.YesNoCancel);

            if (response is not MessageBoxResult.Yes) return;

            Log.Information("Attempting to remove the place \"{PlaceToDeleteName}\" with all relative element",
                Place.Name);
            Place.Delete(true);
            Log.Information("Place and all relative element was successfully removed");
            MsgBox.MsgBox.Show(AddEditLocationResources.MessageBoxDeletePlaceUseSuccess, MsgBoxImage.Check);

            PlaceDeleted = true;
            DialogResult = true;
            Close();

            return;
        }

        Log.Error(exception, "An error occurred please retry");
        MsgBox.MsgBox.Show(AddEditLocationResources.MessageBoxDeletePlaceError, MsgBoxImage.Error);
    }

    private void ButtonValid_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }

    #endregion

    private void Interface_OnThemeChanged()
    {
        var backColor = Utils.Resources.GetMaterialDesignPaperMapsUiStylesColor();
        MapControl.Map.BackColor = backColor;
    }

    private void MapControl_OnInfo(object? sender, MapInfoEventArgs e)
    {
        var worldPosition = e.WorldPosition;

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        var feature = new TemporaryPointFeature(worldPosition)
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            Styles = [MapsuiStyleExtensions.GreenMarkerStyle],
            IsTemp = true
        };

        var oldFeature = WritableLayer.GetFeatures().FirstOrDefault(f => (TemporaryPointFeature)f is { IsTemp: true });
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

        switch (nominatimSearchResults.Count)
        {
            case 0:
                MsgBox.MsgBox.Show(AddEditLocationResources.MessageBoxNominatimResultZeroResultTitle,
                    AddEditLocationResources.MessageBoxNominatimResultZeroResultMessage,
                    MessageBoxButton.OK, MsgBoxImage.Exclamation);
                break;
            case 1:
                MsgBox.MsgBox.Show(AddEditLocationResources.MessageBoxNominatimResultOneResultTitle,
                    AddEditLocationResources.MessageBoxNominatimResultOneResultMessage,
                    MessageBoxButton.OK, MsgBoxImage.Check);

                var nominatimSearchResult = nominatimSearchResults.First();
                place = Mapping.Mapper.Map<TPlace>(nominatimSearchResult);
                break;
            case > 1:
                MsgBox.MsgBox.Show(AddEditLocationResources.HandleNominatimResultMultipleResult,
                    MsgBoxImage.Information);

                var places = nominatimSearchResults.Select(s => Mapping.Mapper.Map<TPlace>(s));

                // ReSharper disable once HeapView.ObjectAllocation.Evident
                var nominatimSearchWindows = new NominatimSearchWindow();
                nominatimSearchWindows.AddRange(places);
                nominatimSearchWindows.ShowDialog();

                if (nominatimSearchWindows.DialogResult is not true) return;

                place = Mapping.Mapper.Map<TPlace>(nominatimSearchWindows.CurrentPlace);
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
        var feature = Place.ToTemporaryFeature(MapsuiStyleExtensions.RedMarkerStyle);
        feature.IsTemp = false;

        WritableLayer.Add(feature);

        MapControl.Map.Navigator.CenterOnAndZoomTo(feature.Point, 1);
        MapControl.Refresh();
    }

    private void UpdateTileLayer()
    {
        const string layerName = "Background";

        var httpTileSource = BruTile.Predefined.KnownTileSources.Create(KnownTileSourceSelected);

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        var tileLayer = new TileLayer(httpTileSource);
        tileLayer.Name = layerName;

        var layers = MapControl?.Map.Layers.FindLayer(layerName);
        if (layers is not null) MapControl?.Map.Layers.Remove(layers.ToArray());

        MapControl?.Map.Layers.Insert(0, tileLayer);
    }

    #endregion
}