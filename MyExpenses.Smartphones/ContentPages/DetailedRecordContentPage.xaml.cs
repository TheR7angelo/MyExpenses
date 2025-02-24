using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Versioning;
using System.Windows.Input;
using BruTile.Predefined;
using Mapsui.Layers;
using Mapsui.Tiling.Layers;
using MyExpenses.Maui.Utils;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.SharedUtils.Objects;
using MyExpenses.SharedUtils.Properties;
using MyExpenses.SharedUtils.Resources.Resx.DetailedRecordManagement;
using MyExpenses.Smartphones.Converters;
using MyExpenses.Smartphones.PackIcons;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using MyExpenses.Utils.DateTimes;
using MyExpenses.Utils.Maps;
using MyExpenses.Utils.Resources.Resx.Converters.EmptyStringTreeViewConverter;
using Serilog;
using Path = Microsoft.Maui.Controls.Shapes.Path;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace MyExpenses.Smartphones.ContentPages;

public partial class DetailedRecordContentPage
{
    public static readonly BindableProperty ButtonCanBeDeletedTextProperty =
        BindableProperty.Create(nameof(ButtonCanBeDeletedText), typeof(string), typeof(DetailedRecordContentPage));

    public string ButtonCanBeDeletedText
    {
        get => (string)GetValue(ButtonCanBeDeletedTextProperty);
        set => SetValue(ButtonCanBeDeletedTextProperty, value);
    }

    public static readonly BindableProperty LabelTextOnTheAccountProperty =
        BindableProperty.Create(nameof(LabelTextOnTheAccount), typeof(string), typeof(DetailedRecordContentPage));

    public string LabelTextOnTheAccount
    {
        get => (string)GetValue(LabelTextOnTheAccountProperty);
        set => SetValue(LabelTextOnTheAccountProperty, value);
    }

    public static readonly BindableProperty ButtonCancelUpdateTextProperty =
        BindableProperty.Create(nameof(ButtonCancelUpdateText), typeof(string), typeof(DetailedRecordContentPage));

    public string ButtonCancelUpdateText
    {
        get => (string)GetValue(ButtonCancelUpdateTextProperty);
        set => SetValue(ButtonCancelUpdateTextProperty, value);
    }

    public static readonly BindableProperty ButtonUpdateTextProperty = BindableProperty.Create(nameof(ButtonUpdateText),
        typeof(string), typeof(DetailedRecordContentPage));

    public string ButtonUpdateText
    {
        get => (string)GetValue(ButtonUpdateTextProperty);
        set => SetValue(ButtonUpdateTextProperty, value);
    }

    public static readonly BindableProperty SelectedCountryProperty = BindableProperty.Create(nameof(SelectedCountry),
        typeof(string), typeof(DetailedRecordContentPage));

    public string? SelectedCountry
    {
        get => (string?)GetValue(SelectedCountryProperty);
        set => SetValue(SelectedCountryProperty, value);
    }

    public static readonly BindableProperty SelectedCityProperty = BindableProperty.Create(nameof(SelectedCity),
        typeof(string), typeof(DetailedRecordContentPage));

    public string SelectedCity
    {
        get => (string)GetValue(SelectedCityProperty);
        set => SetValue(SelectedCityProperty, value);
    }

    public static readonly BindableProperty IsPlaceholderVisibleProperty =
        // ReSharper disable once HeapView.BoxingAllocation
        BindableProperty.Create(nameof(IsPlaceholderVisible), typeof(bool), typeof(DetailedRecordContentPage), false);

