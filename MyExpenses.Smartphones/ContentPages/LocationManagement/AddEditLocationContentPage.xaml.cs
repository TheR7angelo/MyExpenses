using System.Runtime.Versioning;
using BruTile.Predefined;
using Mapsui.Layers;
using Mapsui.Tiling.Layers;
using MyExpenses.Models.AutoMapper;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Models.WebApi.Nominatim;
using MyExpenses.SharedUtils.Properties;
using MyExpenses.SharedUtils.Resources.Resx.AddEditLocation;
using MyExpenses.Utils.Maps;
using MyExpenses.WebApi.Nominatim;
using Serilog;
using Point = NetTopologySuite.Geometries.Point;

namespace MyExpenses.Smartphones.ContentPages.LocationManagement;

public partial class AddEditLocationContentPage
{
    #region Properties

    // ReSharper disable once HeapView.BoxingAllocation
    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly BindableProperty EditPlaceProperty = BindableProperty.Create(nameof(EditPlace),
        typeof(bool), typeof(AddEditLocationContentPage), false);

    // ReSharper disable once HeapView.BoxingAllocation
    public bool EditPlace
    {
        get => (bool)GetValue(EditPlaceProperty);
        set => SetValue(EditPlaceProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly BindableProperty ComboBoxBasemapHintAssistProperty =
        BindableProperty.Create(nameof(ComboBoxBasemapHintAssist), typeof(string), typeof(AddEditLocationContentPage));

    public string ComboBoxBasemapHintAssist
    {
        get => (string)GetValue(ComboBoxBasemapHintAssistProperty);
        set => SetValue(ComboBoxBasemapHintAssistProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly BindableProperty TextBoxNameHintAssistProperty =
        BindableProperty.Create(nameof(TextBoxNameHintAssist), typeof(string), typeof(AddEditLocationContentPage));

    public string TextBoxNameHintAssist
    {
        get => (string)GetValue(TextBoxNameHintAssistProperty);
        set => SetValue(TextBoxNameHintAssistProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly BindableProperty TextBoxNumberHintAssistProperty =
        BindableProperty.Create(nameof(TextBoxNumberHintAssist), typeof(string), typeof(AddEditLocationContentPage));

    public string TextBoxNumberHintAssist
    {
        get => (string)GetValue(TextBoxNumberHintAssistProperty);
        set => SetValue(TextBoxNumberHintAssistProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly BindableProperty CheckBoxContentIsOpenProperty =
        BindableProperty.Create(nameof(CheckBoxContentIsOpen), typeof(string), typeof(AddEditLocationContentPage));

    public string CheckBoxContentIsOpen
    {
        get => (string)GetValue(CheckBoxContentIsOpenProperty);
        set => SetValue(CheckBoxContentIsOpenProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly BindableProperty TextBoxStreetHintAssistProperty =
        BindableProperty.Create(nameof(TextBoxStreetHintAssist), typeof(string), typeof(AddEditLocationContentPage));

    public string TextBoxStreetHintAssist
    {
        get => (string)GetValue(TextBoxStreetHintAssistProperty);
        set => SetValue(TextBoxStreetHintAssistProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly BindableProperty TextBoxPostalCodeHintAssistProperty =
        BindableProperty.Create(nameof(TextBoxPostalCodeHintAssist), typeof(string), typeof(AddEditLocationContentPage));

    public string TextBoxPostalCodeHintAssist
    {
        get => (string)GetValue(TextBoxPostalCodeHintAssistProperty);
        set => SetValue(TextBoxPostalCodeHintAssistProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly BindableProperty TextBoxCityHintAssistProperty =
        BindableProperty.Create(nameof(TextBoxCityHintAssist), typeof(string), typeof(AddEditLocationContentPage));

    public string TextBoxCityHintAssist
    {
        get => (string)GetValue(TextBoxCityHintAssistProperty);
        set => SetValue(TextBoxCityHintAssistProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly BindableProperty TextBoxCountryHintAssistProperty =
        BindableProperty.Create(nameof(TextBoxCountryHintAssist), typeof(string), typeof(AddEditLocationContentPage));

    public string TextBoxCountryHintAssist
    {
        get => (string)GetValue(TextBoxCountryHintAssistProperty);
        set => SetValue(TextBoxCountryHintAssistProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly BindableProperty TextBoxLatitudeHintAssistProperty =
        BindableProperty.Create(nameof(TextBoxLatitudeHintAssist), typeof(string), typeof(AddEditLocationContentPage));

    public string TextBoxLatitudeHintAssist
    {
        get => (string)GetValue(TextBoxLatitudeHintAssistProperty);
        set => SetValue(TextBoxLatitudeHintAssistProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly BindableProperty ButtonContentValidNewPointProperty =
        BindableProperty.Create(nameof(ButtonContentValidNewPoint), typeof(string), typeof(AddEditLocationContentPage));

    public string ButtonContentValidNewPoint
    {
        get => (string)GetValue(ButtonContentValidNewPointProperty);
        set => SetValue(ButtonContentValidNewPointProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly BindableProperty ButtonContentZoomToPointProperty =
        BindableProperty.Create(nameof(ButtonContentZoomToPoint), typeof(string), typeof(AddEditLocationContentPage));

    public string ButtonContentZoomToPoint
    {
        get => (string)GetValue(ButtonContentZoomToPointProperty);
        set => SetValue(ButtonContentZoomToPointProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly BindableProperty TextBoxLongitudeHintAssistProperty =
        BindableProperty.Create(nameof(TextBoxLongitudeHintAssist), typeof(string), typeof(AddEditLocationContentPage));

    public string TextBoxLongitudeHintAssist
    {
        get => (string)GetValue(TextBoxLongitudeHintAssistProperty);
        set => SetValue(TextBoxLongitudeHintAssistProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly BindableProperty ComboBoxBackgroundHintAssistProperty =
        BindableProperty.Create(nameof(ComboBoxBackgroundHintAssist), typeof(string), typeof(AddEditLocationContentPage));

    public string ComboBoxBackgroundHintAssist
    {
        get => (string)GetValue(ComboBoxBackgroundHintAssistProperty);
        set => SetValue(ComboBoxBackgroundHintAssistProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly BindableProperty ButtonContentSearchByAddressProperty =
        BindableProperty.Create(nameof(ButtonContentSearchByAddress), typeof(string), typeof(AddEditLocationContentPage));

    public string ButtonContentSearchByAddress
    {
        get => (string)GetValue(ButtonContentSearchByAddressProperty);
        set => SetValue(ButtonContentSearchByAddressProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly BindableProperty ButtonContentSearchByCoordinateProperty =
        BindableProperty.Create(nameof(ButtonContentSearchByCoordinate), typeof(string),
            typeof(AddEditLocationContentPage));

    public string ButtonContentSearchByCoordinate
    {
        get => (string)GetValue(ButtonContentSearchByCoordinateProperty);
        set => SetValue(ButtonContentSearchByCoordinateProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly BindableProperty ButtonContentSearchByCurrentCoordinateProperty =
        BindableProperty.Create(nameof(ButtonContentSearchByCurrentCoordinate), typeof(string),
            typeof(AddEditLocationContentPage));

    public string ButtonContentSearchByCurrentCoordinate
    {
        get => (string)GetValue(ButtonContentSearchByCurrentCoordinateProperty);
        set => SetValue(ButtonContentSearchByCurrentCoordinateProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly BindableProperty ButtonContentValidProperty =
        BindableProperty.Create(nameof(ButtonContentValid), typeof(string), typeof(AddEditLocationContentPage));

    public string ButtonContentValid
    {
        get => (string)GetValue(ButtonContentValidProperty);
        set => SetValue(ButtonContentValidProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly BindableProperty ButtonContentDeleteProperty =
        BindableProperty.Create(nameof(ButtonContentDelete), typeof(string), typeof(AddEditLocationContentPage));

    public string ButtonContentDelete
    {
        get => (string)GetValue(ButtonContentDeleteProperty);
        set => SetValue(ButtonContentDeleteProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly BindableProperty ButtonContentCancelProperty =
        BindableProperty.Create(nameof(ButtonContentCancel), typeof(string), typeof(AddEditLocationContentPage));

    public string ButtonContentCancel
    {
        get => (string)GetValue(ButtonContentCancelProperty);
        set => SetValue(ButtonContentCancelProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly BindableProperty TitleWindowProperty = BindableProperty.Create(nameof(TitleWindow),
        typeof(string), typeof(AddEditLocationContentPage));

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

    public AddEditLocationContentPage()
    {
        KnownTileSources = [..MapsuiMapExtensions.GetAllKnowTileSource()];

        var map = MapsuiMapExtensions.GetMap(true);
        map.Layers.Add(WritableLayer);

        UpdateLanguage();
        InitializeComponent();

        MapControl.Map = map;
        // MapControl.Map.Navigator.SetZoom(WritableLayer);

        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    private void Interface_OnLanguageChanged()
        => UpdateLanguage();

    private void UpdateLanguage()
    {
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
        ComboBoxBasemapHintAssist = AddEditLocationResources.ComboBoxBasemapHintAssist;

        ButtonContentSearchByAddress = AddEditLocationResources.ButtonContentSearchByAddress;
        ButtonContentSearchByCoordinate = AddEditLocationResources.ButtonContentSearchByCoordinate;
        ButtonContentSearchByCurrentCoordinate = AddEditLocationResources.ButtonContentSearchByCurrentCoordinate;

        ButtonContentCancel = AddEditLocationResources.ButtonContentCancel;
        ButtonContentDelete = AddEditLocationResources.ButtonContentDelete;
        ButtonContentValid = AddEditLocationResources.ButtonContentValid;

        CheckBoxContentIsOpen = AddEditLocationResources.CheckBoxContentIsOpen;
    }

    private void ButtonSearchByAddress_OnClicked(object? sender, EventArgs e)
    {
        var address = Place.ToString();
        Log.Information("Using the nominatim API to search via an address : \"{Address}\"", address);

        var nominatimSearchResults = address.ToNominatim()?.ToList() ?? [];

        Log.Information("The API returned \"{Count}\" result(s)", nominatimSearchResults.Count);
        _ = HandleNominatimResult(nominatimSearchResults);
    }

    private void ButtonSearchByCoordinate_OnClicked(object? sender, EventArgs e)
    {
        if (Place.Latitude is null || Place.Longitude is null)
        {
            // TODO work
            _ = DisplayAlert("Error", "The latitude or longitude are not set", "Ok");
        }

        var point = Place.Geometry as Point;
        _ = SearchByCoordinate(point!);
    }

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst15.0")]
    [SupportedOSPlatform("Windows")]
    private void ButtonSearchByCurrentCoordinate_OnClicked(object? sender, EventArgs e)
        => _ = HandleSearchWithCurrentCoordinate();

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst15.0")]
    [SupportedOSPlatform("Windows")]
    private async Task HandleSearchWithCurrentCoordinate()
    {
        var location = await Maui.Utils.SensorRequestUtils.GetLocation();
        if (location is null) return;

        var point = new Point(location.Longitude, location.Latitude);
        _ = SearchByCoordinate(point);
    }

    private async Task SearchByCoordinate(Point point)
    {
        Log.Information("Using the nominatim API to search via a point : {Point}", point);
        var nominatimSearchResult = point.ToNominatim();

        var results = new List<NominatimSearchResult>();
        if (nominatimSearchResult is not null) results.Add(nominatimSearchResult);
        await HandleNominatimResult(results);
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

    private void PickerFieldKnownTileSource_OnSelectedItemChanged(object? sender, object o)
        => UpdateTileLayer();

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

    private void MapControl_OnLoaded(object? sender, EventArgs e)
        => UpdateTileLayer();

    private void ButtonValidNewPoint_OnClicked(object? sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    private void ButtonZoomToPoint_OnClicked(object? sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    private async Task HandleNominatimResult(IReadOnlyCollection<NominatimSearchResult> nominatimSearchResults)
    {
        TPlace? place = null;

        switch (nominatimSearchResults.Count)
        {
            case 0:
                Log.Information("The API returned no result(s)");
                await DisplayAlert(AddEditLocationResources.MessageBoxNominatimResultZeroResultTitle,
                    AddEditLocationResources.MessageBoxNominatimResultZeroResultMessage,
                    AddEditLocationResources.MessageBoxNominatimResultZeroResultOkButton);
                break;
            case 1:
                Log.Information("The API returned one result");
                await DisplayAlert(AddEditLocationResources.MessageBoxNominatimResultOneResultTitle,
                    AddEditLocationResources.MessageBoxNominatimResultOneResultMessage,
                    AddEditLocationResources.MessageBoxNominatimResultOneResultOkButton);

                var nominatimSearchResult = nominatimSearchResults.First();
                place = Mapping.Mapper.Map<TPlace>(nominatimSearchResult);
                break;
            case > 1:
                Log.Information("The API returned multiple results ({Count}) :", nominatimSearchResults.Count);
                Log.Information("Detailed results: {NominatimSearchResults}", nominatimSearchResults);

                await DisplayAlert(AddEditLocationResources.MessageBoxNominatimResultMultipleResultTitle,
                    AddEditLocationResources.MessageBoxNominatimResultMultipleResultMessage,
                    AddEditLocationResources.MessageBoxNominatimResultMultipleResultOkButton);

                // TODO work

                // var places = nominatimSearchResults
                //     .Select(s => Mapping.Mapper.Map<TPlace>(s));
                // ReSharper disable once HeapView.ObjectAllocation.Evident
                // var nominatimSearchWindows = new NominatimSearchWindow();
                // nominatimSearchWindows.AddRange(places);
                // nominatimSearchWindows.ShowDialog();

                // if (nominatimSearchWindows.DialogResult is not true) return;

                // place = Mapping.Mapper.Map<TPlace>(nominatimSearchWindows.CurrentPlace);
                break;
        }

        if (place is null) return;
        SetPlace(place, true);
    }
}