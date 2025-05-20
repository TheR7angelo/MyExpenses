using Mapsui.Layers;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.SharedUtils.Resources.Resx.NominatimSearchManagement;
using MyExpenses.Utils.Maps;

namespace MyExpenses.Smartphones.ContentPages.LocationManagement;

public partial class NominatimSearchContentPage
{
        // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly BindableProperty PlaceNameHintAssistProperty = BindableProperty.Create(
        nameof(PlaceNameHintAssist),
        typeof(string), typeof(NominatimSearchContentPage));

    public string PlaceNameHintAssist
    {
        get => (string)GetValue(PlaceNameHintAssistProperty);
        set => SetValue(PlaceNameHintAssistProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly BindableProperty PlaceNumberHintAssistProperty =
        BindableProperty.Create(nameof(PlaceNumberHintAssist), typeof(string), typeof(NominatimSearchContentPage));

    public string PlaceNumberHintAssist
    {
        get => (string)GetValue(PlaceNumberHintAssistProperty);
        set => SetValue(PlaceNumberHintAssistProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly BindableProperty CurrentPlaceProperty = BindableProperty.Create(nameof(CurrentPlace),
        typeof(TPlace), typeof(NominatimSearchContentPage));

    public TPlace CurrentPlace
    {
        get => (TPlace)GetValue(CurrentPlaceProperty);
        set => SetValue(CurrentPlaceProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly BindableProperty PlaceStreetHintAssistProperty =
        BindableProperty.Create(nameof(PlaceStreetHintAssist), typeof(string), typeof(NominatimSearchContentPage));

    public string PlaceStreetHintAssist
    {
        get => (string)GetValue(PlaceStreetHintAssistProperty);
        set => SetValue(PlaceStreetHintAssistProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly BindableProperty PlacePostalHintAssistProperty =
        BindableProperty.Create(nameof(PlacePostalHintAssist), typeof(string), typeof(NominatimSearchContentPage));

    public string PlacePostalHintAssist
    {
        get => (string)GetValue(PlacePostalHintAssistProperty);
        set => SetValue(PlacePostalHintAssistProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly BindableProperty PlaceCityHintAssistProperty =
        BindableProperty.Create(nameof(PlaceCityHintAssist), typeof(string), typeof(NominatimSearchContentPage));

    public string PlaceCityHintAssist
    {
        get => (string)GetValue(PlaceCityHintAssistProperty);
        set => SetValue(PlaceCityHintAssistProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly BindableProperty PlaceCountryHintAssistProperty =
        BindableProperty.Create(nameof(PlaceCountryHintAssist), typeof(string), typeof(NominatimSearchContentPage));

    public string PlaceCountryHintAssist
    {
        get => (string)GetValue(PlaceCountryHintAssistProperty);
        set => SetValue(PlaceCountryHintAssistProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly BindableProperty ButtonCancelContentProperty =
        BindableProperty.Create(nameof(ButtonCancelContent), typeof(string), typeof(NominatimSearchContentPage));

    public string ButtonCancelContent
    {
        get => (string)GetValue(ButtonCancelContentProperty);
        set => SetValue(ButtonCancelContentProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly BindableProperty ButtonValidContentProperty =
        BindableProperty.Create(nameof(ButtonValidContent), typeof(string), typeof(NominatimSearchContentPage));

    public string ButtonValidContent
    {
        get => (string)GetValue(ButtonValidContentProperty);
        set => SetValue(ButtonValidContentProperty, value);
    }

    private List<TPlace> Places { get; } = [];

    private int Index { get; set; }
    private int Total { get; set; }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    private WritableLayer WritableLayer { get; } = new() { Style = null };

    public NominatimSearchContentPage()
    {
        var map = MapsuiMapExtensions.GetMap(false);
        map.Layers.Add(Mapsui.Tiling.OpenStreetMap.CreateTileLayer());
        map.Layers.Add(WritableLayer);

        UpdateLanguage();
        InitializeComponent();

        // MapControl.Map = map;

        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    private void Interface_OnLanguageChanged()
        => UpdateLanguage();

    private void UpdateLanguage()
    {
        PlaceNameHintAssist = NominatimSearchManagementResources.PlaceNameHintAssist;
        PlaceNumberHintAssist = NominatimSearchManagementResources.PlaceNumberHintAssist;
        PlaceStreetHintAssist = NominatimSearchManagementResources.PlaceStreetHintAssist;
        PlacePostalHintAssist = NominatimSearchManagementResources.PlacePostalHintAssist;
        PlaceCityHintAssist = NominatimSearchManagementResources.PlaceCityHintAssist;
        PlaceCountryHintAssist = NominatimSearchManagementResources.PlaceCountryHintAssist;

        ButtonCancelContent = NominatimSearchManagementResources.ButtonCancelContent;
        ButtonValidContent = NominatimSearchManagementResources.ButtonValidContent;
    }

    public void AddRange(IEnumerable<TPlace> places)
    {
        Places.AddRange(places);
        Index = 1;
        Total = Places.Count;

        UpdateCurrentPlace();
    }

    private void UpdateCurrentPlace()
    {
        if (Index.Equals(0)) Index = Total;
        if (Index.Equals(Total + 1)) Index = 1;

        CurrentPlace = Places[Index - 1];
        // UpdatePointFeature();
        // UpdateTitle();
    }
}