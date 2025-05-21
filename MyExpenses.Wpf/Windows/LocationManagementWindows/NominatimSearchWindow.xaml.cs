using System.Windows;
using Mapsui.Layers;
using MyExpenses.Models.AutoMapper;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.SharedUtils.Resources.Resx.NominatimSearchManagement;
using MyExpenses.Utils.Maps;
using MyExpenses.Wpf.Utils;

namespace MyExpenses.Wpf.Windows.LocationManagementWindows;

public partial class NominatimSearchWindow
{
    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty PlaceNameHintAssistProperty = DependencyProperty.Register(
        nameof(PlaceNameHintAssist),
        typeof(string), typeof(NominatimSearchWindow), new PropertyMetadata(default(string)));

    public string PlaceNameHintAssist
    {
        get => (string)GetValue(PlaceNameHintAssistProperty);
        set => SetValue(PlaceNameHintAssistProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty PlaceNumberHintAssistProperty =
        DependencyProperty.Register(nameof(PlaceNumberHintAssist), typeof(string), typeof(NominatimSearchWindow),
            new PropertyMetadata(default(string)));

    public string PlaceNumberHintAssist
    {
        get => (string)GetValue(PlaceNumberHintAssistProperty);
        set => SetValue(PlaceNumberHintAssistProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty CurrentPlaceProperty = DependencyProperty.Register(nameof(CurrentPlace),
        typeof(TPlace), typeof(NominatimSearchWindow), new PropertyMetadata(default(TPlace)));

    public TPlace CurrentPlace
    {
        get => (TPlace)GetValue(CurrentPlaceProperty);
        set => SetValue(CurrentPlaceProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty PlaceStreetHintAssistProperty =
        DependencyProperty.Register(nameof(PlaceStreetHintAssist), typeof(string), typeof(NominatimSearchWindow),
            new PropertyMetadata(default(string)));

    public string PlaceStreetHintAssist
    {
        get => (string)GetValue(PlaceStreetHintAssistProperty);
        set => SetValue(PlaceStreetHintAssistProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty PlacePostalHintAssistProperty =
        DependencyProperty.Register(nameof(PlacePostalHintAssist), typeof(string), typeof(NominatimSearchWindow),
            new PropertyMetadata(default(string)));

    public string PlacePostalHintAssist
    {
        get => (string)GetValue(PlacePostalHintAssistProperty);
        set => SetValue(PlacePostalHintAssistProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty PlaceCityHintAssistProperty =
        DependencyProperty.Register(nameof(PlaceCityHintAssist), typeof(string), typeof(NominatimSearchWindow),
            new PropertyMetadata(default(string)));

    public string PlaceCityHintAssist
    {
        get => (string)GetValue(PlaceCityHintAssistProperty);
        set => SetValue(PlaceCityHintAssistProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty PlaceCountryHintAssistProperty =
        DependencyProperty.Register(nameof(PlaceCountryHintAssist), typeof(string), typeof(NominatimSearchWindow),
            new PropertyMetadata(default(string)));

    public string PlaceCountryHintAssist
    {
        get => (string)GetValue(PlaceCountryHintAssistProperty);
        set => SetValue(PlaceCountryHintAssistProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty ButtonCancelContentProperty =
        DependencyProperty.Register(nameof(ButtonCancelContent), typeof(string), typeof(NominatimSearchWindow),
            new PropertyMetadata(default(string)));

    public string ButtonCancelContent
    {
        get => (string)GetValue(ButtonCancelContentProperty);
        set => SetValue(ButtonCancelContentProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty ButtonValidContentProperty =
        DependencyProperty.Register(nameof(ButtonValidContent), typeof(string), typeof(NominatimSearchWindow),
            new PropertyMetadata(default(string)));

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

    public NominatimSearchWindow()
    {
        var map = MapsuiMapExtensions.GetMap(false);
        map.Layers.Add(Mapsui.Tiling.OpenStreetMap.CreateTileLayer());
        map.Layers.Add(WritableLayer);

        UpdateLanguage();

        InitializeComponent();

        MapControl.Map = map;

        // ReSharper disable once HeapView.DelegateAllocation
        Interface.LanguageChanged += Interface_OnLanguageChanged;

        this.SetWindowCornerPreference();
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
        UpdatePointFeature();
        UpdateTitle();
    }

    private void UpdatePointFeature()
    {
        var feature = Mapping.Mapper.Map<PointFeature>(CurrentPlace);
        feature.Styles = [MapsuiStyleExtensions.RedMarkerStyle];
        WritableLayer.Clear();
        WritableLayer.Add(feature);
        MapControl.Map.Navigator.CenterOnAndZoomTo(feature.Point);
        MapControl.Refresh();
    }

    private void UpdateTitle()
        => Title = $"{Index}/{Total} - {CurrentPlace}";

    private void ButtonGoBack_OnClick(object sender, RoutedEventArgs e)
    {
        Index--;
        UpdateCurrentPlace();
    }

    private void ButtonGoNext_OnClick(object sender, RoutedEventArgs e)
    {
        Index++;
        UpdateCurrentPlace();
    }

    private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void ButtonValid_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }
}