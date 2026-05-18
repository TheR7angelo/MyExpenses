using System.Collections.ObjectModel;
using BruTile.Predefined;
using CommunityToolkit.Mvvm.Input;
using Mapsui.Layers;
using Microsoft.Extensions.Logging;
using MyExpenses.Presentation.Converters;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.ViewModels.Locations;
using MyExpenses.SharedUtils.Collection;

namespace MyExpenses.Presentation.ViewModel;

public partial class LocationManagementViewModel : ViewModelBase
{
    private readonly ILocationPresentationService _locationPresentationService;
    private readonly ILocationDtoViewModelMapper _locationDtoViewModelMapper;
    private readonly ILogger<LocationManagementViewModel> _logger;

    private WritableLayer PlaceLayer { get; } = new() { Style = null, Tag = typeof(PlaceViewModel) };
    private IEnumerable<ILayer> PlaceLayers => [PlaceLayer];

    public ObservableCollection<KnownTileSource> KnownTileSources { get; } = [];
    public ObservableCollection<CountryGroupViewModel> CountryGroups { get; } = [];

    public IAsyncRelayCommand LoadCommand { get; }

    public LocationManagementViewModel(ILocationPresentationService locationPresentationService,
        ILocationDtoViewModelMapper locationDtoViewModelMapper,
        ILogger<LocationManagementViewModel> logger)
    {
        _locationPresentationService = locationPresentationService;
        _locationDtoViewModelMapper = locationDtoViewModelMapper;
        _logger = logger;

        LoadCommand = new AsyncRelayCommand(OnLoadAsync);
    }

    private async Task OnLoadAsync(CancellationToken cancellationToken = default)
    {
        var titleSource = _locationPresentationService.GetAllKnowTitleSource();
        if (titleSource.IsSuccess) KnownTileSources.AddRangeAndSort(titleSource.Value!, s => s.ToString());

        var resultPlaces = await _locationPresentationService.GetAllPlaceGroup(cancellationToken);
        if (!resultPlaces.IsSuccess)
        {
            _logger.LogWarning("Failed to load place group: {Error}", resultPlaces.InternalMessage!);
            return;
        }

        CountryGroups.AddRangeAndSort(resultPlaces.Value!, s => s.Country!);

        var pointFeatures = resultPlaces.Value!.SelectMany(s => s.CityGroups!.SelectMany(c => c.Places!))
            .Where(s => s.Latitude is not null && s.Latitude is not 0 && s.Longitude is not null &&
                        s.Longitude is not 0)
            .Select(s => s.IsOpen
                ? _locationDtoViewModelMapper.MapToPointFeature(s, MapsuiStyleExtensions.RedMarkerStyle)
                : _locationDtoViewModelMapper.MapToPointFeature(s, MapsuiStyleExtensions.BlueMarkerStyle));

        PlaceLayer.AddRange(pointFeatures);
    }
}