using System.Collections.ObjectModel;
using BruTile.Predefined;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Domain.Models.Dependencies;
using Domain.Models.Validation;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Manipulations;
using Mapsui.Projections;
using Mapsui.Tiling.Layers;
using Mapsui.UI;
using Microsoft.Extensions.Logging;
using MyExpenses.Application.Interfaces;
using MyExpenses.Presentation.Converters;
using MyExpenses.Presentation.Enums;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.Messages;
using MyExpenses.Presentation.Resources.Resx.LocationResources;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.Utils;
using MyExpenses.Presentation.ViewModels.Locations;
using MyExpenses.SharedUtils.Collection;

namespace MyExpenses.Presentation.ViewModel;

/// <summary>
/// ViewModel responsible for managing places (locations) displayed on the map.
/// It handles loading, searching, adding, updating and deleting places, interacts with
/// presentation services for data operations and provides commands for the view.
/// </summary>
public partial class LocationManagementViewModel : ViewModelBase
{
    /// <summary>
    /// Layer that contains place markers and features displayed on the map.
    /// </summary>
    internal WritableLayer PlaceLayer { get; } = new() { Style = null, Tag = typeof(PlaceViewModel) };

    /// <summary>
    /// Enumerable wrapper exposing the place layer(s) used for hit testing on the map.
    /// </summary>
    private IEnumerable<ILayer> PlaceLayers => [PlaceLayer];

    /// <summary>
    /// Presentation service used to retrieve and manage place data and map defaults.
    /// </summary>
    private readonly ILocationPresentationService _locationPresentationService;

    /// <summary>
    /// Presentation service used to perform nominatim address/coordinate searches.
    /// </summary>
    private readonly INominatimPresentationService _nominatimPresentationService;

    /// <summary>
    /// Mapper responsible for converting between DTO/domain models and <see cref="PlaceViewModel"/> instances
    /// and for creating map features.
    /// </summary>
    private readonly ILocationDtoViewModelMapper _locationDtoViewModelMapper;

    /// <summary>
    /// Service used to open navigation windows (e.g. location management window).
    /// </summary>
    private readonly INavigationWindowService _navigationWindowService;

    /// <summary>
    /// Dialog service used to display messages and confirmation dialogs to the user.
    /// </summary>
    private readonly IDialogService _dialogService;

    /// <summary>
    /// Logger instance used to record informational and error messages.
    /// </summary>
    private readonly ILogger<LocationManagementViewModel> _logger;

    /// <summary>
    /// Action service that performs create/update/delete operations on places.
    /// </summary>
    private readonly ILocationActionService _locationActionService;

    /// <summary>
    /// Helper utilities for opening external map services (Google Maps, Earth, Street View, ...).
    /// </summary>
    private readonly MapsUtils _mapsUtils;

    /// <summary>
    /// Initializes a new instance of <see cref="LocationManagementViewModel"/>.
    /// </summary>
    /// <param name="locationPresentationService">Service used to retrieve and manage place data and map defaults.</param>
    /// <param name="nominatimPresentationService">Service used to search addresses and coordinates.</param>
    /// <param name="locationDtoViewModelMapper">Mapper between DTOs/Domain models and view models for places.</param>
    /// <param name="navigationWindowService">Service used to open additional windows for location management.</param>
    /// <param name="dialogService">Dialog service used to show messages and confirmations.</param>
    /// <param name="logger">Logger instance for logging events and errors.</param>
    /// <param name="locationActionService">Service that executes create/update/delete actions for places.</param>
    /// <param name="mapsUtils">Helper utilities for opening external map services.</param>
    public LocationManagementViewModel(ILocationPresentationService locationPresentationService,
        INominatimPresentationService nominatimPresentationService,
        ILocationDtoViewModelMapper locationDtoViewModelMapper,
        INavigationWindowService navigationWindowService,
        IDialogService dialogService,
        ILogger<LocationManagementViewModel> logger,
        ILocationActionService locationActionService,
        MapsUtils mapsUtils)
    {
        _locationPresentationService = locationPresentationService;
        _nominatimPresentationService = nominatimPresentationService;
        _locationDtoViewModelMapper = locationDtoViewModelMapper;
        _navigationWindowService = navigationWindowService;
        _dialogService = dialogService;
        _logger = logger;
        _locationActionService = locationActionService;
        _mapsUtils = mapsUtils;

        RegisterMessage();
    }

