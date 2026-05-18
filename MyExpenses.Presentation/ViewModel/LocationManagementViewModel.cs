using System.Collections.ObjectModel;
using BruTile.Predefined;
using CommunityToolkit.Mvvm.Input;
using Mapsui.Layers;
using Microsoft.Extensions.Logging;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.ViewModels.Locations;
using MyExpenses.SharedUtils.Collection;

namespace MyExpenses.Presentation.ViewModel;

public partial class LocationManagementViewModel : ViewModelBase
{
    private readonly ILocationPresentationService _locationPresentationService;
    private readonly ILogger<LocationManagementViewModel> _logger;

    private WritableLayer PlaceLayer { get; } = new() { Style = null, Tag = typeof(PlaceViewModel) };
    private IEnumerable<ILayer> PlaceLayers => [PlaceLayer];

    public ObservableCollection<KnownTileSource> KnownTileSources { get; } = [];
    public ObservableCollection<CountryGroupViewModel> CountryGroups { get; } = [];

    public IAsyncRelayCommand LoadCommand { get; }

    public LocationManagementViewModel(ILocationPresentationService locationPresentationService,
        ILogger<LocationManagementViewModel> logger)
    {
        _locationPresentationService = locationPresentationService;
        _logger = logger;

        LoadCommand = new AsyncRelayCommand(OnLoadAsync);
    }

    private async Task OnLoadAsync(CancellationToken cancellationToken = default)
    {
        var titleSource = _locationPresentationService.GetAllKnowTitleSource();
        if (titleSource.IsSuccess) KnownTileSources.AddRangeAndSort(titleSource.Value!, s => s.ToString());

        var places = await _locationPresentationService.GetAllPlaceGroup(cancellationToken);
        if (!places.IsSuccess)
        {
            _logger.LogWarning("Failed to load place group: {Error}", places.InternalMessage!);
            return;
        }

        CountryGroups.AddRangeAndSort(places.Value!, s => s.Country!);
    }
}