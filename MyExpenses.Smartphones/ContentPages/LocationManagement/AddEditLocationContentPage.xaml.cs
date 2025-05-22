using System.ComponentModel.DataAnnotations;
using System.Runtime.Versioning;
using BruTile.Predefined;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Manipulations;
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
using MyExpenses.SharedUtils.Resources.Resx.LocationManagement;
using MyExpenses.Sql.Context;
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

    private readonly TaskCompletionSource<bool> _taskCompletionSource = new();

    public Task<bool> ResultDialog
        => _taskCompletionSource.Task;

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
        if (Place.Latitude is null)
        {
            _ = DisplayAlert(AddEditLocationResources.MessageBoxErrorLatitudeIsNullTitle,
                AddEditLocationResources.MessageBoxErrorLatitudeIsNullMessage,
                AddEditLocationResources.MessageBoxErrorLatitudeIsNullOkButton);
            return;
        }

        if (Place.Longitude is null)
        {
            _ = DisplayAlert(AddEditLocationResources.MessageBoxErrorLongitudeIsNullTitle,
                AddEditLocationResources.MessageBoxErrorLongitudeIsNullMessage,
                AddEditLocationResources.MessageBoxErrorLongitudeIsNullOkButton);
            return;
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

    private void ButtonZoomToPoint_OnClicked(object? sender, EventArgs e)
        => MapControl.Map.Navigator.SetZoom(WritableLayer);

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

                var places = nominatimSearchResults.Select(s => Mapping.Mapper.Map<TPlace>(s));
                // ReSharper disable once HeapView.ObjectAllocation.Evident
                var nominatimSearchContentPage = new NominatimSearchContentPage();
                nominatimSearchContentPage.AddRange(places);
                await nominatimSearchContentPage.NavigateToAsync();

                var result = await nominatimSearchContentPage.ResultDialog;
                if (result is not true) return;

                place = nominatimSearchContentPage.CurrentPlace;
                break;
        }

        if (place is null) return;
        SetPlace(place, true);
    }

    private void MapControl_OnInfo(object? sender, MapInfoEventArgs e)
    {
        if (e.GestureType is not GestureType.SingleTap) return;

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

    private void ButtonValid_OnClick(object? sender, EventArgs e)
    {
                // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The serviceProvider and items are set to null because they are not required in this context.
        // The ValidationResults list will store any validation errors detected during the process.
        var validationContext = new ValidationContext(Place, serviceProvider: null, items: null);

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Using 'var' keeps the code concise and readable, as the type (List<ValidationResult>)
        // is evident from the initialization. The result will still be compatible with any method
        // that expects an ICollection<ValidationResult>, as List<T> implements the ICollection interface.
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(Place, validationContext, validationResults, true);

        if (!isValid)
        {
            var propertyError = validationResults.First();
            var propertyMemberName = propertyError.MemberNames.First();

            var messageErrorKey = propertyMemberName switch
            {
                nameof(TPlace.Name) => nameof(AddEditLocationResources.MessageBoxButtonValidationNameError),
                nameof(TPlace.Street) => nameof(AddEditLocationResources.MessageBoxButtonValidationStreetError),
                nameof(TPlace.Postal) => nameof(AddEditLocationResources.MessageBoxButtonValidationPostalError),
                nameof(TPlace.City) => nameof(AddEditLocationResources.MessageBoxButtonValidationCityError),
                nameof(TPlace.Country) => nameof(AddEditLocationResources.MessageBoxButtonValidationCountryError),
                nameof(TPlace.Latitude) => nameof(AddEditLocationResources.MessageBoxButtonValidationLatitudeError),
                nameof(TPlace.Longitude) => nameof(AddEditLocationResources.MessageBoxButtonValidationLongitudeError),
                _ => null
            };

            var localizedErrorMessage = string.IsNullOrEmpty(messageErrorKey)
                ? propertyError.ErrorMessage!
                : AddEditLocationResources.ResourceManager.GetString(messageErrorKey)!;

            _ = DisplayAlert(AddEditLocationResources.MessageBoxButtonValidationTitleError,
                localizedErrorMessage,
                AddEditLocationResources.MessageBoxButtonValidationOkButtonError);
            return;
        }

        _ = HandleButtonResponse(true);
    }

    private void ButtonDelete_OnClick(object? sender, EventArgs e)
        => _ = HandleButtonDelete();

    private async Task HandleButtonDelete()
    {
        var message = string.Format(LocationManagementResources.MessageBoxDeleteQuestionMessage, Place.Name);
        var response = await DisplayAlert(LocationManagementResources.MessageBoxDeleteQuestionTitle,
            message,
            LocationManagementResources.MessageBoxDeleteQuestionYesButton,
            LocationManagementResources.MessageBoxDeleteQuestionCancelButton);

        if (response is not true) return;

        Log.Information("Attempting to remove the place \"{PlaceToDeleteName}\"", Place.Name);
        var (success, exception) = Place.Delete();
        if (success)
        {
            Log.Information("Place was successfully removed");
            await DisplayAlert(
                LocationManagementResources.MessageBoxMenuItemDeleteFeatureNoUseSuccessTitle,
                LocationManagementResources.MessageBoxMenuItemDeleteFeatureNoUseSuccessMessage,
                LocationManagementResources.MessageBoxMenuItemDeleteFeatureNoUseSuccessOkButton);

            PlaceDeleted = true;
            await HandleButtonResponse(true);
            return;
        }

        if (exception!.InnerException is SqliteException
            {
                SqliteExtendedErrorCode: SQLitePCL.raw.SQLITE_CONSTRAINT_FOREIGNKEY
            })
        {
            Log.Error("Foreign key constraint violation");

            response = await DisplayAlert(LocationManagementResources.MessageBoxMenuItemDeleteFeatureUseQuestionTitle,
                LocationManagementResources.MessageBoxMenuItemDeleteFeatureUseQuestionMessage,
                LocationManagementResources.MessageBoxMenuItemDeleteFeatureUseQuestionYesButton,
                LocationManagementResources.MessageBoxMenuItemDeleteFeatureUseQuestionNoButton);

            if (response is not true) return;

            Log.Information("Attempting to remove the place \"{PlaceToDeleteName}\" with all relative element",
                Place.Name);
            Place.Delete(true);
            Log.Information("Place and all relative element was successfully removed");
            await DisplayAlert(LocationManagementResources.MessageBoxMenuItemDeleteFeatureUseSuccessTitle,
                LocationManagementResources.MessageBoxMenuItemDeleteFeatureUseSuccessMessage,
                LocationManagementResources.MessageBoxMenuItemDeleteFeatureUseSuccessOkButton);

            PlaceDeleted = true;
            await HandleButtonResponse(true);

            return;
        }

        Log.Error(exception, "An error occurred please retry");
        await DisplayAlert(LocationManagementResources.MessageBoxMenuItemDeleteFeatureErrorTitle,
            LocationManagementResources.MessageBoxMenuItemDeleteFeatureErrorMessage,
            LocationManagementResources.MessageBoxMenuItemDeleteFeatureErrorOkButton);
    }

    private void ButtonCancel_OnClick(object? sender, EventArgs e)
        => _ = HandleButtonResponse(false);

    private async Task HandleButtonResponse(bool result)
    {
        _taskCompletionSource.SetResult(result);
        await Navigation.PopAsync();
    }
}