    /// <summary>
    /// Registers to application-wide messages used to react to place changes and deletions.
    /// Uses WeakReferenceMessenger to avoid strong references from the messenger.
    /// </summary>
    private void RegisterMessage()
    {
        WeakReferenceMessenger.Default.Register<EntityChangedMessage<int[]>>(this, OnPlaceDeleted);
        WeakReferenceMessenger.Default.Register<EntityChangedMessage<PlaceViewModel>>(this, OnPlaceChanged);
    }

    /// <summary>
    /// Handler invoked when a place entity is added or updated elsewhere in the application.
    /// Delegates to <see cref="ApplyAdd(PlaceViewModel)"/> or <see cref="ApplyUpdate(PlaceViewModel)"/> depending on the action.
    /// </summary>
    /// <param name="recipient">Message recipient (this view model).</param>
    /// <param name="message">The entity change message containing the place and action.</param>
    private void OnPlaceChanged(object recipient, EntityChangedMessage<PlaceViewModel> message)
    {
        if (message.Value.EntityType != DependencyType.Place) return;

        switch (message.Value.DataAction)
        {
            case DataAction.Update:
                ApplyUpdate(message.Value.Content);
                break;

            case DataAction.Add:
                ApplyAdd(message.Value.Content);
                break;
        }
    }

    /// <summary>
    /// Applies an addition of a place: creates the corresponding map feature and inserts the
    /// place into the grouped collections (country -> city -> places) used by the tree view.
    /// </summary>
    /// <param name="placeViewModel">The place that was added.</param>
    private void ApplyAdd(PlaceViewModel placeViewModel)
    {
        var pointFeature = _locationDtoViewModelMapper.MapToPointFeature(placeViewModel, placeViewModel.GetMarkerStyle());
        PlaceLayer.Add(pointFeature);
        Map?.Refresh();

        var countryGroup = CountryGroups.FirstOrDefault(g => g.Country == placeViewModel.Country);
        if (countryGroup is null)
        {
            countryGroup = new CountryGroupViewModel { Country = placeViewModel.Country };
            CountryGroups.Add(countryGroup);
        }

        var cityGroup = countryGroup.CityGroups?.FirstOrDefault(g => g.City == placeViewModel.City);
        if (cityGroup is null)
        {
            cityGroup = new CityGroupViewModel { City = placeViewModel.City };
            countryGroup.CityGroups ??= [];
            countryGroup.CityGroups.Add(cityGroup);
        }

        cityGroup.Places ??= [];
        cityGroup.Places.AddAndSort(placeViewModel, s => s.Name!);
    }

    /// <summary>
    /// Applies an update of a place by removing any existing feature and grouping entries and re-adding the updated place.
    /// </summary>
    /// <param name="placeViewModel">The updated place view model.</param>
    private void ApplyUpdate(PlaceViewModel placeViewModel)
    {
        RemovePlaceViewModel(placeViewModel);
        ApplyAdd(placeViewModel);
    }

