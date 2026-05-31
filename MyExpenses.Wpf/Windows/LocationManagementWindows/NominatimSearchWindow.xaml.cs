using System.Windows;
using Mapsui.Layers;
using MyExpenses.Models.AutoMapper;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Presentation;
using MyExpenses.Presentation.Converters;
using MyExpenses.SharedUtils.Resources.Resx.NominatimSearchManagement;
using MyExpenses.Utils.Maps;

namespace MyExpenses.Wpf.Windows.LocationManagementWindows;

public partial class NominatimSearchWindow
{
    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty CurrentPlaceProperty = DependencyProperty.Register(nameof(CurrentPlace),
        typeof(TPlace), typeof(NominatimSearchWindow), new PropertyMetadata(default(TPlace)));

    public TPlace CurrentPlace
    {
        get => (TPlace)GetValue(CurrentPlaceProperty);
        set => SetValue(CurrentPlaceProperty, value);
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
    }

    private void Interface_OnLanguageChanged()
        => UpdateLanguage();

    private void UpdateLanguage()
    {
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