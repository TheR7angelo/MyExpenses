using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using BruTile.Predefined;
using Mapsui.Layers;
using Mapsui.Tiling.Layers;
using MyExpenses.Maui.Utils;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Models.Sql.Bases.Views;
using MyExpenses.Smartphones.Converters;
using MyExpenses.Smartphones.PackIcons;
using MyExpenses.Smartphones.Resources.Resx.ContentPages.DetailedRecordContentPage;
using MyExpenses.Smartphones.Resources.Resx.Converters.EmptyStringTreeViewConverter;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using MyExpenses.Utils.Collection;
using MyExpenses.Utils.DateTimes;
using MyExpenses.Utils.Maps;
using MyExpenses.Utils.Objects;
using Serilog;
using Path = Microsoft.Maui.Controls.Shapes.Path;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace MyExpenses.Smartphones.ContentPages;

public partial class DetailedRecordContentPage
{
    public static readonly BindableProperty ButtonCanBeDeletedTextProperty =
        BindableProperty.Create(nameof(ButtonCanBeDeletedText), typeof(string), typeof(DetailedRecordContentPage),
            default(string));

    public string ButtonCanBeDeletedText
    {
        get => (string)GetValue(ButtonCanBeDeletedTextProperty);
        set => SetValue(ButtonCanBeDeletedTextProperty, value);
    }

    public static readonly BindableProperty LabelTextOnTheAccountProperty =
        BindableProperty.Create(nameof(LabelTextOnTheAccount), typeof(string), typeof(DetailedRecordContentPage),
            default(string));

    public string LabelTextOnTheAccount
    {
        get => (string)GetValue(LabelTextOnTheAccountProperty);
        set => SetValue(LabelTextOnTheAccountProperty, value);
    }

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

    public static readonly BindableProperty SelectedCountryProperty = BindableProperty.Create(nameof(SelectedCountry),
        typeof(string), typeof(DetailedRecordContentPage), default(string));

    public string? SelectedCountry
    {
        get => (string?)GetValue(SelectedCountryProperty);
        set => SetValue(SelectedCountryProperty, value);
    }

    public static readonly BindableProperty SelectedCityProperty = BindableProperty.Create(nameof(SelectedCity),
        typeof(string), typeof(DetailedRecordContentPage), default(string));

    public string SelectedCity
    {
        get => (string)GetValue(SelectedCityProperty);
        set => SetValue(SelectedCityProperty, value);
    }

    public static readonly BindableProperty IsPlaceholderVisibleProperty =
        BindableProperty.Create(nameof(IsPlaceholderVisible), typeof(bool), typeof(DetailedRecordContentPage),
            default(bool));

    public bool IsPlaceholderVisible
    {
        get => (bool)GetValue(IsPlaceholderVisibleProperty);
        set => SetValue(IsPlaceholderVisibleProperty, value);
    }

    public static readonly BindableProperty PlaceholderTextPlaceProperty = BindableProperty.Create(
        nameof(PlaceholderTextPlace),
        typeof(string), typeof(DetailedRecordContentPage), default(string));

    public string PlaceholderTextPlace
    {
        get => (string)GetValue(PlaceholderTextPlaceProperty);
        set => SetValue(PlaceholderTextPlaceProperty, value);
    }

    public static readonly BindableProperty PlaceholderTextCityProperty =
        BindableProperty.Create(nameof(PlaceholderTextCity), typeof(string), typeof(DetailedRecordContentPage),
            default(string));

    public string PlaceholderTextCity
    {
        get => (string)GetValue(PlaceholderTextCityProperty);
        set => SetValue(PlaceholderTextCityProperty, value);
    }

    public static readonly BindableProperty PlaceholderTextCountryProperty =
        BindableProperty.Create(nameof(PlaceholderTextCountry), typeof(string), typeof(DetailedRecordContentPage),
            default(object));