    /// <summary>
    /// Removes the map feature and grouped collection entries corresponding to the given place.
    /// Ensures empty city or country groups are removed from the collections.
    /// </summary>
    /// <param name="placeViewModel">The place to remove.</param>
    private void RemovePlaceViewModel(PlaceViewModel placeViewModel)
    {
        var pointFeature = PlaceLayer.GetFeatures()
            .FirstOrDefault(s => s[_locationDtoViewModelMapper.PlaceViewModelPointFeatureKey] is PlaceViewModel vm && vm.Id == placeViewModel.Id) as PointFeature;

        if (pointFeature?[_locationDtoViewModelMapper.PlaceViewModelPointFeatureKey] is not PlaceViewModel oldPlaceViewModel) return;
        PlaceLayer.TryRemove(pointFeature);

        var countryGroup = CountryGroups.FirstOrDefault(g => g.Country == oldPlaceViewModel.Country);
        var cityGroup = countryGroup?.CityGroups?.FirstOrDefault(g => g.City == oldPlaceViewModel.City);
        var oldPlace = cityGroup?.Places?.FirstOrDefault(p => p.Id == oldPlaceViewModel.Id);

        if (oldPlace is not null) cityGroup?.Places?.Remove(oldPlace);
        if (cityGroup?.Places?.Count is 0)
        {
            countryGroup?.CityGroups?.Remove(cityGroup);
        }
        if (countryGroup?.CityGroups?.Count is 0)
        {
            CountryGroups.Remove(countryGroup);
        }
    }

    /// <summary>
    /// Handler invoked when place entities have been deleted elsewhere. Removes corresponding features and grouping entries.
    /// </summary>
    /// <param name="recipient">Message recipient (this view model).</param>
    /// <param name="message">The entity change message containing the ids of deleted places.</param>
    private void OnPlaceDeleted(object recipient, EntityChangedMessage<int[]> message)
    {
        if (message.Value.EntityType != DependencyType.Place) return;

        foreach (var id in message.Value.Content)
        {
            if (PlaceLayer.GetFeatures()
                    .FirstOrDefault(s => s[_locationDtoViewModelMapper.PlaceViewModelPointFeatureKey] is PlaceViewModel vm && vm.Id == id) is not PointFeature pointFeature) continue;

            if (pointFeature[_locationDtoViewModelMapper.PlaceViewModelPointFeatureKey] is not PlaceViewModel placeViewModel) continue;

            RemovePlaceViewModel(placeViewModel);
        }
    }

    /// <summary>
    /// Collection of known tile sources available for the map (e.g. OpenStreetMap, Bing, etc.).
    /// </summary>
    public ObservableCollection<KnownTileSource> KnownTileSources { get; } = [];

    /// <summary>
    /// Collection grouping places by country and city for display in a tree view.
    /// </summary>
    public ObservableCollection<CountryGroupViewModel> CountryGroups { get; } = [];

    /// <summary>
    /// The active Mapsui <see cref="Map"/> instance displayed in the view.
    /// </summary>
    [ObservableProperty]
    public partial Map? Map { get; set; }

    /// <summary>
    /// The currently selected <see cref="PlaceViewModel"/>, or null if none is selected.
    /// </summary>
    [ObservableProperty]
    public partial PlaceViewModel? SelectedPlaceViewModel { get; set; }

    /// <summary>
    /// Current map point (in MPoint) corresponding to the selected or newly created place.
    /// Used when opening external map services or creating new places at the current cursor.
    /// </summary>
    internal MPoint SelectedPlacePoint { get; set; } = new(0, 0);

    /// <summary>
    /// The currently selected tile source for the map.
    /// </summary>
    [ObservableProperty]
    public partial KnownTileSource? KnownTileSourceSelected { get; set; }

    /// <summary>
    /// Indicates whether the view is currently in edit mode for a location.
    /// </summary>
    [ObservableProperty]
    public partial bool IsEditLocation { get; set; }

    /// <summary>
    /// Opens the location management window. If <paramref name="placeViewModel"/> is null the
    /// window is opened to create a new place at <see cref="SelectedPlacePoint"/>, otherwise
    /// the window is opened in edit mode for the provided place.
    /// </summary>
    /// <param name="placeViewModel">The place to edit, or null to create a new one.</param>
    [RelayCommand]
    private void OnManagePlaceViewModel(PlaceViewModel? placeViewModel)
    {
        if (placeViewModel is null)
        {
            _navigationWindowService.ShowLocationManagementWindow(SelectedPlacePoint);
        }
        else
        {
            _navigationWindowService.ShowLocationManagementWindow(placeViewModel, true);
        }
    }

