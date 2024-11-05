using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using BruTile.Predefined;
using Mapsui.Layers;
using Mapsui.Tiling.Layers;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Models.Sql.Bases.Views;
using MyExpenses.Smartphones.Resources.Resx.ContentPages.DetailedRecordContentPage;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using MyExpenses.Utils.Collection;
using MyExpenses.Utils.Maps;
using MyExpenses.Utils.Objects;
using Serilog;

namespace MyExpenses.Smartphones.ContentPages;

public partial class DetailedRecordContentPage
{
    public static readonly BindableProperty ButtonCancelUpdateTextProperty =
        BindableProperty.Create(nameof(ButtonCancelUpdateText), typeof(string), typeof(DetailedRecordContentPage),
            default(string));

    public string ButtonCancelUpdateText
    {
        get => (string)GetValue(ButtonCancelUpdateTextProperty);
        set => SetValue(ButtonCancelUpdateTextProperty, value);
    }

    public static readonly BindableProperty ButtonUpdateTextProperty = BindableProperty.Create(nameof(ButtonUpdateText),
        typeof(string), typeof(DetailedRecordContentPage), default(string));

    public string ButtonUpdateText
    {
        get => (string)GetValue(ButtonUpdateTextProperty);
        set => SetValue(ButtonUpdateTextProperty, value);
    }

    public static readonly BindableProperty ButtonRefocusTextProperty = BindableProperty.Create(
        nameof(ButtonRefocusText),
        typeof(string), typeof(DetailedRecordContentPage), default(string));

    public string ButtonRefocusText
    {
        get => (string)GetValue(ButtonRefocusTextProperty);
        set => SetValue(ButtonRefocusTextProperty, value);
    }

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

    public static readonly BindableProperty IsDirtyProperty = BindableProperty.Create(nameof(IsDirty), typeof(bool),
        typeof(DetailedRecordContentPage), default(bool));

