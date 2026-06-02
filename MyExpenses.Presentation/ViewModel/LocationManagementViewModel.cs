using System.Collections.ObjectModel;
using BruTile.Predefined;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Manipulations;
using Mapsui.Projections;
using Mapsui.Tiling.Layers;
using Mapsui.UI;
using Microsoft.Extensions.Logging;
using MyExpenses.Presentation.Converters;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.Utils;
using MyExpenses.Presentation.ViewModels.Locations;
using MyExpenses.SharedUtils.Collection;

namespace MyExpenses.Presentation.ViewModel;

public partial class LocationManagementViewModel(ILocationPresentationService locationPresentationService,
    ILocationDtoViewModelMapper locationDtoViewModelMapper,
    INavigationWindowService navigationWindowService,
    ILogger<LocationManagementViewModel> logger,
    MapsUtils mapsUtils) : ViewModelBase
{
    private WritableLayer PlaceLayer { get; } = new() { Style = null, Tag = typeof(PlaceViewModel) };
    private IEnumerable<ILayer> PlaceLayers => [PlaceLayer];

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
        if (SelectedPlaceViewModel is null)
        {
            navigationWindowService.ShowLocationManagementWindow(SelectedPlacePoint);
        }
        else
        {
            navigationWindowService.ShowLocationManagementWindow(placeViewModel);
        }
    }

    public void LoadPlaceViewModel(PlaceViewModel placeViewModel, bool isEdit)
    {
        SelectedPlaceViewModel ??= new PlaceViewModel();
        locationDtoViewModelMapper.Merge(placeViewModel, SelectedPlaceViewModel);

        IsEditLocation = isEdit;
    }

    public void OnPositionChanged((double Longitude, double Latitude) position, IMapControl mapControl)
    {
        if (Map is null) return;

        var worldPosition = Map.Navigator.Viewport.ScreenToWorld(position.Longitude, position.Latitude);
        var lonLat = SphericalMercator.ToLonLat(worldPosition.X, worldPosition.Y);
        SelectedPlacePoint = locationDtoViewModelMapper.MapToMPoint(lonLat);

        var mapInfo = mapControl.GetMapInfo(new ScreenPosition(position.Longitude, position.Latitude), PlaceLayers);
        SetSelectedPlaceViewModel(mapInfo);
    }

    [RelayCommand]
    private void OnGoToGoogleEarthWeb()
        => mapsUtils.GoToGoogleEarthWeb(SelectedPlacePoint);

    [RelayCommand]
    private void OnGoToGoogleMaps()
        => mapsUtils.GoToGoogleMaps(SelectedPlacePoint);

    [RelayCommand]
    private void OnGoToGoogleStreetView()
        => mapsUtils.GoToGoogleStreetView(SelectedPlacePoint);

    private void OpenWebUriWithLog(string uri, string webPageTitle)
    {
        logger.LogInformation("Opening for {WebPageTitle} with the following url: {Uri}", webPageTitle, uri);
        navigationWindowService.OpenUri(uri);
    }

    [RelayCommand]
    private void OnMapInfo(MapInfoEventArgs? mapInfoEventArgs)
    {
        var mapInfo = mapInfoEventArgs?.GetMapInfo(PlaceLayers);

        SetSelectedPlaceViewModel(mapInfo);
    }

    private void SetSelectedPlaceViewModel(MapInfo? mapInfo)
    {
        SelectedPlaceViewModel = mapInfo?.Feature is not PointFeature pointFeature
            ? null
            : locationDtoViewModelMapper.MapToPlaceViewModel(pointFeature);
    }

    [RelayCommand]
    private void OnSelectedTreeViewItem(object? item)
    {
        var points = item switch
        {
            CountryGroupViewModel countryGroup => countryGroup.CityGroups?
                .SelectMany(cityGroup => GetPoints(cityGroup.Places)) ?? [],
            CityGroupViewModel cityGroup => GetPoints(cityGroup.Places),
            PlaceViewModel placeViewModel => [locationDtoViewModelMapper.MapToMPoint(placeViewModel)],
            _ => []
        };

        Map?.Navigator.SetZoom(points.ToArray());

        return;

        IEnumerable<MPoint> GetPoints(IEnumerable<PlaceViewModel>? places)
        {
            return places?
                .Where(s =>s.Latitude is not 0 && s.Longitude is not 0)
                .Select(locationDtoViewModelMapper.MapToMPoint) ?? [];
        }
    }

    [RelayCommand]
    private void OnMapControlLoaded(IMapControl mapControl)
    {
        var mapResult = locationPresentationService.GetDefaultMap(true, Mapsui.Styles.Color.Black);
        if (!mapResult.IsSuccess) return;

        Map = mapResult.Value!;
        Map.Layers.AddOnTop(PlaceLayer);
        mapControl.Map = Map;

        OnUpdateTileSource();

        if (PlaceLayer.GetFeatures().Any()) mapControl.Map.Navigator.SetZoom(PlaceLayer);
        else
        {
            var resolution = mapControl.Map.Navigator.Resolutions[2];
            mapControl.Map.Navigator.ZoomTo(resolution);
        }
    }

    [RelayCommand]
    private void OnPlaceViewModelSelected(PlaceViewModel? placeViewModel)
    {
        if (placeViewModel is null) return;
        var feature = locationDtoViewModelMapper.MapToPointFeature(placeViewModel);
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
        var titleSource = locationPresentationService.GetAllKnowTitleSource();
        if (titleSource.IsSuccess) KnownTileSources.AddRangeAndSort(titleSource.Value!, s => s.ToString());
    }

    [RelayCommand]
    private async Task OnLoadWithFeature(CancellationToken cancellationToken = default)
    {
        OnLoad();

        var resultPlaces = await locationPresentationService.GetAllPlaces(cancellationToken);
        if (!resultPlaces.IsSuccess)
        {
            logger.LogWarning("Failed to load place group: {Error}", resultPlaces.InternalMessage!);
            return;
        }

        var pointFeatures = resultPlaces.Value!
            .Where(s => s.Latitude is not null && s.Latitude is not 0 && s.Longitude is not null &&
                        s.Longitude is not 0)
            .Select(s => s.IsOpen
                ? locationDtoViewModelMapper.MapToPointFeature(s, MapsuiStyleExtensions.RedMarkerStyle)
                : locationDtoViewModelMapper.MapToPointFeature(s, MapsuiStyleExtensions.BlueMarkerStyle));

        PlaceLayer.AddRange(pointFeatures);

        var group = locationDtoViewModelMapper.MapToGroup(resultPlaces.Value!);
        CountryGroups.AddRangeAndSort(group, s => s.Country!);
    }
}