    /// <summary>
    /// Validates the temporary feature placed on the map, converts it to a permanent marker
    /// and updates the <see cref="SelectedPlaceViewModel"/>'s coordinates accordingly.
    /// </summary>
    [RelayCommand]
    private void OnValidTemporaryFeature()
    {
        var features = PlaceLayer.GetFeatures().ToArray();
        if (features.Length is not 2) return;

        var temporaryFeature = features.First(f => f is TemporaryPointFeature { IsTemp: true }) as TemporaryPointFeature;
        PlaceLayer.Clear();

        var point = SphericalMercator.ToLonLat(temporaryFeature!.Point);
        SelectedPlaceViewModel?.Latitude = point.Y;
        SelectedPlaceViewModel?.Longitude = point.X;

        point = SphericalMercator.FromLonLat(point);
        var pointFeature = _locationDtoViewModelMapper.MapToPointFeature(point, MapsuiStyleExtensions.RedMarkerStyle);

        PlaceLayer.Add(pointFeature);

        OnZoomToPoints();
    }

    /// <summary>
    /// Loads the specified place into the view model for editing or viewing.
    /// This will copy values into <see cref="SelectedPlaceViewModel"/>, set the edit mode,
    /// and display the corresponding map marker.
    /// </summary>
    /// <param name="placeViewModel">The place to load.</param>
    /// <param name="isEdit">True to open in edit mode; false for creation/view mode.</param>
    public void LoadPlaceViewModel(PlaceViewModel placeViewModel, bool isEdit)
    {
        SelectedPlaceViewModel ??= new PlaceViewModel();
        _locationDtoViewModelMapper.Merge(placeViewModel, SelectedPlaceViewModel);

        IsEditLocation = isEdit;
        if (isEdit) SelectedPlaceViewModel.AcceptChanges();

        var pointFeature = _locationDtoViewModelMapper.MapToPointFeature(placeViewModel, placeViewModel.GetMarkerStyle(), false);
        PlaceLayer.Clear();
        PlaceLayer.Add(pointFeature);
    }

    /// <summary>
    /// Called when the mouse/cursor position on the map changes. Converts screen coordinates to
    /// geographic coordinates and updates the selected place point. Optionally updates the
    /// selected place view model with feature information from the map.
    /// </summary>
    /// <param name="screenX">X coordinate on the control (pixels).</param>
    /// <param name="screenY">Y coordinate on the control (pixels).</param>
    /// <param name="mapControl">The map control used to retrieve map info.</param>
    /// <param name="updateInfo">If true, update <see cref="SelectedPlaceViewModel"/> with information from map features.</param>
    public void OnPositionChanged(double screenX, double screenY, IMapControl mapControl, bool updateInfo)
    {
        if (Map is null) return;

        var worldPosition = Map.Navigator.Viewport.ScreenToWorld(screenX, screenY);
        var lonLat = SphericalMercator.ToLonLat(worldPosition.X, worldPosition.Y);
        SelectedPlacePoint = _locationDtoViewModelMapper.MapToMPoint(lonLat);

        if (!updateInfo) return;

        var mapInfo = mapControl.GetMapInfo(new ScreenPosition(screenX, screenY), PlaceLayers);
        SetSelectedPlaceViewModel(mapInfo);
    }

    /// <summary>
    /// Opens the current selected place in Google Earth Web in the default browser.
    /// </summary>
    [RelayCommand]
    private void OnGoToGoogleEarthWeb()
        => _mapsUtils.GoToGoogleEarthWeb(SelectedPlacePoint);

