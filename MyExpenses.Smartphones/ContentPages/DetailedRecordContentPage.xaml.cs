using System.Collections.ObjectModel;
using BruTile.Predefined;
using Mapsui.Layers;
using Mapsui.Tiling.Layers;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Models.Sql.Bases.Views;
using MyExpenses.Smartphones.Resources.Resx.ContentPages.DetailedRecordContentPage;
using MyExpenses.Sql.Context;
using MyExpenses.Utils.Collection;
using MyExpenses.Utils.Maps;

namespace MyExpenses.Smartphones.ContentPages;

public partial class DetailedRecordContentPage
{
    public static readonly BindableProperty LabelTextPointedOnProperty =
        BindableProperty.Create(nameof(LabelTextPointedOn), typeof(string), typeof(DetailedRecordContentPage),
            default(string));

    public string LabelTextPointedOn
    {
        get => (string)GetValue(LabelTextPointedOnProperty);
        set => SetValue(LabelTextPointedOnProperty, value);
    }

    public static readonly BindableProperty PointedOperationProperty = BindableProperty.Create(nameof(PointedOperation),
        typeof(string), typeof(DetailedRecordContentPage), default(string));

    public string PointedOperation
    {
        get => (string)GetValue(PointedOperationProperty);
        set => SetValue(PointedOperationProperty, value);
    }

    public static readonly BindableProperty LabelTextAddedOnProperty = BindableProperty.Create(nameof(LabelTextAddedOn),
        typeof(string), typeof(DetailedRecordContentPage), default(string));

    public string LabelTextAddedOn
    {
        get => (string)GetValue(LabelTextAddedOnProperty);
        set => SetValue(LabelTextAddedOnProperty, value);
    }

    public static readonly BindableProperty THistoryProperty = BindableProperty.Create(nameof(THistory),
        typeof(THistory), typeof(DetailedRecordContentPage), default(THistory));

    public THistory THistory
    {
        get => (THistory)GetValue(THistoryProperty);
        set => SetValue(THistoryProperty, value);
    }

    public static readonly BindableProperty VHistoryProperty = BindableProperty.Create(nameof(VHistory),
        typeof(VHistory), typeof(DetailedRecordContentPage), default(VHistory));

    public VHistory VHistory
    {
        get => (VHistory)GetValue(VHistoryProperty);
        set => SetValue(VHistoryProperty, value);
    }

    private WritableLayer PlaceLayer { get; } = new() { Style = null, IsMapInfoLayer = true, Tag = typeof(TPlace) };
    public List<KnownTileSource> KnownTileSources { get; private set; } = [];
    public KnownTileSource KnownTileSourceSelected { get; set; }

    public ObservableCollection<TModePayment> ModePayments { get; private set; } = [];
    public ObservableCollection<TCategoryType> CategoryTypes { get; private set; } = [];

    public DetailedRecordContentPage(int historyPk)
    {
        using var context = new DataBaseContext();
        THistory = context.THistories.First(s => s.Id.Equals(historyPk));
        VHistory = context.VHistories.First(s => s.Id.Equals(historyPk));

        InitializeContentPage();
    }

    public DetailedRecordContentPage(THistory tHistory)
    {
        using var context = new DataBaseContext();
        THistory = tHistory;
        VHistory = context.VHistories.First(s => s.Id.Equals(tHistory.Id));

        InitializeContentPage();
    }

    public DetailedRecordContentPage(VHistory vHistory)
    {
        using var context = new DataBaseContext();
        THistory = context.THistories.First(s => s.Id.Equals(vHistory.Id));
        VHistory = vHistory;

        InitializeContentPage();
    }

    private void InitializeContentPage()
    {
        using var context = new DataBaseContext();
        ModePayments.AddRange(context.TModePayments);
        CategoryTypes.AddRange(context.TCategoryTypes);

        //TODO work
        var knowTileSource = MapsuiMapExtensions.GetAllKnowTileSource();
        KnownTileSources.AddRange(knowTileSource);

        var map = MapsuiMapExtensions.GetMap(false);

        UpdateLanguage();
        InitializeComponent();

        MapControl.Map = map;

        var place = THistory.PlaceFk?.ToISql<TPlace>();
        UpdateMapPoint(place);

        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    private void UpdateMapPoint(TPlace? place)
    {
        // PlaceLayer.Clear();

        if (place is null)
        {
            MapControl.Refresh();
            return;
        }

        // var pointFeature = place.ToFeature(MapsuiStyleExtensions.RedMarkerStyle);
        var pointFeature = place.ToFeature();

        // PlaceLayer.Add(pointFeature);
        // MapControl.Map.Navigator.CenterOn(pointFeature.Point);
        // MapControl.Map.Navigator.ZoomTo(0);

        MapControl.Map.Home = navigator =>
        {
            navigator.CenterOn(pointFeature.Point);
            navigator.ZoomTo(1);
        };
    }

    private void UpdateTileLayer()
    {
        const string layerName = "Background";

        var httpTileSource = BruTile.Predefined.KnownTileSources.Create(KnownTileSourceSelected);
        var tileLayer = new TileLayer(httpTileSource);
        tileLayer.Name = layerName;

        var layers = MapControl?.Map.Layers.FindLayer(layerName);
        if (layers is not null) MapControl?.Map.Layers.Remove(layers.ToArray());

        MapControl?.Map.Layers.Insert(0, tileLayer);
    }

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
        => UpdateLanguage();

    private void UpdateLanguage()
    {
        LabelTextAddedOn = DetailedRecordContentPageResources.LabelTextAddedOn;
        PointedOperation = DetailedRecordContentPageResources.PointedOperation;
        LabelTextPointedOn = DetailedRecordContentPageResources.LabelTextPointedOn;
    }

    private void MapControl_OnLoaded(object? sender, EventArgs e)
        => UpdateTileLayer();

    private void PickerKnownTileSources_OnSelectedIndexChanged(object? sender, EventArgs e)
        => UpdateTileLayer();
}