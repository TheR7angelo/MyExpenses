using System.Windows;
using Mapsui.Layers;
using Mapsui.Styles;
using MyExpenses.Models.AutoMapper;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Utils.Maps;
using MyExpenses.Wpf.Resources.Resx.Windows.NominatimSearchWindow;
using MyExpenses.Wpf.Utils;
using MyExpenses.Wpf.Utils.Maps;

namespace MyExpenses.Wpf.Windows.LocationManagementWindows;

public partial class NominatimSearchWindow
{
    public static readonly DependencyProperty PlaceNameHintAssistProperty = DependencyProperty.Register(
        nameof(PlaceNameHintAssist),
        typeof(string), typeof(NominatimSearchWindow), new PropertyMetadata(default(string)));

    public string PlaceNameHintAssist
    {
        get => (string)GetValue(PlaceNameHintAssistProperty);
        set => SetValue(PlaceNameHintAssistProperty, value);
    }

    public static readonly DependencyProperty PlaceNumberHintAssistProperty =
        DependencyProperty.Register(nameof(PlaceNumberHintAssist), typeof(string), typeof(NominatimSearchWindow),
            new PropertyMetadata(default(string)));

    public string PlaceNumberHintAssist
    {
        get => (string)GetValue(PlaceNumberHintAssistProperty);
        set => SetValue(PlaceNumberHintAssistProperty, value);
    }

    public static readonly DependencyProperty CurrentPlaceProperty = DependencyProperty.Register(nameof(CurrentPlace),
        typeof(TPlace), typeof(NominatimSearchWindow), new PropertyMetadata(default(TPlace)));

    public TPlace CurrentPlace
    {
        get => (TPlace)GetValue(CurrentPlaceProperty);
        set => SetValue(CurrentPlaceProperty, value);
    }

    public static readonly DependencyProperty PlaceStreetHintAssistProperty =
        DependencyProperty.Register(nameof(PlaceStreetHintAssist), typeof(string), typeof(NominatimSearchWindow),
            new PropertyMetadata(default(string)));

    public string PlaceStreetHintAssist
    {
        get => (string)GetValue(PlaceStreetHintAssistProperty);
        set => SetValue(PlaceStreetHintAssistProperty, value);
    }

    public static readonly DependencyProperty PlacePostalHintAssistProperty =
        DependencyProperty.Register(nameof(PlacePostalHintAssist), typeof(string), typeof(NominatimSearchWindow),
            new PropertyMetadata(default(string)));

    public string PlacePostalHintAssist
    {
        get => (string)GetValue(PlacePostalHintAssistProperty);
        set => SetValue(PlacePostalHintAssistProperty, value);
    }

    public static readonly DependencyProperty PlaceCityHintAssistProperty =
        DependencyProperty.Register(nameof(PlaceCityHintAssist), typeof(string), typeof(NominatimSearchWindow),
            new PropertyMetadata(default(string)));

    public string PlaceCityHintAssist
    {
        get => (string)GetValue(PlaceCityHintAssistProperty);
        set => SetValue(PlaceCityHintAssistProperty, value);
    }

    public static readonly DependencyProperty PlaceCountryHintAssistProperty =
        DependencyProperty.Register(nameof(PlaceCountryHintAssist), typeof(string), typeof(NominatimSearchWindow),
            new PropertyMetadata(default(string)));

    public string PlaceCountryHintAssist
    {
        get => (string)GetValue(PlaceCountryHintAssistProperty);
        set => SetValue(PlaceCountryHintAssistProperty, value);
    }

    public static readonly DependencyProperty ButtonCancelContentProperty =
        DependencyProperty.Register(nameof(ButtonCancelContent), typeof(string), typeof(NominatimSearchWindow),
            new PropertyMetadata(default(string)));

    public string ButtonCancelContent
    {
        get => (string)GetValue(ButtonCancelContentProperty);
        set => SetValue(ButtonCancelContentProperty, value);
    }

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

    private WritableLayer WritableLayer { get; } = new() { Style = null };

    public NominatimSearchWindow()
    {
        var map = MapsuiMapExtensions.GetMap(false);
        map.Layers.Add(Mapsui.Tiling.OpenStreetMap.CreateTileLayer());
        map.Layers.Add(WritableLayer);

        UpdateLanguage();

        Interface.LanguageChanged += Interface_OnLanguageChanged;
        InitializeComponent();

        this.SetWindowCornerPreference();

        MapControl.Map = map;
    }

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
        => UpdateLanguage();

    private void UpdateLanguage()
    {
        PlaceNameHintAssist = NominatimSearchWindowResources.PlaceNameHintAssist;
        PlaceNumberHintAssist = NominatimSearchWindowResources.PlaceNumberHintAssist;
        PlaceStreetHintAssist = NominatimSearchWindowResources.PlaceStreetHintAssist;
        PlacePostalHintAssist = NominatimSearchWindowResources.PlacePostalHintAssist;
        PlaceCityHintAssist = NominatimSearchWindowResources.PlaceCityHintAssist;
        PlaceCountryHintAssist = NominatimSearchWindowResources.PlaceCountryHintAssist;

        ButtonCancelContent = NominatimSearchWindowResources.ButtonCancelContent;
        ButtonValidContent = NominatimSearchWindowResources.ButtonValidContent;
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
        var mapper = Mapping.Mapper;
        var feature = mapper.Map<PointFeature>(CurrentPlace);
        feature.Styles = new List<IStyle> { MapsuiStyleExtensions.RedMarkerStyle };
        WritableLayer.Clear();
        WritableLayer.Add(feature);
        MapControl.Map.Navigator.CenterOnAndZoomTo(feature.Point, 1);
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