    /// <summary>
    /// Opens the current selected place in Google Maps in the default browser.
    /// </summary>
    [RelayCommand]
    private void OnGoToGoogleMaps()
        => _mapsUtils.GoToGoogleMaps(SelectedPlacePoint);

    /// <summary>
    /// Opens Google Street View centered on the current selected place.
    /// </summary>
    [RelayCommand]
    private void OnGoToGoogleStreetView()
        => _mapsUtils.GoToGoogleStreetView(SelectedPlacePoint);

    /// <summary>
    /// Retrieves map feature information at the clicked/tapped position and updates the selected place view model.
    /// </summary>
    /// <param name="mapInfoEventArgs">Event args containing the screen/world position and hit-test info.</param>
    [RelayCommand]
    private void OnMapInfo(MapInfoEventArgs? mapInfoEventArgs)
    {
        var mapInfo = mapInfoEventArgs?.GetMapInfo(PlaceLayers);

        SetSelectedPlaceViewModel(mapInfo);
    }

    /// <summary>
    /// Searches for an address/place using the latitude/longitude of the currently selected place.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    [RelayCommand]
    private async Task SearchByCoordinate(CancellationToken cancellationToken = default)
    {
        if (SelectedPlaceViewModel is null) return;

        var results = await _nominatimPresentationService.SearchAsync(SelectedPlaceViewModel.Latitude ?? 0, SelectedPlaceViewModel.Longitude ?? 0, cancellationToken);
        HandleNominatimResult(results);
    }

    /// <summary>
    /// Searches for places using the address composed from the selected place view model.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    [RelayCommand]
    private async Task OnSearchByAddress(CancellationToken cancellationToken = default)
    {
        if (SelectedPlaceViewModel is null) return;

        var address = SelectedPlaceViewModel.GetAddress();
        var results = await _nominatimPresentationService.SearchAsync(address, cancellationToken);
        HandleNominatimResult(results);
    }

    /// <summary>
    /// Handles the results returned by the nominatim search service by asking the navigation
    /// window service to let the user choose a place and then loading the chosen place.
    /// </summary>
    /// <param name="results">The results returned by the nominatim search.</param>
    private void HandleNominatimResult(Result<IEnumerable<NominatimSearchResultViewModel>> results)
    {
        var placeViewModel = _navigationWindowService.ManageLocationWindowAction(results);
        if (placeViewModel is null) return;

        LoadPlaceViewModel(placeViewModel, false);
        OnZoomToPoints();
    }

    /// <summary>
    /// Creates or replaces a temporary feature on the map at the provided world position.
    /// Temporary features are used when the user selects a location interactively.
    /// </summary>
    /// <param name="mapInfoEventArgs">Map info event arguments containing the world position.</param>
    [RelayCommand]
    private void OnMapInfoTemporaryFeature(MapInfoEventArgs? mapInfoEventArgs)
    {
        if (mapInfoEventArgs is null) return;

        var worldPositionPoint = mapInfoEventArgs.WorldPosition;
        var pointFeature = _locationDtoViewModelMapper.MapToTemporaryFeature(worldPositionPoint, MapsuiStyleExtensions.GreenMarkerStyle);

        var oldFeature = PlaceLayer.GetFeatures().FirstOrDefault(f => f is TemporaryPointFeature { IsTemp: true });
        if (oldFeature is not null) PlaceLayer.TryRemove(oldFeature);

        PlaceLayer.Add(pointFeature);
    }

    /// <summary>
    /// Sets <see cref="SelectedPlaceViewModel"/> based on the provided map feature info.
    /// </summary>
    /// <param name="mapInfo">Map info containing the selected feature.</param>
    private void SetSelectedPlaceViewModel(MapInfo? mapInfo)
    {
        SelectedPlaceViewModel = mapInfo?.Feature is not PointFeature pointFeature
            ? null
            : _locationDtoViewModelMapper.MapToPlaceViewModel(pointFeature);
    }

