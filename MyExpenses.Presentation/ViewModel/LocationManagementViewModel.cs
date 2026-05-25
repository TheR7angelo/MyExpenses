using System.Collections.ObjectModel;
using BruTile.Predefined;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Tiling.Layers;
using Mapsui.UI;
using Microsoft.Extensions.Logging;
using MyExpenses.Presentation.Converters;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.ViewModels.Locations;
using MyExpenses.SharedUtils.Collection;

namespace MyExpenses.Presentation.ViewModel;

public partial class LocationManagementViewModel(ILocationPresentationService locationPresentationService,
    ILocationDtoViewModelMapper locationDtoViewModelMapper,
    ILogger<LocationManagementViewModel> logger) : ViewModelBase
{
    private WritableLayer PlaceLayer { get; } = new() { Style = null, Tag = typeof(PlaceViewModel) };
    private IEnumerable<ILayer> PlaceLayers => [PlaceLayer];

    public ObservableCollection<KnownTileSource> KnownTileSources { get; } = [];
    public ObservableCollection<CountryGroupViewModel> CountryGroups { get; } = [];

    [ObservableProperty]
    public partial Map? Map { get; set; }

    [ObservableProperty]
    public partial KnownTileSource? KnownTileSourceSelected { get; set; }

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
    private async Task OnLoad(CancellationToken cancellationToken = default)
    {
        var titleSource = locationPresentationService.GetAllKnowTitleSource();
        if (titleSource.IsSuccess) KnownTileSources.AddRangeAndSort(titleSource.Value!, s => s.ToString());

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