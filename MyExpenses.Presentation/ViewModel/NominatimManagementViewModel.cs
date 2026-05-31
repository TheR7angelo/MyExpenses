using System.Collections.ObjectModel;
using BruTile.Predefined;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Tiling.Layers;
using Mapsui.UI;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.Utils;
using MyExpenses.Presentation.ViewModels.Locations;
using MyExpenses.SharedUtils.Collection;

namespace MyExpenses.Presentation.ViewModel;

/// <summary>
/// Manages the view model for a window that allows managing locations using Nominatim search.
/// </summary>
public partial class NominatimManagementViewModel(
    ILocationPresentationService locationPresentationService,
    ILocationDtoViewModelMapper locationDtoViewModelMapper,
    MapsUtils mapsUtils) : ViewModelBase
{
    /// <summary>
    /// Stores an array of search results retrieved from the Nominatim API.
    /// </summary>
    private NominatimSearchResultViewModel[] _searchResults = [];

    /// <summary>
    /// Gets the count of search results.
    /// </summary>
    private int NominatimSearchResultViewModelCount => _searchResults.Length;

    /// <summary>
    /// Gets or sets the selected place point.
    /// </summary>
    private MPoint SelectedPlacePoint { get; set; } = new(0, 0);

    /// <summary>
    /// Gets or sets the current search result.
    /// </summary>
    [ObservableProperty]
    public partial NominatimSearchResultViewModel CurrentSearchResult { get; private set; } = null!;

    /// <summary>
    /// Gets or sets the index of the current search result.
    /// </summary>
    private int CurrentIndex { get; set; }

    /// <summary>
    /// Gets or sets the title of the window.
    /// </summary>
    [ObservableProperty]
    public partial string WindowTitle { get; private set; } = string.Empty;

    /// <summary>
    /// Represents the map control for displaying geographic information.
    /// </summary>
    [ObservableProperty]
    public partial Map? Map { get; set; }

    /// <summary>
    /// Gets the collection of known tile sources available for use.
    /// </summary>
    public ObservableCollection<KnownTileSource> KnownTileSources { get; } = [];

    /// <summary>
    /// Gets the WritableLayer instance used for dynamic map layers.
    /// </summary>
    private WritableLayer WritableLayer { get; } = new() { Style = null };

    /// <summary>
    /// Gets or sets the currently selected known tile source.
    /// </summary>
    [ObservableProperty]
    public partial KnownTileSource? KnownTileSourceSelected { get; set; }

    /// <summary>
    /// Updates the tile source for the map.
    /// </summary>
    /// <param name="knownTileSource">The known tile source to use. If null, OpenStreetMap will be used by default.</param>
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
    private void OnLoad(CancellationToken cancellationToken = default)
    {
        var titleSource = locationPresentationService.GetAllKnowTitleSource();
        if (titleSource.IsSuccess) KnownTileSources.AddRangeAndSort(titleSource.Value!, s => s.ToString());

        UpdatePointFeature();
    }

    /// <summary>
    /// Handles the map control loaded event.
    /// </summary>
    [RelayCommand]
    private void OnMapControlLoaded()
    {
        var mapResult = locationPresentationService.GetDefaultMap(true, Mapsui.Styles.Color.Black);
        if (!mapResult.IsSuccess) return;

        Map = mapResult.Value!;
        Map.Layers.AddOnTop(WritableLayer);

        OnUpdateTileSource();
        UpdatePointFeature();
    }

    /// <summary>
    /// Moves to the next or previous search result based on the given offset.
    /// </summary>
    /// <param name="offset">The number of positions to move. Positive values move forward, negative values move backward.</param>
    [RelayCommand]
    private void OnMoveNominatim(int offset)
    {
        CurrentIndex += offset;
        if (CurrentIndex < 0) CurrentIndex = NominatimSearchResultViewModelCount - 1;
        else if (CurrentIndex >= NominatimSearchResultViewModelCount) CurrentIndex = 0;

        CurrentSearchResult = _searchResults[CurrentIndex];
        WindowTitle = $"{CurrentIndex + 1}/{NominatimSearchResultViewModelCount} - {CurrentSearchResult.DisplayName}";

        UpdatePointFeature();
    }

    /// <summary>
    /// Updates the point feature on the map based on the current search result.
    /// </summary>
    private void UpdatePointFeature()
    {
        var pointFeature = locationPresentationService.MapToPointFeature(CurrentSearchResult);

        WritableLayer.Clear();
        WritableLayer.Add(pointFeature);

        if (WritableLayer.GetFeatures().Any()) Map?.Navigator.SetZoom(WritableLayer);
        else
        {
            var resolution = Map?.Navigator.Resolutions[2];
            if (resolution is null) return;
            Map?.Navigator.ZoomTo((double)resolution);
        }
    }

    /// <summary>
    /// Navigates to Google Earth Web at the current selected place point.
    /// </summary>
    [RelayCommand]
    private void OnGoToGoogleEarthWeb()
        => mapsUtils.GoToGoogleEarthWeb(SelectedPlacePoint);

    /// <summary>
    /// Navigates to Google Maps at the current selected place point.
    /// </summary>
    [RelayCommand]
    private void OnGoToGoogleMaps()
        => mapsUtils.GoToGoogleMaps(SelectedPlacePoint);

    /// <summary>
    /// Navigates to Google Street View at the current selected place point.
    /// </summary>
    [RelayCommand]
    private void OnGoToGoogleStreetView()
        => mapsUtils.GoToGoogleStreetView(SelectedPlacePoint);

    /// <summary>
    /// Handles the position change event by updating the selected place point.
    /// </summary>
    /// <param name="position">The new position as a tuple containing longitude and latitude.</param>
    /// <param name="mapControl">The map control on which the position change occurred.</param>
    public void OnPositionChanged((double Longitude, double Latitude) position, IMapControl mapControl)
    {
        if (Map is null) return;

        var worldPosition = Map.Navigator.Viewport.ScreenToWorld(position.Longitude, position.Latitude);
        var lonLat = SphericalMercator.ToLonLat(worldPosition.X, worldPosition.Y);
        SelectedPlacePoint = locationDtoViewModelMapper.MapToMPoint(lonLat);
    }

    /// <summary>
    /// Loads Nominatim search results into the view model.
    /// </summary>
    /// <param name="searchResults">The collection of Nominatim search result view models to load.</param>
    public void LoadNominatimSearchResults(IEnumerable<NominatimSearchResultViewModel> searchResults)
    {
        _searchResults = searchResults.ToArray();
        CurrentSearchResult = _searchResults[CurrentIndex];
        WindowTitle = $"{CurrentIndex + 1}/{NominatimSearchResultViewModelCount} - {CurrentSearchResult.DisplayName}";

        OnMoveNominatim(0);
    }
}