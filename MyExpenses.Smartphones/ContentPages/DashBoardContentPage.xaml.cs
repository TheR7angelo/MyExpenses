using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Versioning;
using System.Windows.Input;
using CommunityToolkit.Maui.Views;
using MyExpenses.Maui.Utils;
using MyExpenses.Models.AutoMapper;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Maui.CustomPopup;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Models.Sql.Bases.Views;
using MyExpenses.Models.Sql.Derivatives.Tables;
using MyExpenses.Models.Sql.Derivatives.Views;
using MyExpenses.Smartphones.ContentPages.CustomPopups;
using MyExpenses.Smartphones.ContentPages.CustomPopups.CustomPopupActivityIndicator;
using MyExpenses.Smartphones.PackIcons;
using MyExpenses.Smartphones.Resources.Resx.ContentPages.DashBoardContentPage;
using MyExpenses.Smartphones.UserControls.Images;
using MyExpenses.Sql.Context;
using MyExpenses.Sql.Queries;
using MyExpenses.Utils;
using MyExpenses.Utils.Collection;
using MyExpenses.Utils.Strings;
using Serilog;

namespace MyExpenses.Smartphones.ContentPages;

public partial class DashBoardContentPage
{
    public static readonly BindableProperty RecordFoundOnProperty = BindableProperty.Create(nameof(RecordFoundOn),
        typeof(string), typeof(DashBoardContentPage));

    public string RecordFoundOn
    {
        get => (string)GetValue(RecordFoundOnProperty);
        set => SetValue(RecordFoundOnProperty, value);
    }

    public static readonly BindableProperty RowTotalCountProperty =
        // ReSharper disable once HeapView.BoxingAllocation
        BindableProperty.Create(nameof(RowTotalCount), typeof(int), typeof(DashBoardContentPage), 0);