    public bool IsPlaceholderVisible
    {
        get => (bool)GetValue(IsPlaceholderVisibleProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        set => SetValue(IsPlaceholderVisibleProperty, value);
    }

    public static readonly BindableProperty PlaceholderTextPlaceProperty = BindableProperty.Create(
        nameof(PlaceholderTextPlace),
        typeof(string), typeof(DetailedRecordContentPage));

    public string PlaceholderTextPlace
    {
        get => (string)GetValue(PlaceholderTextPlaceProperty);
        set => SetValue(PlaceholderTextPlaceProperty, value);
    }

    public static readonly BindableProperty PlaceholderTextCityProperty =
        BindableProperty.Create(nameof(PlaceholderTextCity), typeof(string), typeof(DetailedRecordContentPage));

    public string PlaceholderTextCity
    {
        get => (string)GetValue(PlaceholderTextCityProperty);
        set => SetValue(PlaceholderTextCityProperty, value);
    }

    public static readonly BindableProperty PlaceholderTextCountryProperty =
        BindableProperty.Create(nameof(PlaceholderTextCountry), typeof(string), typeof(DetailedRecordContentPage));

    public string PlaceholderTextCountry
    {
        get => (string)GetValue(PlaceholderTextCountryProperty);
        set => SetValue(PlaceholderTextCountryProperty, value);
    }

    public static readonly BindableProperty ButtonRefocusTextProperty = BindableProperty.Create(
        nameof(ButtonRefocusText),
        typeof(string), typeof(DetailedRecordContentPage));

    public string ButtonRefocusText
    {
        get => (string)GetValue(ButtonRefocusTextProperty);
        set => SetValue(ButtonRefocusTextProperty, value);
    }

    public static readonly BindableProperty LabelTextPointedOnProperty =
        BindableProperty.Create(nameof(LabelTextPointedOn), typeof(string), typeof(DetailedRecordContentPage));

    public string LabelTextPointedOn
    {
        get => (string)GetValue(LabelTextPointedOnProperty);
        set => SetValue(LabelTextPointedOnProperty, value);
    }

    public static readonly BindableProperty PointedOperationProperty = BindableProperty.Create(nameof(PointedOperation),
        typeof(string), typeof(DetailedRecordContentPage));

    public string PointedOperation
    {
        get => (string)GetValue(PointedOperationProperty);
        set => SetValue(PointedOperationProperty, value);
    }

    public static readonly BindableProperty LabelTextAddedOnProperty = BindableProperty.Create(nameof(LabelTextAddedOn),
        typeof(string), typeof(DetailedRecordContentPage));

    public string LabelTextAddedOn
    {
        get => (string)GetValue(LabelTextAddedOnProperty);
        set => SetValue(LabelTextAddedOnProperty, value);
    }

    public static readonly BindableProperty IsDirtyProperty = BindableProperty.Create(nameof(IsDirty), typeof(bool),
        // ReSharper disable once HeapView.BoxingAllocation
        typeof(DetailedRecordContentPage), false);

    public bool IsDirty
    {
        get => (bool)GetValue(IsDirtyProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        set => SetValue(IsDirtyProperty, value);
    }

    public static readonly BindableProperty CanBeDeletedProperty = BindableProperty.Create(nameof(CanBeDeleted),
        // ReSharper disable once HeapView.BoxingAllocation
        typeof(bool), typeof(DetailedRecordContentPage), false);

    public bool CanBeDeleted
    {
        get => (bool)GetValue(CanBeDeletedProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        init => SetValue(CanBeDeletedProperty, value);
    }

    public static readonly BindableProperty HistorySymbolProperty = BindableProperty.Create(nameof(HistorySymbol),
        typeof(string), typeof(DetailedRecordContentPage));

    public string HistorySymbol
    {
        get => (string)GetValue(HistorySymbolProperty);
        set => SetValue(HistorySymbolProperty, value);
    }

    public static readonly BindableProperty HexadecimalColorCodeProperty =
        BindableProperty.Create(nameof(HexadecimalColorCode), typeof(string), typeof(DetailedRecordContentPage),
            "#00000000");

    public string HexadecimalColorCode
    {
        get => (string)GetValue(HexadecimalColorCodeProperty);
        set => SetValue(HexadecimalColorCodeProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    // A new instance of `THistory` is intentionally allocated here to represent the current
    // history being added or edited. This ensures that every `DetailedRecordContentPage` instance
    // starts with its own unique `THistory` object, avoiding any shared or unintended state
    // between multiple pages or operations. This allows proper handling and safe manipulation
    // of history data throughout the application's workflow.
    public THistory History { get; } = new();

    public EPackIcons CloseCircle { get; } = EPackIcons.CloseCircle;

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    // A new WritableLayer instance is intentionally allocated here to represent the layer
    // dedicated to places (TPlace). This layer acts as a container for displaying map features
    // related to places and provides the flexibility to dynamically add or remove features
    // as needed. By creating a unique instance for each `DetailedRecordContentPage`, we
    // ensure that map layers remain properly isolated and don't interfere with layers
    // managed by other pages or components in the application.
    private WritableLayer PlaceLayer { get; } = new() { Style = null, Tag = typeof(TPlace) };
    public List<KnownTileSource> KnownTileSources { get; private init; } = [];
    public KnownTileSource KnownTileSourceSelected { get; set; }

    public ObservableCollection<TModePayment> ModePayments { get; private init; } = [];
    public ObservableCollection<TAccount> Accounts { get; private init; } = [];
    public ObservableCollection<TCategoryType> CategoryTypes { get; private init; } = [];
    public ObservableCollection<string> CountriesCollection { get; private init; } = [];

    public ObservableCollection<string> CitiesCollection { get; private init; } = [];
    public List<TPlace> PlacesCollection { get; private init; } = [];

    private THistory? OriginalHistory { get; set; }

    public ICommand BackCommand { get; set; }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    // TaskCompletionSource is intentionally allocated here as it is the fundamental mechanism
    // for creating and controlling the completion of the Task exposed by `ResultDialog`.
    // This object is required to manually signal task completion (`SetResult`, `SetException`, etc.)
    // when the operation is resolved, ensuring proper asynchronous flow.
    private readonly TaskCompletionSource<bool> _taskCompletionSource = new();

    public Task<bool> ResultDialog
        => _taskCompletionSource.Task;

    private static readonly BindableProperty IsNewHistoryProperty =
        BindableProperty.Create(nameof(IsNewHistory), typeof(bool), typeof(DetailedRecordContentPage),
            // ReSharper disable once HeapView.BoxingAllocation
            false, propertyChanged: IsNewHistory_PropertyChanged);

    private static void IsNewHistory_PropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (newValue is not bool isNewHistory) return;
        if (bindable is not DetailedRecordContentPage sender) return;

        sender.ButtonUpdateText = isNewHistory
            ? DetailedRecordManagementResources.ButtonAddNewHistoryText
            : DetailedRecordManagementResources.ButtonUpdateText;
    }

    public bool IsNewHistory
    {
        get => (bool)GetValue(IsNewHistoryProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        init => SetValue(IsNewHistoryProperty, value);
    }

    public DetailedRecordContentPage()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // ReSharper disable once HeapView.DelegateAllocation
        // The Command object is explicitly created here to handle the user's interaction with the UI.
        // This allocation is necessary because `Command` encapsulates the behavior (in this case, `OnBackCommandPressed`)
        // and binds it to the associated UI element, such as a Button or a gesture.
        // This ensures proper separation between the UI and logic layers.
        BackCommand = new Command(OnBackCommandPressed);

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The creation of a new DataBaseContext instance (via `new DataBaseContext()`) is necessary to interact with the database.
        // This context provides the connection to the database and allows querying or updating data.
        // The `using` statement ensures that the context is disposed of properly after its use, freeing up resources like database connections.
        using var context = new DataBaseContext();
        ModePayments.AddRange(context.TModePayments.OrderBy(s => s.Name));
        CategoryTypes.AddRange(context.TCategoryTypes.OrderBy(s => s.Name));
        Accounts.AddRange(context.TAccounts.OrderBy(s => s.Name));

        PlacesCollection.AddRange(context.TPlaces.OrderBy(s => s.Name));

        var records = PlacesCollection.Select(s => EmptyStringTreeViewConverter.ToUnknown(s.Country)).Order()
            .Distinct();
        CountriesCollection.AddRange(records);

        records = PlacesCollection.Select(s => EmptyStringTreeViewConverter.ToUnknown(s.City)).Order().Distinct();
        CitiesCollection.AddRange(records);

        var knowTileSource = MapsuiMapExtensions.GetAllKnowTileSource();
        KnownTileSources.AddRange(knowTileSource);

        var map = MapsuiMapExtensions.GetMap(false);
        map.Layers.Add(PlaceLayer);

        UpdateLanguage();
        InitializeComponent();

        MapControl.Map = map;

        // ReSharper disable once HeapView.DelegateAllocation
        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    #region Action

    private void ButtonCancelUpdateHistory_OnClicked(object? sender, EventArgs e)
    {
        if (OriginalHistory is null) History.Reset();
        else
        {
            var place = OriginalHistory.PlaceFk?.ToISql<TPlace>();
            SelectedCountry = EmptyStringTreeViewConverter.ToUnknown(place?.Country);
            SelectedCity = EmptyStringTreeViewConverter.ToUnknown(place?.City);

            OriginalHistory.CopyPropertiesTo(History);
            Refocus();
        }

        UpdateIsDirty();
    }

    private void ButtonDeleteHistory_OnClicked(object? sender, EventArgs e)
        => _ = HandleButtonDeleteHistory();

    private void ButtonRefocus_OnClicked(object? sender, EventArgs e)
        => Refocus();

    private void ButtonUpdateHistory_OnClicked(object? sender, EventArgs e)
        => _ = HandleButtonUpdateHistory();

    private void DatePicker_OnDateSelected(object? sender, DateChangedEventArgs e)
        => UpdateIsDirty();

    private void EntryDescription_OnTextChanged(object? sender, TextChangedEventArgs e)
        => UpdateIsDirty();

    private void EntryValue_OnTextChanged(object? sender, TextChangedEventArgs e)
        => UpdateIsDirty();

    private void Interface_OnLanguageChanged()
        => UpdateLanguage();

    private void MapControl_OnLoaded(object? sender, EventArgs e)
        => UpdateTileLayer();

    private void OnBackCommandPressed()
        => _ = HandleBackCommand();

    private void PickerAccount_OnSelectedIndexChanged(object? sender, EventArgs e)
    {
        UpdateHistorySymbol();
        UpdateIsDirty();
    }

    private void PickerCategoryTypeFk_OnSelectedIndexChanged(object? sender, EventArgs e)
    {
        UpdateHexadecimalColorCode();
        UpdateIsDirty();
    }

    private void PickerKnownTileSources_OnSelectedIndexChanged(object? sender, EventArgs e)
        => UpdateTileLayer();

    private void PickerModePayment_OnSelectedIndexChanged(object? sender, EventArgs e)
        => UpdateIsDirty();

    private void SelectorCountry_OnSelectionChanged(object? sender, EventArgs e)
    {
        if (sender is not Picker comboBox) return;
        var country = comboBox.SelectedItem as string;

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The creation of a new DataBaseContext instance (via `new DataBaseContext()`) is necessary to interact with the database.
        // This context provides the connection to the database and allows querying or updating data.
        // The `using` statement ensures that the context is disposed of properly after its use, freeing up resources like database connections.
        using var context = new DataBaseContext();
        var query = context.TPlaces.Where(s => s.IsOpen);

        IQueryable<TPlace> records;

        if (!string.IsNullOrEmpty(country))
        {
            records = country.Equals(EmptyStringTreeViewConverterResources.Unknown)
                ? query.Where(s => s.Country == null)
                : query.Where(s => s.Country == country);
        }
        else
        {
            records = query;
        }

        var citiesResults = records.Select(s => EmptyStringTreeViewConverter.ToUnknown(s.City)).Distinct();

        // ReSharper disable HeapView.DelegateAllocation
        ComboBoxSelectorCity.SelectedIndexChanged -= SelectorCity_OnSelectionChanged;
        ComboBoxSelectorPlace.SelectedIndexChanged -= SelectorPlace_OnSelectionChanged;
        // ReSharper restore HeapView.DelegateAllocation

        CitiesCollection.Clear();
        CitiesCollection.AddRangeAndSort(citiesResults, s => s);

        PlacesCollection.Clear();
        PlacesCollection.AddRange(records.OrderBy(s => s.Name));

        // ReSharper disable HeapView.DelegateAllocation
        ComboBoxSelectorCity.SelectedIndexChanged += SelectorCity_OnSelectionChanged;
        ComboBoxSelectorPlace.SelectedIndexChanged += SelectorPlace_OnSelectionChanged;
        // ReSharper restore HeapView.DelegateAllocation
    }

    private void SelectorCity_OnSelectionChanged(object? sender, EventArgs e)
    {
        if (sender is not Picker comboBox) return;
        var city = comboBox.SelectedItem as string;

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The creation of a new DataBaseContext instance (via `new DataBaseContext()`) is necessary to interact with the database.
        // This context provides the connection to the database and allows querying or updating data.
        // The `using` statement ensures that the context is disposed of properly after its use, freeing up resources like database connections.
        using var context = new DataBaseContext();
        var query = context.TPlaces.Where(s => s.IsOpen);

        IQueryable<TPlace> records;

        if (!string.IsNullOrEmpty(city))
        {
            records = city.Equals(EmptyStringTreeViewConverterResources.Unknown)
                ? query.Where(s => s.City == null)
                : query.Where(s => s.City == city);
        }
        else
        {
            records = query;
        }

        if (SelectedCountry is null)
        {
            // ReSharper disable HeapView.DelegateAllocation
            ComboBoxSelectorCountry.SelectedIndexChanged -= SelectorCountry_OnSelectionChanged;
            SelectedCountry = records.First().Country;
            ComboBoxSelectorCountry.SelectedIndexChanged += SelectorCountry_OnSelectionChanged;
            // ReSharper restore HeapView.DelegateAllocation
        }

        PlacesCollection.Clear();
        PlacesCollection.AddRange(records.OrderBy(s => s.Name));
    }

    private void SelectorPlace_OnSelectionChanged(object? sender, EventArgs e)
    {
        if (sender is not Picker picker) return;

        IsPlaceholderVisible = picker.SelectedItem is null;

        var place = History.PlaceFk?.ToISql<TPlace>();
        UpdateMapPoint(place);

        try
        {
            // ReSharper disable once HeapView.DelegateAllocation
            ComboBoxSelectorCountry.SelectedIndexChanged -= SelectorCountry_OnSelectionChanged;
        }
        catch (NullReferenceException)
        {
            // Pass
            return;
        }

        // ReSharper disable once HeapView.DelegateAllocation
        ComboBoxSelectorCity.SelectedIndexChanged -= SelectorCity_OnSelectionChanged;

        var country = string.IsNullOrEmpty(place?.Country)
            ? EmptyStringTreeViewConverterResources.Unknown
            : place.Country;

        var city = string.IsNullOrEmpty(place?.City)
            ? EmptyStringTreeViewConverterResources.Unknown
            : place.City;

        ComboBoxSelectorCountry.SelectedItem = country;
        ComboBoxSelectorCity.SelectedItem = city;
        History.PlaceFk = place?.Id;

        // ReSharper disable HeapView.DelegateAllocation
        ComboBoxSelectorCountry.SelectedIndexChanged += SelectorCountry_OnSelectionChanged;
        ComboBoxSelectorCity.SelectedIndexChanged += SelectorCity_OnSelectionChanged;
        // ReSharper restore HeapView.DelegateAllocation

        UpdateIsDirty();
    }

    private void SwitchPointed_OnToggled(object? sender, ToggledEventArgs e)
        => UpdateIsDirty();

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private void TapGestureRecognizer_OnTapped(object? sender, TappedEventArgs e)
    {
        if (sender is not Path path) return;
        if (path.Parent is not Grid grid) return;
        var picker = grid.FindVisualChildren<Picker>().FirstOrDefault();
        if (picker is null) return;

        picker.SelectedItem = null;
        UpdateIsDirty();
    }

    private void TimePicker_OnTimeChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not TimePicker timePicker) return;
        var time = timePicker.Time;
        var dateTime = (DateTime)History.Date!;

        dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, time.Hours, time.Minutes, time.Seconds);
        History.Date = dateTime;

        UpdateIsDirty();
    }

    #endregion

    #region Function

    private bool AddOrEditHistory()
    {
        var now = DateTime.Now;
        if (OriginalHistory?.IsPointed is true) { if (!History.IsPointed) History.DatePointed = null; }
        else if (History.IsPointed) History.DatePointed = now;

        if (IsNewHistory) History.DateAdded = now;

        var json = History.ToJson();

        Log.Information("Attempting to add edit history : {Json}", json);
        var (success, exception) = History.AddOrEdit();

        if (success) Log.Information("Successful history editing");
        else Log.Error(exception, "Failed history editing");

        return success;
    }

    private async Task HandleBackCommand()
    {
        if (IsDirty)
        {
            var response = await DisplayAlert(
                DetailedRecordManagementResources.MessageBoxValidNewHistoryQuestionTitle,
                DetailedRecordManagementResources.MessageBoxValidNewHistoryQuestionMessage,
                DetailedRecordManagementResources.MessageBoxValidNewHistoryQuestionYesButton,
                DetailedRecordManagementResources.MessageBoxValidNewHistoryQuestionNoButton);

            if (response)
            {
                await HandleButtonUpdateHistory();
                return;
            }

            _taskCompletionSource.SetResult(false);
        }

        await Navigation.PopAsync();
    }

    private async Task HandleButtonDeleteHistory()
    {
        var response = await DisplayAlert(
            DetailedRecordManagementResources.MessageBoxDeleteHistoryQuestionTitle,
            DetailedRecordManagementResources.MessageBoxDeleteHistoryQuestionMessage,
            DetailedRecordManagementResources.MessageBoxDeleteHistoryQuestionYesButton,
            DetailedRecordManagementResources.MessageBoxDeleteHistoryQuestionNoButton);
        if (!response) return;

        var json = History.ToJson();
        Log.Information("Attempting to delete history : {Json}", json);
        var (success, exception) = History.Delete();
        if (!success)
        {
            Log.Error(exception, "An error occur while deleting the record");

            await DisplayAlert(
                DetailedRecordManagementResources.MessageBoxDeleteHistoryErrorTitle,
                DetailedRecordManagementResources.MessageBoxDeleteHistoryErrorMessage,
                DetailedRecordManagementResources.MessageBoxDeleteHistoryErrorOkButton);
            return;
        }

        Log.Information("Record was successfully deleted");
        await DisplayAlert(
            DetailedRecordManagementResources.MessageBoxDeleteHistorySuccessTitle,
            DetailedRecordManagementResources.MessageBoxDeleteHistorySuccessMessage,
            DetailedRecordManagementResources.MessageBoxDeleteHistorySuccessOkButton);

        _taskCompletionSource.SetResult(true);
        await Navigation.PopAsync();
    }

    private async Task HandleButtonUpdateHistory()
    {
        var isValidHistory = await ValidHistory();
        if (!isValidHistory) return;

        var success = AddOrEditHistory();
        if (!success)
        {
            await DisplayAlert(
                DetailedRecordManagementResources.MessageBoxValidNewHistoryErrorTitle,
                DetailedRecordManagementResources.MessageBoxValidNewHistoryErrorMessage,
                DetailedRecordManagementResources.MessageBoxValidNewHistoryErrorOkButton);
            return;
        }

        _taskCompletionSource.SetResult(true);
        await Navigation.PopAsync();
    }

    private void Refocus()
    {
        try
        {
            var features = PlaceLayer.GetFeatures();
            var firstFeature = features.FirstOrDefault();
            if (firstFeature is not PointFeature pointFeature) return;

            MapControl.Map.Navigator.CenterOnAndZoomTo(pointFeature.Point, 1);
        }
        catch (NullReferenceException)
        {
            // Pass
        }
    }

    public void SetHistory(int? historyPk = null, THistory? tHistory = null)
    {
        if (historyPk is null && tHistory is null)
        {
            throw new ArgumentNullException(nameof(historyPk), @"historyPk is null");
        }

        if (tHistory is not null)
        {
            tHistory.CopyPropertiesTo(History);
        }
        else
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            // The creation of a new DataBaseContext instance (via `new DataBaseContext()`) is necessary to interact with the database.
            // This context provides the connection to the database and allows querying or updating data.
            // The `using` statement ensures that the context is disposed of properly after its use, freeing up resources like database connections.
            using var context = new DataBaseContext();
            var history = context.THistories.First(s => s.Id.Equals(historyPk));
            history.CopyPropertiesTo(History);
        }

        TimePicker.Time = History.Date.ToTimeSpan();

        UpdateHistorySymbol();
        UpdateHexadecimalColorCode();

        OriginalHistory = History.DeepCopy();

        if (History.PlaceFk is null)
        {
            SelectedCountry = EmptyStringTreeViewConverter.ToUnknown();
            SelectedCity = EmptyStringTreeViewConverter.ToUnknown();
            ComboBoxSelectorPlace.SelectedItem = null;
        }
        else
        {
            // ReSharper disable once HeapView.DelegateAllocation
            var place = PlacesCollection.First(s => s.Id.Equals(History.PlaceFk.Value));
            SelectedCountry = EmptyStringTreeViewConverter.ToUnknown(place.Country);
            SelectedCity = EmptyStringTreeViewConverter.ToUnknown(place.City);
            ComboBoxSelectorPlace.SelectedItem = place;
        }
    }

    private void UpdateHexadecimalColorCode()
    {
        string hexadecimalColorCode;
        if (History.CategoryTypeFk is null) hexadecimalColorCode = "#00000000";
        else
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            // The creation of a new DataBaseContext instance (via `new DataBaseContext()`) is necessary to interact with the database.
            // This context provides the connection to the database and allows querying or updating data.
            // The `using` statement ensures that the context is disposed of properly after its use, freeing up resources like database connections.
            using var context = new DataBaseContext();

            // ReSharper disable once HeapView.DelegateAllocation
            var category = CategoryTypes.First(s => s.Id.Equals(History.CategoryTypeFk.Value));
            var color = context.TColors.First(s => s.Id.Equals(category.ColorFk!.Value));
            hexadecimalColorCode = color.HexadecimalColorCode!;
        }

        HexadecimalColorCode = hexadecimalColorCode;
    }

