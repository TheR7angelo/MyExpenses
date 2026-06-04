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

public partial class LocationManagementViewModel : ViewModelBase
{
    private WritableLayer PlaceLayer { get; } = new() { Style = null, Tag = typeof(PlaceViewModel) };
    private IEnumerable<ILayer> PlaceLayers => [PlaceLayer];

    private readonly ILocationPresentationService _locationPresentationService;
    private readonly INominatimPresentationService _nominatimPresentationService;
    private readonly ILocationDtoViewModelMapper _locationDtoViewModelMapper;
    private readonly INavigationWindowService _navigationWindowService;
    private readonly IDialogService _dialogService;
    private readonly ILogger<LocationManagementViewModel> _logger;
    private readonly ILocationActionService _locationActionService;
    private readonly MapsUtils _mapsUtils;

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

    private void RegisterMessage()
    {
        WeakReferenceMessenger.Default.Register<EntityChangedMessage<int[]>>(this, OnPlaceDeleted);
        WeakReferenceMessenger.Default.Register<EntityChangedMessage<PlaceViewModel>>(this, OnPlaceChanged);
    }

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

    private void ApplyUpdate(PlaceViewModel placeViewModel)
    {
        RemovePlaceViewModel(placeViewModel);
        ApplyAdd(placeViewModel);
    }

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

    public ObservableCollection<KnownTileSource> KnownTileSources { get; } = [];
    public ObservableCollection<CountryGroupViewModel> CountryGroups { get; } = [];

    [ObservableProperty]
    public partial Map? Map { get; set; }

    [ObservableProperty]
    public partial PlaceViewModel? SelectedPlaceViewModel { get; set; }

    private MPoint SelectedPlacePoint { get; set; } = new(0, 0);

    [ObservableProperty]
    public partial KnownTileSource? KnownTileSourceSelected { get; set; }

    [ObservableProperty]
    public partial bool IsEditLocation { get; set; }

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

    [RelayCommand]
    private void OnGoToGoogleEarthWeb()
        => _mapsUtils.GoToGoogleEarthWeb(SelectedPlacePoint);

    [RelayCommand]
    private void OnGoToGoogleMaps()
        => _mapsUtils.GoToGoogleMaps(SelectedPlacePoint);

    [RelayCommand]
    private void OnGoToGoogleStreetView()
        => _mapsUtils.GoToGoogleStreetView(SelectedPlacePoint);

    [RelayCommand]
    private void OnMapInfo(MapInfoEventArgs? mapInfoEventArgs)
    {
        var mapInfo = mapInfoEventArgs?.GetMapInfo(PlaceLayers);

        SetSelectedPlaceViewModel(mapInfo);
    }

    [RelayCommand]
    private async Task SearchByCoordinate(CancellationToken cancellationToken = default)
    {
        if (SelectedPlaceViewModel is null) return;

        var results = await _nominatimPresentationService.SearchAsync(SelectedPlaceViewModel.Latitude ?? 0, SelectedPlaceViewModel.Longitude ?? 0, cancellationToken);
        HandleNominatimResult(results);
    }

    [RelayCommand]
    private async Task OnSearchByAddress(CancellationToken cancellationToken = default)
    {
        if (SelectedPlaceViewModel is null) return;

        var address = SelectedPlaceViewModel.GetAddress();
        var results = await _nominatimPresentationService.SearchAsync(address, cancellationToken);
        HandleNominatimResult(results);
    }

    private void HandleNominatimResult(Result<IEnumerable<NominatimSearchResultViewModel>> results)
    {
        var placeViewModel = _navigationWindowService.ManageLocationWindowAction(results);
        if (placeViewModel is null) return;

        LoadPlaceViewModel(placeViewModel, false);
        OnZoomToPoints();
    }

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

    private void SetSelectedPlaceViewModel(MapInfo? mapInfo)
    {
        SelectedPlaceViewModel = mapInfo?.Feature is not PointFeature pointFeature
            ? null
            : _locationDtoViewModelMapper.MapToPlaceViewModel(pointFeature);
    }

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

    [RelayCommand]
    private void OnPlaceViewModelSelected(PlaceViewModel? placeViewModel)
    {
        if (placeViewModel is null) return;
        var feature = _locationDtoViewModelMapper.MapToPointFeature(placeViewModel);
        var point = feature.Point;

        Map?.Navigator.CenterOnAndZoomTo(point);
    }

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

    [RelayCommand]
    private void OnLoad()
    {
        var titleSource = _locationPresentationService.GetAllKnowTitleSource();
        if (titleSource.IsSuccess) KnownTileSources.AddRangeAndSort(titleSource.Value!, s => s.ToString());
    }

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

    [RelayCommand]
    private void OnCancel(IClosable? closable)
    {
        closable?.DialogResult = false;
        closable?.Close();
    }

    [RelayCommand]
    private void OnDelete(IClosable? closable)
    {
        if (SelectedPlaceViewModel is null) return;

        // TODO continue
    }
}