    public bool IsDirty
    {
        get => (bool)GetValue(IsDirtyProperty);
        set => SetValue(IsDirtyProperty, value);
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

    private THistory OriginalHistory { get; set; } = null!;

    public ICommand BackCommand { get; set; } = null!;

    private readonly TaskCompletionSource<bool> _taskCompletionSource = new();

    public Task<bool> ResultDialog
        => _taskCompletionSource.Task;

    public DetailedRecordContentPage(int historyPk)
    {
        using var context = new DataBaseContext();
        THistory = context.THistories.First(s => s.Id.Equals(historyPk));
        VHistory = context.VHistories.First(s => s.Id.Equals(historyPk));

        InitializeContentPage();
    }

    public DetailedRecordContentPage(THistory tHistory)
    {
        THistory = tHistory;
        VHistory = tHistory.Id.ToISql<VHistory>()!;

        InitializeContentPage();
    }

    public DetailedRecordContentPage(VHistory vHistory)
    {
        THistory = vHistory.Id.ToISql<THistory>()!;
        VHistory = vHistory;

        InitializeContentPage();
    }

    #region Action

    private void ButtonCancelUpdateHistory_OnClicked(object? sender, EventArgs e)
    {
        OriginalHistory.CopyPropertiesTo(THistory);
        Refocus();

        UpdateIsDirty();
    }

    private void ButtonRefocus_OnClicked(object? sender, EventArgs e)
        => Refocus();

    private async void ButtonUpdateHistory_OnClicked(object? sender, EventArgs e)
    {
        var success = AddOrEditHistory();
        if (!success)
        {
            await DisplayAlert(
                DetailedRecordContentPageResources.MessageBoxOnBackCommandPressedErrorTitle,
                DetailedRecordContentPageResources.MessageBoxOnBackCommandPressedErrorMessage,
                DetailedRecordContentPageResources.MessageBoxOnBackCommandPressedErrorOkButton);
            return;
        }

        _taskCompletionSource.SetResult(true);
        await Navigation.PopAsync();
    }

    private void DatePicker_OnDateSelected(object? sender, DateChangedEventArgs e)
        => UpdateIsDirty();

    private void EntryDescription_OnTextChanged(object? sender, TextChangedEventArgs e)
        => UpdateIsDirty();

    private void EntryValue_OnTextChanged(object? sender, TextChangedEventArgs e)
        => UpdateIsDirty();

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
        => UpdateLanguage();

    private void MapControl_OnLoaded(object? sender, EventArgs e)
        => UpdateTileLayer();

    private async void OnBackCommandPressed()
    {
        if (IsDirty)
        {
            var response = await DisplayAlert(
                DetailedRecordContentPageResources.MessageBoxOnBackCommandPressedTitle,
                DetailedRecordContentPageResources.MessageBoxOnBackCommandPressedMessage,
                DetailedRecordContentPageResources.MessageBoxOnBackCommandPressedYesButton,
                DetailedRecordContentPageResources.MessageBoxOnBackCommandPressedNoButton);

            if (response)
            {
                var success = AddOrEditHistory();
                if (!success)
                {
                    await DisplayAlert(
                        DetailedRecordContentPageResources.MessageBoxOnBackCommandPressedErrorTitle,
                        DetailedRecordContentPageResources.MessageBoxOnBackCommandPressedErrorMessage,
                        DetailedRecordContentPageResources.MessageBoxOnBackCommandPressedErrorOkButton);
                    return;
                }

                _taskCompletionSource.SetResult(true);
            }
            else _taskCompletionSource.SetResult(false);
        }

        await Navigation.PopAsync();
    }

    private void PickerCategoryTypeFk_OnSelectedIndexChanged(object? sender, EventArgs e)
        => UpdateIsDirty();

    private void PickerKnownTileSources_OnSelectedIndexChanged(object? sender, EventArgs e)
        => UpdateTileLayer();

    private void PickerModePayment_OnSelectedIndexChanged(object? sender, EventArgs e)
        => UpdateIsDirty();

    private void SwitchPointed_OnToggled(object? sender, ToggledEventArgs e)
        => UpdateIsDirty();

    private void TimePicker_OnTimeChanged(object? sender, PropertyChangedEventArgs e)
        => UpdateIsDirty();

    #endregion

    #region Function

    private bool AddOrEditHistory()
    {
        var json = THistory.ToJson();

        Log.Information("Attempting to add edit history : {Json}", json);
        var (success, exception) = THistory.AddOrEdit();

        if (success) Log.Information("Successful history editing");
        else Log.Error(exception, "Failed history editing");

        return success;
    }

    private void InitializeContentPage()
    {
        BackCommand = new Command(OnBackCommandPressed);

        OriginalHistory = THistory.DeepCopy()!;

        using var context = new DataBaseContext();
        ModePayments.AddRange(context.TModePayments);
        CategoryTypes.AddRange(context.TCategoryTypes);

        var knowTileSource = MapsuiMapExtensions.GetAllKnowTileSource();
        KnownTileSources.AddRange(knowTileSource);

        var map = MapsuiMapExtensions.GetMap(false);
        map.Layers.Add(PlaceLayer);

        UpdateLanguage();
        InitializeComponent();

        MapControl.Map = map;

        var place = THistory.PlaceFk?.ToISql<TPlace>();
        UpdateMapPoint(place);

        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    private void Refocus()
    {
        var features = PlaceLayer.GetFeatures();
        var firstFeature = features.FirstOrDefault();
        if (firstFeature is not PointFeature pointFeature) return;

        MapControl.Map.Navigator.CenterOn(pointFeature.Point);
        MapControl.Map.Navigator.ZoomTo(0);

        MapControl.Map.Home = navigator =>
        {
            navigator.CenterOn(pointFeature.Point);
            navigator.ZoomTo(1);
        };
    }

    private void UpdateIsDirty()
    {
        IsDirty = !THistory.AreEqual(OriginalHistory);

        Title = IsDirty
            ? DetailedRecordContentPageResources.TitleIsDirty
            : string.Empty;
    }

    private void UpdateLanguage()
    {
        ButtonUpdateText = DetailedRecordContentPageResources.ButtonUpdateText;
        ButtonCancelUpdateText = DetailedRecordContentPageResources.ButtonCancelUpdateText;

        LabelTextAddedOn = DetailedRecordContentPageResources.LabelTextAddedOn;
        PointedOperation = DetailedRecordContentPageResources.PointedOperation;
        LabelTextPointedOn = DetailedRecordContentPageResources.LabelTextPointedOn;

        ButtonRefocusText = DetailedRecordContentPageResources.ButtonRefocusText;

        if (IsDirty) Title = DetailedRecordContentPageResources.TitleIsDirty;
    }

    private void UpdateMapPoint(TPlace? place)
    {
        PlaceLayer.Clear();

        if (place is null)
        {
            MapControl.Refresh();
            return;
        }

        var symbolStyle = place.IsOpen
            ? MapsuiStyleExtensions.RedMarkerStyle
            : MapsuiStyleExtensions.BlueMarkerStyle;

        var pointFeature = place.ToFeature(symbolStyle);

        PlaceLayer.Add(pointFeature);
        Refocus();
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

    #endregion
}