    /// <summary>
    /// Handles selection of items in the tree view (country, city or place) and updates the map
    /// viewport to show the corresponding points.
    /// </summary>
    /// <param name="item">The selected tree view item (country group, city group or place).</param>
    [RelayCommand]
    private void OnSelectedTreeViewItem(object? item)
    {
        var points = item switch
        {
            CountryGroupViewModel countryGroup => countryGroup.CityGroups?
                .SelectMany(cityGroup => GetPoints(cityGroup.Places)) ?? [],
            CityGroupViewModel cityGroup => GetPoints(cityGroup.Places),
            PlaceViewModel placeViewModel => [_locationDtoViewModelMapper.MapToMPoint(placeViewModel)],
            _ => []
        };

        Map?.Navigator.SetZoom(points.ToArray());

        return;

        IEnumerable<MPoint> GetPoints(IEnumerable<PlaceViewModel>? places)
        {
            return places?
                .Where(s =>s.Latitude is not 0 && s.Longitude is not 0)
                .Select(_locationDtoViewModelMapper.MapToMPoint) ?? [];
        }
    }

    /// <summary>
    /// Called when the map control is loaded. Initializes the map from the presentation service
    /// and attaches the place layer.
    /// </summary>
    /// <param name="mapControl">The loaded map control.</param>
    [RelayCommand]
    private void OnMapControlLoaded(IMapControl mapControl)
    {
        var mapResult = _locationPresentationService.GetDefaultMap(true, Mapsui.Styles.Color.Black);
        if (!mapResult.IsSuccess) return;

        Map = mapResult.Value!;
        Map.Layers.AddOnTop(PlaceLayer);
        mapControl.Map = Map;

        OnUpdateTileSource();
        OnZoomToPoints();
    }

    /// <summary>
    /// Adjusts the map viewport to include all points in the place layer, or sets a default zoom
    /// if there are no points.
    /// </summary>
    [RelayCommand]
    private void OnZoomToPoints()
    {
        if (Map is null) return;

        if (PlaceLayer.GetFeatures().Any()) Map.Navigator.SetZoom(PlaceLayer);
        else
        {
            var resolution = Map.Navigator.Resolutions[2];
            Map.Navigator.ZoomTo(resolution);
        }
    }

    /// <summary>
    /// Centers the map on the provided place view model and zooms to it.
    /// </summary>
    /// <param name="placeViewModel">The place to center on.</param>
    [RelayCommand]
    private void OnPlaceViewModelSelected(PlaceViewModel? placeViewModel)
    {
        if (placeViewModel is null) return;
        var feature = _locationDtoViewModelMapper.MapToPointFeature(placeViewModel);
        var point = feature.Point;

        Map?.Navigator.CenterOnAndZoomTo(point);
    }

    /// <summary>
    /// Updates the map's background tile source (tile provider) and refreshes the map layers.
    /// </summary>
    /// <param name="knownTileSource">Optional known tile source to select; if null the selected source is used or a default.</param>
    [RelayCommand]
    private void OnUpdateTileSource(KnownTileSource? knownTileSource = null)
    {
        const string backgroundLayer = "Background";

        if (knownTileSource is null)
        {
            knownTileSource ??= KnownTileSource.OpenStreetMap;
            KnownTileSourceSelected = knownTileSource;
        }

        var httpTileSource = BruTile.Predefined.KnownTileSources.Create((KnownTileSource)knownTileSource);
        var tileLayer = new TileLayer(httpTileSource);
        tileLayer.Name = backgroundLayer;

        var layers = Map?.Layers.FindLayer(backgroundLayer).ToArray();
        if (layers?.Length > 0) Map?.Layers.Remove(layers);

        Map?.Layers.AddOnBottom(tileLayer);
        Map?.RefreshGraphics();
    }

