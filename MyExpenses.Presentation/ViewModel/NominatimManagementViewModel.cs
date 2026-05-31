using System.Collections.ObjectModel;
using BruTile.Predefined;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Tiling.Layers;
using Mapsui.UI;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.ViewModels.Locations;
using MyExpenses.SharedUtils.Collection;

namespace MyExpenses.Presentation.ViewModel;

public partial class NominatimManagementViewModel(ILocationPresentationService locationPresentationService) : ViewModelBase
{
    private NominatimSearchResultViewModel[] _searchResults = [];

    private int NominatimSearchResultViewModelCount => _searchResults.Length;

    [ObservableProperty]
    public partial NominatimSearchResultViewModel CurrentSearchResult { get; private set; } = null!;

    [ObservableProperty]
    public partial int CurrentIndex { get; private set; }

    [ObservableProperty]
    public partial string WindowTitle { get; private set; } = string.Empty;

    [ObservableProperty]
    public partial Map? Map { get; set; }

    public ObservableCollection<KnownTileSource> KnownTileSources { get; } = [];

    private WritableLayer WritableLayer { get; } = new() { Style = null };

    [ObservableProperty]
    public partial KnownTileSource? KnownTileSourceSelected { get; set; }

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

        var pointFeature = locationPresentationService.MapToPointFeature(CurrentSearchResult);
        WritableLayer.Add(pointFeature);
    }

    [RelayCommand]
    private void OnMapControlLoaded(IMapControl mapControl)
    {
        var mapResult = locationPresentationService.GetDefaultMap(true, Mapsui.Styles.Color.Black);
        if (!mapResult.IsSuccess) return;

        Map = mapResult.Value!;
        Map.Layers.AddOnTop(WritableLayer);

        OnUpdateTileSource();

        if (WritableLayer.GetFeatures().Any()) mapControl.Map.Navigator.SetZoom(WritableLayer);
        else
        {
            var resolution = mapControl.Map.Navigator.Resolutions[2];
            mapControl.Map.Navigator.ZoomTo(resolution);
        }
    }

    public void LoadNominatimSearchResults(IEnumerable<NominatimSearchResultViewModel> searchResults)
    {
        _searchResults = searchResults.ToArray();
        CurrentSearchResult = _searchResults[CurrentIndex];
        WindowTitle = $"{CurrentIndex+1}/{NominatimSearchResultViewModelCount} - {CurrentSearchResult.DisplayName}";
    }
}