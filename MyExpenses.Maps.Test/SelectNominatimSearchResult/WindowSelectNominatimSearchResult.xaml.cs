using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Mapsui.Layers;
using Mapsui.Styles;
using MyExpenses.Maps.Test.Utils;
using MyExpenses.Models.Sql.Tables;

namespace MyExpenses.Maps.Test.SelectNominatimSearchResult;

public partial class WindowSelectNominatimSearchResult : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private List<TPlace> Places { get; set; } = [];

    private TPlace _currentPlace { get; set; } = new();

    public TPlace CurrentPlace
    {
        get => _currentPlace;
        private set
        {
            _currentPlace = value;
            OnPropertyChanged();
        }
    }

    private int Index { get; set; }
    private int Total { get; set; }

    private WritableLayer WritableLayer { get; } = new() { Style = null };

    public WindowSelectNominatimSearchResult()
    {
        var map = MapStyle.GetMap(false);
        map.Layers.Add(Mapsui.Tiling.OpenStreetMap.CreateTileLayer());
        map.Layers.Add(WritableLayer);

        InitializeComponent();

        MapControl.Map = map;
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

        CurrentPlace = Places[Index-1];
        UpdatePointFeature();
        UpdateTitle();
    }

    private void UpdatePointFeature()
    {
        var feature = CurrentPlace.ToPointFeature();
        feature.Styles = new List<IStyle> { MapStyle.RedMarkerStyle };
        WritableLayer.Clear();
        WritableLayer.Add(feature);
        MapControl.Map.Home = n => { n.CenterOnAndZoomTo(feature.Point, 1); };
        MapControl.Refresh();
    }

    private void UpdateTitle()
    {
        Title = $"{Index}/{Total} - {CurrentPlace}";
    }

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
}