    public int RowTotalCount
    {
        get => (int)GetValue(RowTotalCountProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        set => SetValue(RowTotalCountProperty, value);
    }

    public static readonly BindableProperty RowTotalFilteredCountProperty =
        // ReSharper disable once HeapView.BoxingAllocation
        BindableProperty.Create(nameof(RowTotalFilteredCount), typeof(int), typeof(DashBoardContentPage), 0);

    public int RowTotalFilteredCount
    {
        get => (int)GetValue(RowTotalFilteredCountProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        set => SetValue(RowTotalFilteredCountProperty, value);
    }

    private static readonly BindableProperty ElapsedTimeProperty = BindableProperty.Create(
        nameof(ElapsedTimeLoadingData),
        typeof(string), typeof(DashBoardContentPage));

    public string ElapsedTimeLoadingData
    {
        get => (string)GetValue(ElapsedTimeProperty);
        set => SetValue(ElapsedTimeProperty, value);
    }

    public static readonly BindableProperty ElapsedTimeLoadingDataTextProperty =
        BindableProperty.Create(nameof(ElapsedTimeLoadingDataText), typeof(string), typeof(DashBoardContentPage));

    public string ElapsedTimeLoadingDataText
    {
        get => (string)GetValue(ElapsedTimeLoadingDataTextProperty);
        set => SetValue(ElapsedTimeLoadingDataTextProperty, value);
    }

    public static readonly BindableProperty LabelTextPlaceProperty = BindableProperty.Create(nameof(LabelTextPlace),
        typeof(string), typeof(DashBoardContentPage));

    public string LabelTextPlace
    {
        get => (string)GetValue(LabelTextPlaceProperty);
        set => SetValue(LabelTextPlaceProperty, value);
    }

    public static readonly BindableProperty LabelTextCheckedProperty = BindableProperty.Create(nameof(LabelTextChecked),
        typeof(string), typeof(DashBoardContentPage));

    public string LabelTextChecked
    {
        get => (string)GetValue(LabelTextCheckedProperty);
        set => SetValue(LabelTextCheckedProperty, value);
    }

    public static readonly BindableProperty LabelTextValueProperty = BindableProperty.Create(nameof(LabelTextValue),
        typeof(string), typeof(DashBoardContentPage));

    public string LabelTextValue
    {
        get => (string)GetValue(LabelTextValueProperty);
        set => SetValue(LabelTextValueProperty, value);
    }

    public static readonly BindableProperty LabelTextPaymentModeProperty =
        BindableProperty.Create(nameof(LabelTextPaymentMode), typeof(string), typeof(DashBoardContentPage));

    public string LabelTextPaymentMode
    {
        get => (string)GetValue(LabelTextPaymentModeProperty);
        set => SetValue(LabelTextPaymentModeProperty, value);
    }

    public static readonly BindableProperty LabelTextDescriptionProperty =
        BindableProperty.Create(nameof(LabelTextDescription), typeof(string), typeof(DashBoardContentPage));

    public string LabelTextDescription
    {
        get => (string)GetValue(LabelTextDescriptionProperty);
        set => SetValue(LabelTextDescriptionProperty, value);
    }

    public static readonly BindableProperty LabelTextCategoryProperty =
        BindableProperty.Create(nameof(LabelTextCategory), typeof(string), typeof(DashBoardContentPage));

    public string LabelTextCategory
    {
        get => (string)GetValue(LabelTextCategoryProperty);
        set => SetValue(LabelTextCategoryProperty, value);
    }

    public static readonly BindableProperty ComboBoxMonthHintAssistProperty =
        BindableProperty.Create(nameof(ComboBoxMonthHintAssist), typeof(string), typeof(DashBoardContentPage));

    public string ComboBoxMonthHintAssist
    {
        get => (string)GetValue(ComboBoxMonthHintAssistProperty);
        set => SetValue(ComboBoxMonthHintAssistProperty, value);
    }

    public static readonly BindableProperty ComboBoxYearsHintAssistProperty =
        BindableProperty.Create(nameof(ComboBoxYearsHintAssist), typeof(string), typeof(DashBoardContentPage));

    public string ComboBoxYearsHintAssist
    {
        get => (string)GetValue(ComboBoxYearsHintAssistProperty);
        set => SetValue(ComboBoxYearsHintAssistProperty, value);
    }

    public static readonly BindableProperty CurrentVTotalByAccountProperty =
        BindableProperty.Create(nameof(CurrentVTotalByAccount), typeof(VTotalByAccount), typeof(DashBoardContentPage));

    public VTotalByAccount? CurrentVTotalByAccount
    {
        get => (VTotalByAccount)GetValue(CurrentVTotalByAccountProperty);
        set => SetValue(CurrentVTotalByAccountProperty, value);
    }

    public static readonly BindableProperty SelectedYearProperty = BindableProperty.Create(nameof(SelectedYear),
        typeof(string), typeof(DashBoardContentPage));

    public string SelectedYear
    {
        get => (string)GetValue(SelectedYearProperty);
        set => SetValue(SelectedYearProperty, value);
    }

    public static readonly BindableProperty SelectedMonthProperty = BindableProperty.Create(nameof(SelectedMonth),
        typeof(string), typeof(DashBoardContentPage));

    public string SelectedMonth
    {
        get => (string)GetValue(SelectedMonthProperty);
        set => SetValue(SelectedMonthProperty, value);
    }

    public ObservableCollection<string> Years { get; }
    public ObservableCollection<string> Months { get; } = [];

    public ObservableCollection<VHistory> VHistories { get; } = [];
    public ObservableCollection<VTotalByAccount> VTotalByAccounts { get; } = [];

    private static VTotalByAccount? _staticVTotalByAccount;

    private static VTotalByAccount? StaticVTotalByAccount
    {
        get => _staticVTotalByAccount;
        set
        {
            _staticVTotalByAccount = value;
            Instance.CurrentVTotalByAccount = value;
        }
    }

    public static DashBoardContentPage Instance { get; set; } = null!;

    private List<VCategoryDerive> VCategoryDerivesFilter { get; } = [];
    private List<StringIsChecked> HistoryDescriptions { get; } = [];
    private List<TModePaymentDerive> ModePaymentDeriveFilter { get; } = [];
    private List<DoubleIsChecked> HistoryValues { get; } = [];
    private List<BoolIsChecked> HistoryChecked { get; } = [];
    private List<TPlaceDerive> PlaceDeriveFilter { get; } = [];

    private List<EFilter> Filters { get; } = [];
    private List<List<VHistory>> OriginalVHistories { get; } = [];

    public ICommand CollectionViewVHistoryShortPressCommand { get; }
    private bool _isCollectionViewVHistoryLongPressInvoked;

    public ICommand CollectionViewVHistoryLongPressCommand { get; }

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    public DashBoardContentPage()
    {
        // ReSharper disable HeapView.ObjectAllocation.Evident
        // The Command object is explicitly created here to handle the user's interaction with the UI.
        // This allocation is necessary because `Command` encapsulates the behavior (in this case, `OnBackCommandPressed`)
        // and binds it to the associated UI element, such as a Button or a gesture.
        // This ensures proper separation between the UI and logic layers.
        CollectionViewVHistoryShortPressCommand = new Command(CollectionViewVHistory_OnShortPress);
        CollectionViewVHistoryLongPressCommand = new Command(CollectionViewVHistory_OnLongPress);
        // ReSharper restore HeapView.ObjectAllocation.Evident

        Instance = this;

        UpdateMonthLanguage();

        var (currentYear, currentMonth, _) = DateTime.Now;
        using var context = new DataBaseContext();
        Years =
        [
            ..context
                .THistories
                .Where(s => s.Date.HasValue)
                .Select(s => s.Date!.Value.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .Select(y => y.ToString())
        ];

        if (Years.Count.Equals(0)) Years.Add(currentYear.ToString());
        var lastYear = int.Parse(Years.Max()!);
        for (var year = lastYear + 1; year <= currentYear; year++)
        {
            Years.Insert(0, year.ToString());
        }

        SelectedYear = currentYear.ToString();
        SelectedMonth = Months[currentMonth - 1];

        CurrentVTotalByAccount = context.VTotalByAccounts.FirstOrDefault();

        RefreshAccountTotal();

        UpdateLanguage();
        InitializeComponent();

        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    #region Action

    private void ButtonAddMonth_OnClick(object? sender, EventArgs e)
        => _ = HandleButtonAddMonth();

    private void ButtonDateNow_OnClick(object? sender, EventArgs e)
    {
        var now = DateOnly.FromDateTime(DateTime.Now);
        UpdateFilterDate(now);
    }

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private void ButtonImageViewAddRecordHistory_OnClicked(object? sender, EventArgs e)
        => _ = HandleButtonImageViewAddRecordHistory();

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private async Task HandleButtonImageViewAddRecordHistory()
    {
        var detailedRecordContentPage = new DetailedRecordContentPage { IsNewHistory = true };
        await Navigation.PushAsync(detailedRecordContentPage);

        var result = await detailedRecordContentPage.ResultDialog;
        if (result is not true) return;

        RefreshDataGrid();
        RefreshAccountTotal();
    }

    private void ButtonRemoveMonth_OnClick(object? sender, EventArgs e)
        => _ = HandleButtonRemoveMonth();

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private void CategoryTapGestureRecognizer_OnTapped(object? sender, TappedEventArgs e)
        => _ = RunFilter(sender, FilterCategory);

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private void CategorySvgPath_OnClicked(object? sender, EventArgs e)
        => _ = RunFilter(sender, FilterCategory);

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private void CollectionViewVHistory_OnLongPress(object obj)
        => _ = HandleCollectionViewVHistoryLongPress(obj);

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private void CollectionViewVHistory_OnShortPress(object obj)
        => _ = HandleCollectionViewVHistoryShortPress(obj);

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private void CollectionViewVTotalAccount_OnLoaded(object? sender, EventArgs e)
    {
        _ = Dispatcher.DispatchAsync(async () =>
        {
            await Task.Delay(TimeSpan.FromMilliseconds(100));
            RefreshRadioButtonSelected();

            CustomPopupActivityIndicatorHelper.CloseCustomPopupActivityIndicator();
        });
    }

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private void CustomPicker_OnSelectedIndexChanged(object? sender, EventArgs e)
        => RefreshDataGrid();

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private void DescriptionTapGestureRecognizer_OnTapped(object? sender, TappedEventArgs e)
        => _ = RunFilter(sender, FilterDescription);

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private void DescriptionSvgPath_OnClicked(object? sender, EventArgs e)
        => _ = RunFilter(sender, FilterDescription);

    private void Interface_OnLanguageChanged()
    {
        UpdateLanguage();
        UpdateMonthLanguage();
    }

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private void PaymentModeTapGestureRecognizer_OnTapped(object? sender, TappedEventArgs e)
        => _ = RunFilter(sender, FilterPaymentMode);

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private void PaymentModeSvgPath_OnClicked(object? sender, EventArgs e)
        => _ = RunFilter(sender, FilterPaymentMode);

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private void PlaceTapGestureRecognizer_OnTapped(object? sender, TappedEventArgs e)
        => _ = RunFilter(sender, FilterPlace);

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private void PlaceSvgPath_OnClicked(object? sender, EventArgs e)
        => _ = RunFilter(sender, FilterPlace);

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private void PointedTapGestureRecognizer_OnTapped(object? sender, TappedEventArgs e)
        => _ = RunFilter(sender, FilterChecked);

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private void PointedSvgPath_OnClicked(object? sender, EventArgs e)
        => _ = RunFilter(sender, FilterChecked);

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private void SvgPathRefresh_OnClicked(object? sender, EventArgs e)
    {
        if (sender is not SvgPath svgPath) return;
        if (svgPath.Parent is not HorizontalStackLayout horizontalStackLayout) return;

        var horizontalStackLayoutChildren = horizontalStackLayout.FindVisualChildren<HorizontalStackLayout>();
        foreach (var horizontalStackLayoutChild in horizontalStackLayoutChildren)
        {
            var svgPathChild = horizontalStackLayoutChild.FindVisualChildren<SvgPath>().FirstOrDefault();
            if (svgPathChild is null) continue;

            svgPathChild.GeometrySource = EPackIcons.Filter;
        }

        VCategoryDerivesFilter.Clear();
        HistoryDescriptions.Clear();
        ModePaymentDeriveFilter.Clear();
        HistoryValues.Clear();
        HistoryChecked.Clear();
        PlaceDeriveFilter.Clear();

        RefreshDataGrid();
    }

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private void ValueTapGestureRecognizer_OnTapped(object? sender, TappedEventArgs e)
        => _ = RunFilter(sender, FilterValue);

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private void ValueSvgPath_OnClicked(object? sender, EventArgs e)
        => _ = RunFilter(sender, FilterValue);

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private void RadioButton_OnCheckedChanged(object? sender, CheckedChangedEventArgs e)
    {
        var button = sender as RadioButton;
        if (button?.BindingContext is not VTotalByAccount vTotalByAccount) return;

        StaticVTotalByAccount = vTotalByAccount;

        var name = vTotalByAccount.Name;
        if (string.IsNullOrEmpty(name)) return;

        RefreshDataGrid(name);
    }

    #endregion

    #region Function

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private async Task FilterCategory(SvgPath svgPath)
    {
        const EFilter eFilter = EFilter.Category;

        IEnumerable<int> historyIds;
        if (Filters.Count is 0) historyIds = VHistories.Select(s => s.Id);
        else
        {
            var items = Filters.Last() == eFilter
                ? OriginalVHistories.Last().AsEnumerable()
                : VHistories.AsEnumerable();

            historyIds = items.Select(s => s.Id);
        }

        var mapper = Mapping.Mapper;
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The creation of a new DataBaseContext instance (via `new DataBaseContext()`) is necessary to interact with the database.
        // This context provides the connection to the database and allows querying or updating data.
        // The `using` statement ensures that the context is disposed of properly after its use, freeing up resources like database connections.
        await using var context = new DataBaseContext();
        var categoryTypeFk = context.THistories
            .Where(s => historyIds.Contains(s.Id))
            .Select(s => s.CategoryTypeFk!)
            .Distinct()
            .ToList();

        var vCategoryDerives = context.VCategories
            .Where(s => categoryTypeFk.Contains(s.Id))
            .OrderBy(s => s.CategoryName)
            .Select(s => mapper.Map<VCategoryDerive>(s))
            .ToList();

        var customPopupFilterCategories = new CustomPopupFilterCategories(vCategoryDerives, VCategoryDerivesFilter);
        await this.ShowPopupAsync(customPopupFilterCategories);

        FilterManagement(VCategoryDerivesFilter, customPopupFilterCategories, eFilter, svgPath);
    }

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private async Task FilterChecked(SvgPath svgPath)
    {
        const EFilter eFilter = EFilter.Checked;

        IEnumerable<BoolIsChecked> isCheckeds;
        if (Filters.Count is 0) isCheckeds = VHistories.Select(s => new BoolIsChecked { BoolValue = (bool)s.IsPointed! });
        else
        {
            var items = Filters.Last() == eFilter
                ? OriginalVHistories.Last().AsEnumerable()
                : VHistories.AsEnumerable();

            isCheckeds = items.Select(s => new BoolIsChecked { BoolValue = (bool)s.IsPointed! });
        }

        isCheckeds = isCheckeds.Distinct();
        isCheckeds = isCheckeds.OrderBy(s => s.BoolValue);

        var customPopupFilterChecked = new CustomPopupFilterChecked(isCheckeds, HistoryChecked);
        await this.ShowPopupAsync(customPopupFilterChecked);

        FilterManagement(HistoryChecked, customPopupFilterChecked, eFilter, svgPath);
    }

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private async Task FilterDescription(SvgPath svgPath)
    {
        const EFilter eFilter = EFilter.Description;

        IEnumerable<StringIsChecked> historyDescription;
        if (Filters.Count is 0) historyDescription = VHistories.Select(s => new StringIsChecked { StringValue = s.Description });
        else
        {
            var items = Filters.Last() == eFilter
                ? OriginalVHistories.Last().AsEnumerable()
                : VHistories.AsEnumerable();

            historyDescription = items.Select(s => new StringIsChecked { StringValue = s.Description });
        }

        historyDescription = historyDescription.Distinct();
        historyDescription = historyDescription.OrderBy(s => s.StringValue);

        var customPopupFilterDescription = new CustomPopupFilterDescriptions(historyDescription, HistoryDescriptions);
        await this.ShowPopupAsync(customPopupFilterDescription);

        FilterManagement(HistoryDescriptions, customPopupFilterDescription, eFilter, svgPath);
    }

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private void FilterManagement<T>(List<T> collection, ICustomPopupFilter<T> customPopupFilter, EFilter eFilter,
        SvgPath svgPath)
    {
        if (Filters.Count is 0 || Filters.Last() != eFilter)
        {
            Filters.Add(eFilter);
            OriginalVHistories.Add(VHistories.ToList());
        }

        var isActive = RefreshFilter(collection, customPopupFilter, svgPath);

        if (!isActive && Filters.Last() == eFilter)
        {
            var lastIndex = Filters.Count - 1;
            Filters.RemoveAt(lastIndex);

            lastIndex = OriginalVHistories.Count - 1;
            OriginalVHistories.RemoveAt(lastIndex);
        }
    }

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private async Task FilterPaymentMode(SvgPath svgPath)
    {
        const EFilter eFilter = EFilter.PaymentMode;

        IEnumerable<int> historyIds;
        if (Filters.Count is 0) historyIds = VHistories.Select(s => s.Id);
        else
        {
            var items = Filters.Last() == eFilter
                ? OriginalVHistories.Last().AsEnumerable()
                : VHistories.AsEnumerable();

            historyIds = items.Select(s => s.Id);
        }

        var mapper = Mapping.Mapper;
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The creation of a new DataBaseContext instance (via `new DataBaseContext()`) is necessary to interact with the database.
        // This context provides the connection to the database and allows querying or updating data.
        // The `using` statement ensures that the context is disposed of properly after its use, freeing up resources like database connections.
        await using var context = new DataBaseContext();
        var modePaymentFk = context.THistories
            .Where(s => historyIds.Contains(s.Id))
            .Select(s => s.ModePaymentFk!)
            .Distinct()
            .ToList();

        var tModePaymentDerives = context.TModePayments
            .Where(s => modePaymentFk.Contains(s.Id))
            .OrderBy(s => s.Name)
            .Select(s => mapper.Map<TModePaymentDerive>(s))
            .ToList();

        var customPopupFilterModePayment =
            new CustomPopupFilterModePayments(tModePaymentDerives, ModePaymentDeriveFilter);
        await this.ShowPopupAsync(customPopupFilterModePayment);

        FilterManagement(ModePaymentDeriveFilter, customPopupFilterModePayment, eFilter, svgPath);
    }

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private async Task FilterPlace(SvgPath svgPath)
    {
        const EFilter eFilter = EFilter.Place;

        IEnumerable<int> historyIds;
        if (Filters.Count is 0) historyIds = VHistories.Select(s => s.Id);
        else
        {
            var items = Filters.Last() == eFilter
                ? OriginalVHistories.Last().AsEnumerable()
                : VHistories.AsEnumerable();

            historyIds = items.Select(s => s.Id);
        }

        var mapper = Mapping.Mapper;
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The creation of a new DataBaseContext instance (via `new DataBaseContext()`) is necessary to interact with the database.
        // This context provides the connection to the database and allows querying or updating data.
        // The `using` statement ensures that the context is disposed of properly after its use, freeing up resources like database connections.
        await using var context = new DataBaseContext();
        var placeFk = context.THistories
            .Where(s => historyIds.Contains(s.Id))
            .Select(s => s.PlaceFk!)
            .Distinct()
            .ToList();

        var placeDerives = context.TPlaces
            .Where(s => placeFk.Contains(s.Id))
            .OrderBy(s => s.Name)
            .Select(s => mapper.Map<TPlaceDerive>(s))
            .ToList();

        var customPopupFilterPlaces = new CustomPopupFilterPlaces(placeDerives, PlaceDeriveFilter);
        await this.ShowPopupAsync(customPopupFilterPlaces);

        FilterManagement(PlaceDeriveFilter, customPopupFilterPlaces, eFilter, svgPath);
    }

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private async Task FilterValue(SvgPath svgPath)
    {
        const EFilter eFilter = EFilter.Value;

        IEnumerable<DoubleIsChecked> historyValues;
        if (Filters.Count is 0) historyValues = VHistories.Select(s => new DoubleIsChecked { DoubleValue = s.Value });
        else
        {
            var items = Filters.Last() == eFilter
                ? OriginalVHistories.Last().AsEnumerable()
                : VHistories.AsEnumerable();

            historyValues = items.Select(s => new DoubleIsChecked { DoubleValue = s.Value });
        }

        historyValues = historyValues.Distinct();
        historyValues = historyValues.OrderBy(s => s.DoubleValue);

        var customPopupFilterDoubleValues = new CustomPopupFilterDoubleValues(historyValues, HistoryValues);
        await this.ShowPopupAsync(customPopupFilterDoubleValues);

        FilterManagement(HistoryValues, customPopupFilterDoubleValues, eFilter, svgPath);
    }

    private DateOnly GetDateOnlyFilter()
    {
        var monthIndex = string.IsNullOrEmpty(SelectedMonth)
            ? DateTime.Now.Month
            : Months.IndexOf(SelectedMonth) + 1;

        var year = string.IsNullOrEmpty(SelectedYear)
            ? DateTime.Now.Year
            : int.Parse(SelectedYear);

        var date = DateOnly.Parse($"{year}/{monthIndex}/01");
        return date;
    }

    private async Task HandleButtonAddMonth()
    {
        var date = GetDateOnlyFilter();
        date = date.AddMonths(1);

        var result = UpdateFilterDate(date);

        if (result) return;

        await DisplayAlert(DashBoardContentPageResources.MessageBoxAddMonthErrorTitle,
            DashBoardContentPageResources.MessageBoxAddMonthErrorMessage,
            DashBoardContentPageResources.MessageBoxAddMonthErrorOkButton);
    }

    private async Task HandleButtonRemoveMonth()
    {
        var date = GetDateOnlyFilter();
        date = date.AddMonths(-1);

        var result = UpdateFilterDate(date);

        if (result) return;

        await DisplayAlert(DashBoardContentPageResources.MessageBoxRemoveMonthErrorTitle,
            DashBoardContentPageResources.MessageBoxRemoveMonthErrorMessage,
            DashBoardContentPageResources.MessageBoxRemoveMonthErrorOkButton);
    }

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private async Task HandleCollectionViewVHistoryLongPress(object obj)
    {
        if (obj is not VHistory vHistory) return;

        _isCollectionViewVHistoryLongPressInvoked = true;

        var history = vHistory.Id.ToISql<THistory>()!;

        var word = history.IsPointed
            ? DashBoardContentPageResources.MessageBoxCollectionViewVHistoryOnLongPressUnCheck
            : DashBoardContentPageResources.MessageBoxCollectionViewVHistoryOnLongPressCheck;

        var message = string.Format(DashBoardContentPageResources.MessageBoxCollectionViewVHistoryOnLongPressMessage,
            word, $"\n{history.Description}");
        var response = await DisplayAlert(
            DashBoardContentPageResources.MessageBoxCollectionViewVHistoryOnLongPressTitle, message,
            DashBoardContentPageResources.MessageBoxCollectionViewVHistoryOnLongPressYesButton,
            DashBoardContentPageResources.MessageBoxCollectionViewVHistoryOnLongPressNoButton);
        if (response)
        {
            history.IsPointed = !history.IsPointed;

            if (history.IsPointed) history.DatePointed = DateTime.Now;
            else history.DatePointed = null;

            Log.Information("Attention to pointed record, id: \"{HistoryId}\"", history.Id);
            history.AddOrEdit();
            Log.Information("The recording was successfully pointed");

            RefreshDataGrid();
        }

        await Task.Delay(TimeSpan.FromSeconds(1));
        _isCollectionViewVHistoryLongPressInvoked = false;
    }

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private async Task HandleCollectionViewVHistoryShortPress(object obj)
    {
        if (_isCollectionViewVHistoryLongPressInvoked) return;

        if (obj is not VHistory vHistory) return;

        var detailedRecordContentPage = new DetailedRecordContentPage { CanBeDeleted = true };
        detailedRecordContentPage.SetHistory(vHistory.Id);
        await Navigation.PushAsync(detailedRecordContentPage);

        var result = await detailedRecordContentPage.ResultDialog;
        if (result is not true) return;

        RefreshDataGrid();
        RefreshAccountTotal();
    }

    internal void RefreshAccountTotal()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The creation of a new DataBaseContext instance (via `new DataBaseContext()`) is necessary to interact with the database.
        // This context provides the connection to the database and allows querying or updating data.
        // The `using` statement ensures that the context is disposed of properly after its use, freeing up resources like database connections.
        using var context = new DataBaseContext();
        var newVTotalByAccounts = context.VTotalByAccounts.ToList();

        var itemsToDelete = VTotalByAccounts
            .Where(s => newVTotalByAccounts.All(n => n.Id != s.Id)).ToImmutableArray();

        foreach (var item in itemsToDelete)
        {
            VTotalByAccounts.Remove(item);
        }

        foreach (var vTotalByAccount in newVTotalByAccounts)
        {
            var exist = VTotalByAccounts.FirstOrDefault(s => s.Id == vTotalByAccount.Id);
            if (exist is not null)
            {
                vTotalByAccount.CopyPropertiesTo(exist);
            }
            else
            {
                VTotalByAccounts.AddAndSort(vTotalByAccount, s => s.Name!);
            }
        }
    }

    private void RefreshAccountTotal(int id)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The creation of a new DataBaseContext instance (via `new DataBaseContext()`) is necessary to interact with the database.
        // This context provides the connection to the database and allows querying or updating data.
        // The `using` statement ensures that the context is disposed of properly after its use, freeing up resources like database connections.
        using var context = new DataBaseContext();
        var newVTotalByAccount = context.VTotalByAccounts.FirstOrDefault(s => s.Id.Equals(id));
        if (newVTotalByAccount is null) return;

        var vTotalByAccount = VTotalByAccounts.FirstOrDefault(s => s.Id.Equals(id));
        if (vTotalByAccount is null) return;

        newVTotalByAccount.CopyPropertiesTo(vTotalByAccount);
    }

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private void RefreshDataGrid(string? accountName = null)
    {
        if (string.IsNullOrEmpty(accountName))
        {
            var radioButtons = CollectionViewVTotalAccount?.FindVisualChildren<RadioButton>().ToList() ?? [];
            if (radioButtons.Count.Equals(0)) return;

            accountName = radioButtons.FirstOrDefault(s => s.IsChecked)?.Content as string;
        }

        if (string.IsNullOrEmpty(accountName)) return;

        var rowTotalCount = FilterAndSortHistoryRecords(accountName, out var records, out var elapsedTimeLoadingData);

        RowTotalCount = rowTotalCount;
        RowTotalFilteredCount = records.Count;
        ElapsedTimeLoadingData = elapsedTimeLoadingData.ToString("hh\\:mm\\:ss");

        VHistories.Clear();
        VHistories.AddRange(records);
    }

    private int FilterAndSortHistoryRecords(string accountName, out List<VHistory> records,
        out TimeSpan elapsedTimeLoadingData)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The creation of a new DataBaseContext instance (via `new DataBaseContext()`) is necessary to interact with the database.
        // This context provides the connection to the database and allows querying or updating data.
        // The `using` statement ensures that the context is disposed of properly after its use, freeing up resources like database connections.
        using var context = new DataBaseContext();

        int? monthInt = null;
        if (!string.IsNullOrEmpty(SelectedMonth))
        {
            monthInt = Months.IndexOf(SelectedMonth) + 1;
        }

        int? yearInt = null;
        if (!string.IsNullOrEmpty(SelectedYear))
        {
            _ = SelectedYear.ToInt(out yearInt);
        }

        string[]? categoryNames = null;
        if (VCategoryDerivesFilter.Count > 0)
        {
            categoryNames = VCategoryDerivesFilter.Select(s => s.CategoryName!).ToArray();
        }

        string?[]? descriptions = null;
        if (HistoryDescriptions.Count > 0)
        {
            descriptions = HistoryDescriptions.Select(s => s.StringValue).ToArray();
        }

        string[]? modePayments = null;
        if (ModePaymentDeriveFilter.Count > 0)
        {
            modePayments = ModePaymentDeriveFilter.Select(s => s.Name!).ToArray();
        }

        string[]? places = null;
        if (PlaceDeriveFilter.Count > 0)
        {
            places = PlaceDeriveFilter.Select(s => s.Name!).ToArray();
        }

        double[]? values = null;
        if (HistoryValues.Count > 0)
        {
            values = HistoryValues.Select(s => s.DoubleValue!.Value).ToArray();
        }

        bool[]? pointeds = null;
        if (HistoryChecked.Count > 0)
        {
            pointeds = HistoryChecked.Select(s => s.BoolValue).ToArray();
        }

        var results = accountName.GetFilteredHistories(monthInt, yearInt, categoryNames, descriptions, modePayments, places, values, pointeds);
        records = results.Histories.ToList();

        stopwatch.Stop();
        elapsedTimeLoadingData = stopwatch.Elapsed;
        return results.TotalRowCount;
    }

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private bool RefreshFilter<T>(List<T> collection, ICustomPopupFilter<T> customPopupFilter, SvgPath svgPath)
    {
        collection.Clear();
        collection.AddRange(customPopupFilter.GetFilteredItemChecked());

        var itemCheckedCount = customPopupFilter.GetFilteredItemCheckedCount();
        var itemCount = customPopupFilter.GetFilteredItemCount();

        var icon = itemCheckedCount is 0 || itemCheckedCount.Equals(itemCount)
            ? EPackIcons.Filter
            : EPackIcons.FilterCheck;

        svgPath.GeometrySource = icon;

        RefreshDataGrid();

        return icon is EPackIcons.FilterCheck;
    }

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private void RefreshRadioButtonSelected()
    {
        var radioButtons = CollectionViewVTotalAccount.FindVisualChildren<RadioButton>().ToList();

        var radioButton = StaticVTotalByAccount is null
            ? radioButtons.FirstOrDefault()
            : radioButtons.FirstOrDefault(rb =>
                rb.BindingContext is VTotalByAccount vTotalByAccount &&
                vTotalByAccount.Id.Equals(StaticVTotalByAccount.Id));

        StaticVTotalByAccount = radioButton?.BindingContext as VTotalByAccount;

        if (radioButton is null) return;
        radioButton.IsChecked = true;

        RefreshDataGrid();
        RefreshAccountTotal(StaticVTotalByAccount!.Id);
    }

    private bool UpdateFilterDate(DateOnly date)
    {
        var yearStr = date.Year.ToString();
        if (!Years.Contains(yearStr)) return false;

        if (!yearStr.Equals(SelectedYear)) SelectedYear = yearStr;

        var monthIndex = date.Month - 1;
        SelectedMonth = Months[monthIndex];

        return true;
    }

    private void UpdateLanguage()
    {
        ComboBoxYearsHintAssist = DashBoardContentPageResources.ComboBoxYearsHintAssist;
        ComboBoxMonthHintAssist = DashBoardContentPageResources.ComboBoxMonthHintAssist;

        LabelTextCategory = DashBoardContentPageResources.LabelTextCategory;
        LabelTextDescription = DashBoardContentPageResources.LabelTextDescription;
        LabelTextPaymentMode = DashBoardContentPageResources.LabelTextPaymentMode;
        LabelTextValue = DashBoardContentPageResources.LabelTextValue;
        LabelTextChecked = DashBoardContentPageResources.LabelTextChecked;
        LabelTextPlace = DashBoardContentPageResources.LabelTextPlace;

        ElapsedTimeLoadingDataText = $"{DashBoardContentPageResources.ElapsedTimeLoadingDataText} ";
        RecordFoundOn = $" {DashBoardContentPageResources.RecordFoundOn} ";
    }

    private void UpdateMonthLanguage()
    {
        var currentCulture = CultureInfo.CurrentCulture;

        var months = currentCulture.DateTimeFormat.MonthNames
            .Where(s => !string.IsNullOrEmpty(s))
            .Select(s => s.ToFirstCharUpper()).ToList();

        if (Months.Count is 0)
        {
            Months.AddRange(months);
        }
        else
        {
            var selectedMonth = Months.FirstOrDefault(month => month.Equals(SelectedMonth)) ?? string.Empty;
            for (var i = 0; i < months.Count; i++)
            {
                Months[i] = months[i];
            }

            SelectedMonth = selectedMonth;
        }
    }

    #endregion

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private static async Task RunFilter(object? sender, Func<SvgPath, Task> func)
    {
        var svgPath = FindSvgPath(sender);
        if (svgPath is null) return;

        await func.Invoke(svgPath);
    }

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private static SvgPath? FindSvgPath(object? sender)
    {
        return sender switch
        {
            null => null,
            SvgPath svgPath => svgPath,
            _ => sender is HorizontalStackLayout horizontalStackLayout
                ? horizontalStackLayout.FindVisualChildren<SvgPath>().FirstOrDefault()
                : null
        };
    }

    private enum EFilter
    {
        Category,
        Description,
        PaymentMode,
        Value,
        Checked,
        Place
    }
}