    /// <summary>
    /// Loads static data required by the view model such as the available tile sources.
    /// </summary>
    [RelayCommand]
    private void OnLoad()
    {
        var titleSource = _locationPresentationService.GetAllKnowTitleSource();
        if (titleSource.IsSuccess) KnownTileSources.AddRangeAndSort(titleSource.Value!, s => s.ToString());
    }

    /// <summary>
    /// Loads places and their features asynchronously and populates the map and grouping collections.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    [RelayCommand]
    private async Task OnLoadWithFeature(CancellationToken cancellationToken = default)
    {
        OnLoad();

        var resultPlaces = await _locationPresentationService.GetAllPlaces(cancellationToken);
        if (!resultPlaces.IsSuccess)
        {
            _logger.LogWarning("Failed to load place group: {Error}", resultPlaces.InternalMessage!);
            return;
        }

        var pointFeatures = resultPlaces.Value!
            .Where(s => s.Latitude is not null && s.Latitude is not 0 && s.Longitude is not null &&
                        s.Longitude is not 0)
            .Select(s => _locationDtoViewModelMapper.MapToPointFeature(s, s.GetMarkerStyle()));

        PlaceLayer.AddRange(pointFeatures);

        var group = _locationDtoViewModelMapper.MapToGroup(resultPlaces.Value!);
        CountryGroups.AddRangeAndSort(group, s => s.Country!);
    }

    /// <summary>
    /// Validates and either creates or updates the selected place depending on the current edit mode.
    /// On success closes the provided dialog if present. Errors are logged and shown via the dialog service.
    /// </summary>
    /// <param name="dialog">Optional dialog that will be closed on success.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    [RelayCommand]
    private async Task OnValid(IClosable? dialog, CancellationToken cancellationToken = default)
    {
        if (SelectedPlaceViewModel is null) return;

        try
        {
            var result = IsEditLocation
                ? await _locationActionService.UpdatePlaceAsync(SelectedPlaceViewModel, cancellationToken)
                : await _locationActionService.CreatePlaceAsync(SelectedPlaceViewModel, cancellationToken);

            if (result.IsSuccess) dialog?.Close();
        }
        catch (Exception e)
        {
            if (IsEditLocation)
            {
                _logger.LogError(e, "Error updating place");

                _dialogService.ShowMessageBox(LocationResources.MessageboxPlaceUpdateErrorCaption,
                    LocationResources.MessageboxPlaceUpdateErrorContent,
                    MessageBoxButton.Ok, MsgBoxImage.Error);
            }
            else
            {
                _logger.LogError(e, "Error creating place");

                _dialogService.ShowMessageBox(LocationResources.MessageboxPlaceCreateErrorCaption,
                    LocationResources.MessageboxPlaceCreateErrorContent,
                    MessageBoxButton.Ok, MsgBoxImage.Error);
            }
        }
    }

    /// <summary>
    /// Cancels the current dialog and closes it without saving changes.
    /// </summary>
    /// <param name="closable">The dialog to close.</param>
    [RelayCommand]
    private void OnCancel(IClosable? closable)
    {
        closable?.DialogResult = false;
        closable?.Close();
    }

    /// <summary>
    /// Deletes the currently selected place via the action service and closes the dialog on success.
    /// Errors are logged and displayed via the dialog service.
    /// </summary>
    /// <param name="closable">The dialog to close on successful deletion.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    [RelayCommand]
    private async Task OnDelete(IClosable? closable, CancellationToken cancellationToken = default)
    {
        if (SelectedPlaceViewModel is null) return;

        try
        {
            var result = await _locationActionService.DeletePlaceAsync(SelectedPlaceViewModel, cancellationToken);
            if (result.IsSuccess) closable?.Close();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error deleting place");

            _dialogService.ShowMessageBox(LocationResources.MessageboxPlaceDeleteErrorCaption,
                LocationResources.MessageboxPlaceDeleteErrorContent,
                MessageBoxButton.Ok, MsgBoxImage.Error);
        }
    }
}