    public string PlaceholderTextCountry
    {
        get => (string)GetValue(PlaceholderTextCountryProperty);
        set => SetValue(PlaceholderTextCountryProperty, value);
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

    public static readonly BindableProperty CanBeDeletedProperty = BindableProperty.Create(nameof(CanBeDeleted),
        typeof(bool), typeof(DetailedRecordContentPage), default(bool));

    public bool CanBeDeleted
    {
        get => (bool)GetValue(CanBeDeletedProperty);
        set => SetValue(CanBeDeletedProperty, value);
    }

    public static readonly BindableProperty HistorySymbolProperty = BindableProperty.Create(nameof(HistorySymbol),
        typeof(string), typeof(DetailedRecordContentPage), default(string));

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

    public THistory THistory { get; } = new();

    public EPackIcons CloseCircle { get; } = EPackIcons.CloseCircle;

    private WritableLayer PlaceLayer { get; } = new() { Style = null, IsMapInfoLayer = true, Tag = typeof(TPlace) };
    public List<KnownTileSource> KnownTileSources { get; private init; } = [];
    public KnownTileSource KnownTileSourceSelected { get; set; }

    public ObservableCollection<TModePayment> ModePayments { get; private init; } = [];
    public ObservableCollection<TAccount> Accounts { get; private init; } = [];
    public ObservableCollection<TCategoryType> CategoryTypes { get; private init; } = [];
    public ObservableCollection<string> CountriesCollection { get; private init; } = [];

    public ObservableCollection<string> CitiesCollection { get; private init; } = [];
    public List<TPlace> PlacesCollection { get; private init; } = [];

    private THistory? OriginalHistory { get; set; }

    public ICommand BackCommand { get; set; } = null!;

    private readonly TaskCompletionSource<bool> _taskCompletionSource = new();

    public Task<bool> ResultDialog
        => _taskCompletionSource.Task;

    public bool IsNewHistory { get; set; }

    public DetailedRecordContentPage()
    {
        BackCommand = new Command(OnBackCommandPressed);

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

        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    #region Action

    private void ButtonCancelUpdateHistory_OnClicked(object? sender, EventArgs e)
    {
        var place = OriginalHistory.PlaceFk?.ToISql<TPlace>();
        SelectedCountry = EmptyStringTreeViewConverter.ToUnknown(place?.Country);
        SelectedCity = EmptyStringTreeViewConverter.ToUnknown(place?.City);

        OriginalHistory.CopyPropertiesTo(THistory);
        Refocus();

        UpdateIsDirty();
    }

    private async void ButtonDeleteHistory_OnClicked(object? sender, EventArgs e)
    {
        var response = await DisplayAlert(
            DetailedRecordContentPageResources.MessageBoxDeleteHistoryQuestionTitle,
            DetailedRecordContentPageResources.MessageBoxDeleteHistoryQuestionMessage,
            DetailedRecordContentPageResources.MessageBoxDeleteHistoryQuestionYesButton,
            DetailedRecordContentPageResources.MessageBoxDeleteHistoryQuestionNoButton);
        if (!response) return;

        var json = THistory.ToJson();
        Log.Information("Attempting to delete history : {Json}", json);
        var (success, exception) = THistory.Delete();
        if (!success)
        {
            Log.Error(exception, "An error occur while deleting the record");

            await DisplayAlert(
                DetailedRecordContentPageResources.MessageBoxDeleteHistoryErrorTitle,
                DetailedRecordContentPageResources.MessageBoxDeleteHistoryErrorMessage,
                DetailedRecordContentPageResources.MessageBoxDeleteHistoryErrorOkButton);
            return;
        }

        Log.Information("Record was successfully deleted");
        await DisplayAlert(
            DetailedRecordContentPageResources.MessageBoxDeleteHistorySuccessTitle,
            DetailedRecordContentPageResources.MessageBoxDeleteHistorySuccessMessage,
            DetailedRecordContentPageResources.MessageBoxDeleteHistorySuccessOkButton);

        _taskCompletionSource.SetResult(true);
        await Navigation.PopAsync();
    }

    private void ButtonRefocus_OnClicked(object? sender, EventArgs e)
        => Refocus();

    private async void ButtonUpdateHistory_OnClicked(object? sender, EventArgs e)
    {
        var isValidHistory = await ValidHistory();
        if (!isValidHistory) return;

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
                var isValidHistory = await ValidHistory();
                if (!isValidHistory) return;

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

        ComboBoxSelectorCity.SelectedIndexChanged -= SelectorCity_OnSelectionChanged;
        ComboBoxSelectorPlace.SelectedIndexChanged -= SelectorPlace_OnSelectionChanged;

        CitiesCollection.Clear();
        CitiesCollection.AddRangeAndSort(citiesResults, s => s);

        PlacesCollection.Clear();
        PlacesCollection.AddRange(records.OrderBy(s => s.Name));

        ComboBoxSelectorCity.SelectedIndexChanged += SelectorCity_OnSelectionChanged;
        ComboBoxSelectorPlace.SelectedIndexChanged += SelectorPlace_OnSelectionChanged;
    }

    private void SelectorCity_OnSelectionChanged(object? sender, EventArgs e)
    {
        if (sender is not Picker comboBox) return;
        var city = comboBox.SelectedItem as string;

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
            ComboBoxSelectorCountry.SelectedIndexChanged -= SelectorCountry_OnSelectionChanged;
            SelectedCountry = records.First().Country;
            ComboBoxSelectorCountry.SelectedIndexChanged += SelectorCountry_OnSelectionChanged;
        }

        PlacesCollection.Clear();
        PlacesCollection.AddRange(records.OrderBy(s => s.Name));
    }

    private void SelectorPlace_OnSelectionChanged(object? sender, EventArgs e)
    {
        if (sender is not Picker picker) return;

        IsPlaceholderVisible = picker.SelectedItem is null;

        var place = THistory.PlaceFk?.ToISql<TPlace>();
        UpdateMapPoint(place);

        try
        {
            ComboBoxSelectorCountry.SelectedIndexChanged -= SelectorCountry_OnSelectionChanged;
        }
        catch (NullReferenceException)
        {
            // Pass
            return;
        }

        ComboBoxSelectorCity.SelectedIndexChanged -= SelectorCity_OnSelectionChanged;

        var country = string.IsNullOrEmpty(place?.Country)
            ? EmptyStringTreeViewConverterResources.Unknown
            : place.Country;

        var city = string.IsNullOrEmpty(place?.City)
            ? EmptyStringTreeViewConverterResources.Unknown
            : place.City;

        ComboBoxSelectorCountry.SelectedItem = country;
        ComboBoxSelectorCity.SelectedItem = city;
        THistory.PlaceFk = place?.Id;

        ComboBoxSelectorCountry.SelectedIndexChanged += SelectorCountry_OnSelectionChanged;
        ComboBoxSelectorCity.SelectedIndexChanged += SelectorCity_OnSelectionChanged;

        UpdateIsDirty();
    }

    private void SwitchPointed_OnToggled(object? sender, ToggledEventArgs e)
        => UpdateIsDirty();

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
        var dateTime = (DateTime)THistory.Date!;

        dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, time.Hours, time.Minutes, time.Seconds);
        THistory.Date = dateTime;