    private void UpdateHistorySymbol()
    {
        string symbol;
        if (History.AccountFk is null) symbol = string.Empty;
        else
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            // The creation of a new DataBaseContext instance (via `new DataBaseContext()`) is necessary to interact with the database.
            // This context provides the connection to the database and allows querying or updating data.
            // The `using` statement ensures that the context is disposed of properly after its use, freeing up resources like database connections.
            using var context = new DataBaseContext();
            var currency = context.TCurrencies.First(s => s.Id.Equals(History.AccountFk));
            symbol = currency.Symbol!;
        }

        HistorySymbol = symbol;
    }

    private void UpdateIsDirty()
    {
        IsDirty = !History.AreEqual(OriginalHistory);

        Title = IsDirty && !IsNewHistory
            ? DetailedRecordManagementResources.TitleIsDirty
            : string.Empty;
    }

    private void UpdateLanguage()
    {
        ButtonUpdateText = IsNewHistory
            ? DetailedRecordManagementResources.ButtonAddNewHistoryText
            : DetailedRecordManagementResources.ButtonUpdateText;
        ButtonCancelUpdateText = DetailedRecordManagementResources.ButtonCancelText;
        ButtonCanBeDeletedText = DetailedRecordManagementResources.ButtonDeleteText;

        LabelTextAddedOn = DetailedRecordManagementResources.LabelTextAddedOn;
        PointedOperation = DetailedRecordManagementResources.PointedOperation;
        LabelTextPointedOn = DetailedRecordManagementResources.LabelTextPointedOn;
        LabelTextOnTheAccount = DetailedRecordManagementResources.LabelTextOnTheAccount;

        ButtonRefocusText = DetailedRecordManagementResources.ButtonRefocusText;

        if (IsDirty) Title = DetailedRecordManagementResources.TitleIsDirty;

        PlaceholderTextCountry = DetailedRecordManagementResources.ComboBoxPlaceCountryHintAssist;
        PlaceholderTextCity = DetailedRecordManagementResources.ComboBoxPlaceCityHintAssist;
        PlaceholderTextPlace = DetailedRecordManagementResources.ComboBoxPlaceHintAssist;
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

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // A new instance of TileLayer is created here using the specified httpTileSource.
        // This layer is responsible for rendering map tiles from the configured tile source,
        // allowing the application to display background maps or other geographic data dynamically
        // based on the selected tile provider.
        var tileLayer = new TileLayer(httpTileSource);
        tileLayer.Name = layerName;

        var layers = MapControl?.Map.Layers.FindLayer(layerName);
        if (layers is not null) MapControl?.Map.Layers.Remove(layers.ToArray());

        MapControl?.Map.Layers.Insert(0, tileLayer);
    }

    private async Task<bool> ValidHistory()
    {
        // A ValidationContext is created for the History object to perform data validation.
        // This specifies the object to validate and allows passing optional services or metadata.
        // A List of ValidationResult is also initialized to store any validation errors or warnings
        // detected during the validation process.
        // ReSharper disable HeapView.ObjectAllocation.Evident
        var validationContext = new ValidationContext(History, serviceProvider: null, items: null);
        var validationResults = new List<ValidationResult>();
        // ReSharper restore HeapView.ObjectAllocation.Evident

        var isValid = Validator.TryValidateObject(History, validationContext, validationResults, true);

        if (isValid) return isValid;

        var propertyError = validationResults.First();
        var propertyMemberName = propertyError.MemberNames.First();

        var messageErrorKey = propertyMemberName switch
        {
            nameof(History.AccountFk) => nameof(DetailedRecordManagementResources.MessageBoxValidationAccountFkError),
            nameof(History.Description) => nameof(DetailedRecordManagementResources.MessageBoxValidationDescriptionError),
            nameof(History.CategoryTypeFk) => nameof(DetailedRecordManagementResources.MessageBoxValidationCategoryTypeFkError),
            nameof(History.ModePaymentFk) => nameof(DetailedRecordManagementResources.MessageBoxValidationModePaymentFkError),
            nameof(History.Value) => nameof(DetailedRecordManagementResources.MessageBoxValidationValueError),
            nameof(History.Date) => nameof(DetailedRecordManagementResources.MessageBoxValidationDateError),
            nameof(History.PlaceFk) => nameof(DetailedRecordManagementResources.MessageBoxValidationPlaceFkError),
            _ => null
        };

        var localizedErrorMessage = string.IsNullOrEmpty(messageErrorKey)
            ? propertyError.ErrorMessage!
            : DetailedRecordManagementResources.ResourceManager.GetString(messageErrorKey)!;

        await DisplayAlert(DetailedRecordManagementResources.MessageBoxValidHistoryErrorTitle, localizedErrorMessage,
            DetailedRecordManagementResources.MessageBoxValidHistoryErrorOkButton);

        return isValid;
    }

    #endregion
}