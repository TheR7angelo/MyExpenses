using BruTile.Predefined;
using Domain.Models.Validation;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Styles;
using Mapsui.Tiling.Layers;
using Mapsui.Widgets;
using Mapsui.Widgets.ButtonWidgets;
using Mapsui.Widgets.InfoWidgets;
using Mapsui.Widgets.ScaleBar;
using MyExpenses.Application.Interfaces.IServices;
using MyExpenses.Presentation.Converters;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.ViewModels.Locations;
using NetTopologySuite.Geometries;

namespace MyExpenses.Presentation.Services;

public class LocationPresentationService(ILocationDtoViewModelMapper locationDtoViewModelMapper,
    ILocationService locationService) : ILocationPresentationService
{
    private const string EpsgCode = "EPSG:3857";

    private static readonly HashSet<KnownTileSource> BlackListKnownTileSource =
    [
        KnownTileSource.OpenCycleMap, KnownTileSource.OpenCycleMapTransport,
        KnownTileSource.StamenToner, KnownTileSource.StamenTonerLite, KnownTileSource.StamenWatercolor,
        KnownTileSource.StamenTerrain, KnownTileSource.EsriWorldReferenceOverlay,
        KnownTileSource.EsriWorldBoundariesAndPlaces, KnownTileSource.HereHybrid,
        KnownTileSource.HereTerrain
    ];

    public Result<IEnumerable<KnownTileSource>> GetAllKnowTitleSource()
    {
        try
        {
            var records = Enum.GetValues<KnownTileSource>().Where(s => !BlackListKnownTileSource.Contains(s)).ToList();
            return Result<IEnumerable<KnownTileSource>>.Success(records);
        }
        catch (Exception e)
        {
            return Result<IEnumerable<KnownTileSource>>.Failure(ErrorCode.UnknownError, e.Message);
        }
    }

    public Result<Map> GetDefaultMap(bool widgetEnabled, Color? backgroundColor = null)
    {
        try
        {
            backgroundColor ??= Color.Black;

            // ReSharper disable once HeapView.ObjectAllocation.Evident
            // Map initialization allocates memory for persistent objects like CRS and BackColor.
            var map = new Map { CRS = EpsgCode, BackColor = backgroundColor.Value };
            if (widgetEnabled)
            {
                // ReSharper disable HeapView.ObjectAllocation.Evident
                // Widgets are added to the map using a heap-allocated List<IWidget> to ensure persistence and support MapsUI's dynamic behavior.
                map.Widgets.AddRange(new List<IWidget>
                {
                    new MapInfoWidget(map, s => s is not TileLayer),
                    new ZoomInOutWidget(),
                    new ScaleBarWidget(map)
                });
                // ReSharper restore HeapView.ObjectAllocation.Evident
            }

            return Result<Map>.Success(map);
        }
        catch (Exception e)
        {
            return Result<Map>.Failure(ErrorCode.UnknownError, e.Message);
        }
    }

    public async Task<PlaceViewModel?> GetPlaceViewModel(int placeId, CancellationToken cancellationToken = default)
    {
        var placeDto = await locationService.GetPlace(placeId, cancellationToken);
        if (placeDto is null) return null;

        var placeViewModel = locationDtoViewModelMapper.MapToViewModel(placeDto);
        return placeViewModel;
    }

    public async Task<Result<IEnumerable<PlaceViewModel>>> GetAllPlaces(CancellationToken cancellationToken = default)
    {
        var result = await locationService.GetAllPlaces(cancellationToken);
        if (!result.IsSuccess) return Result<IEnumerable<PlaceViewModel>>.Failure(result.ErrorCode, result.InternalMessage!);

        var placeViewModels = result.Value!.Select(locationDtoViewModelMapper.MapToViewModel);
        return Result<IEnumerable<PlaceViewModel>>.Success(placeViewModels);
    }

    public IEnumerable<CountryGroupViewModel> GetAllPlaceGroup(IEnumerable<PlaceViewModel> placeViewModels, CancellationToken cancellationToken = default)
    {
        var group = locationDtoViewModelMapper.MapToGroup(placeViewModels);
        return group;
    }

    public PointFeature MapToPointFeature(NominatimSearchResultViewModel currentSearchResult)
    {
        var centerPoint = currentSearchResult.GeoJson?.CenterPoint ?? Point.Empty;

        var point = SphericalMercator.FromLonLat(centerPoint.X, centerPoint.Y);
        var pointFeature = locationDtoViewModelMapper.MapToPointFeature(point);

        pointFeature.Styles.Clear();
        pointFeature.Styles.Add(MapsuiStyleExtensions.RedMarkerStyle);

        return pointFeature;
    }

    public async Task<Result<PlaceViewModel>> CreatePlaceAsync(PlaceViewModel placeViewModel, CancellationToken cancellationToken = default)
    {
        var placeDto = locationDtoViewModelMapper.MapToDto(placeViewModel);
        var result = await locationService.CreatePlaceAsync(placeDto, cancellationToken);

        return result.IsSuccess
            ? Result<PlaceViewModel>.Success(locationDtoViewModelMapper.MapToViewModel(result.Value!))
            : Result<PlaceViewModel>.Failure(result.ErrorCode, result.InternalMessage!);
    }

    public async Task<Result<PlaceViewModel>> UpdatePlaceAsync(PlaceViewModel placeViewModel, CancellationToken cancellationToken = default)
    {
        var placeDto = locationDtoViewModelMapper.MapToDto(placeViewModel);
        var result = await locationService.UpdatePlaceAsync(placeDto, cancellationToken);

        return result.IsSuccess
            ? Result<PlaceViewModel>.Success(locationDtoViewModelMapper.MapToViewModel(result.Value!))
            : Result<PlaceViewModel>.Failure(result.ErrorCode, result.InternalMessage!);
    }

    public Task<DeletionResult> DeletePlaceAsync(PlaceViewModel placeViewModel, CancellationToken cancellationToken = default)
    {
        var placeDto = locationDtoViewModelMapper.MapToDto(placeViewModel);
        return locationService.DeletePlaceAsync(placeDto, cancellationToken);
    }

    // public async Task<Result<IEnumerable<CountryGroupViewModel>>> GetAllPlaceGroup(CancellationToken cancellationToken = default)
    // {
    //     var result = await locationService.GetAllPlaces(cancellationToken);
    //     if (!result.IsSuccess) return Result<IEnumerable<CountryGroupViewModel>>.Failure(result.ErrorCode, result.InternalMessage!);
    //
    //     var group = locationDtoViewModelMapper.MapToGroup(result.Value!);
    //     return Result<IEnumerable<CountryGroupViewModel>>.Success(group);
    // }
}