        UpdateIsDirty();
    }

    #endregion

    #region Function

    private bool AddOrEditHistory()
    {
        if (OriginalHistory?.IsPointed is true) { if (!THistory.IsPointed) THistory.DatePointed = null; }
        else if (THistory.IsPointed) THistory.DatePointed = DateTime.Now;

        var json = THistory.ToJson();

        Log.Information("Attempting to add edit history : {Json}", json);
        var (success, exception) = THistory.AddOrEdit();

        if (success) Log.Information("Successful history editing");
        else Log.Error(exception, "Failed history editing");

        return success;
    }

    private void Refocus()
    {
        try
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
        catch (NullReferenceException)
        {
            // Pass
        }
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
        ButtonUpdateText = IsNewHistory
            ? DetailedRecordContentPageResources.ButtonAddNewHistoryText
            : DetailedRecordContentPageResources.ButtonUpdateText;
        ButtonCancelUpdateText = DetailedRecordContentPageResources.ButtonCancelUpdateText;
        ButtonCanBeDeletedText = DetailedRecordContentPageResources.ButtonCanBeDeletedText;

        LabelTextAddedOn = DetailedRecordContentPageResources.LabelTextAddedOn;
        PointedOperation = DetailedRecordContentPageResources.PointedOperation;
        LabelTextPointedOn = DetailedRecordContentPageResources.LabelTextPointedOn;
        LabelTextOnTheAccount = DetailedRecordContentPageResources.LabelTextOnTheAccount;

        ButtonRefocusText = DetailedRecordContentPageResources.ButtonRefocusText;

        if (IsDirty) Title = DetailedRecordContentPageResources.TitleIsDirty;

        PlaceholderTextCountry = DetailedRecordContentPageResources.PlaceholderTextCountry;
        PlaceholderTextCity = DetailedRecordContentPageResources.PlaceholderTextCity;
        PlaceholderTextPlace = DetailedRecordContentPageResources.PlaceholderTextPlace;
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

    private async Task<bool> ValidHistory()
    {
        var validationContext = new ValidationContext(THistory, serviceProvider: null, items: null);
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(THistory, validationContext, validationResults, true);

        if (isValid) return isValid;

        var propertyError = validationResults.First();
        var propertyMemberName = propertyError.MemberNames.First();

        var messageErrorKey = propertyMemberName switch
        {
            nameof(THistory.AccountFk) => nameof(DetailedRecordContentPageResources.MessageBoxValidationAccountFkError),
            nameof(THistory.Description) => nameof(DetailedRecordContentPageResources
                .MessageBoxValidationDescriptionError),
            nameof(THistory.CategoryTypeFk) => nameof(DetailedRecordContentPageResources
                .MessageBoxValidationCategoryTypeFkError),
            nameof(THistory.ModePaymentFk) => nameof(DetailedRecordContentPageResources
                .MessageBoxValidationModePaymentFkError),
            nameof(THistory.Value) => nameof(DetailedRecordContentPageResources.MessageBoxValidationValueError),
            nameof(THistory.Date) => nameof(DetailedRecordContentPageResources.MessageBoxValidationDateError),
            nameof(THistory.PlaceFk) => nameof(DetailedRecordContentPageResources.MessageBoxValidationPlaceFkError),
            _ => null
        };

        var localizedErrorMessage = string.IsNullOrEmpty(messageErrorKey)
            ? propertyError.ErrorMessage!
            : DetailedRecordContentPageResources.ResourceManager.GetString(messageErrorKey)!;

        await DisplayAlert(DetailedRecordContentPageResources.MessageBoxValidHistoryErrorTitle, localizedErrorMessage,
            DetailedRecordContentPageResources.MessageBoxValidHistoryErrorOkButton);

        return isValid;
    }

    #endregion

    public void SetHistory(int? historyPk = null, THistory? tHistory = null)
    {
        if (historyPk is null && tHistory is null)
        {
            throw new ArgumentNullException(nameof(historyPk), @"historyPk is null");
        }

        using var context = new DataBaseContext();
        if (tHistory is not null)
        {
            tHistory.CopyPropertiesTo(THistory);
        }
        else
        {
            var history = context.THistories.First(s => s.Id.Equals(historyPk));
            history.CopyPropertiesTo(THistory);
        }

        TimePicker.Time = THistory.Date.ToTimeSpan();

        UpdateHistorySymbol();
        UpdateHexadecimalColorCode();

        OriginalHistory = THistory.DeepCopy();

        var place = PlacesCollection.FirstOrDefault(s => s.Id.Equals(THistory.PlaceFk));
        SelectedCountry = EmptyStringTreeViewConverter.ToUnknown(place?.Country);
        SelectedCity = EmptyStringTreeViewConverter.ToUnknown(place?.City);
        ComboBoxSelectorPlace.SelectedItem = place;
    }

    private void UpdateHistorySymbol()
    {
        string symbol;
        if (THistory.AccountFk is null) symbol = string.Empty;
        else
        {
            using var context = new DataBaseContext();
            var currency = context.TCurrencies.First(s => s.Id.Equals(THistory.AccountFk));
            symbol = currency.Symbol!;
        }

        HistorySymbol = symbol;
    }

    private void UpdateHexadecimalColorCode()
    {
        string hexadecimalColorCode;
        if (THistory.CategoryTypeFk is null) hexadecimalColorCode = "#00000000";
        else
        {
            using var context = new DataBaseContext();
            var category = CategoryTypes.First(s => s.Id.Equals(THistory.CategoryTypeFk));
            var color = context.TColors.First(s => s.Id.Equals(category.ColorFk));
            hexadecimalColorCode = color.HexadecimalColorCode!;
        }

        HexadecimalColorCode = hexadecimalColorCode;
    }
}