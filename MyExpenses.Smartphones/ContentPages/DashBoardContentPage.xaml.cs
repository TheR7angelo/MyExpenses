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
using MyExpenses.Models.Sql.Queries;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.SharedUtils.Properties;
using MyExpenses.Smartphones.ContentPages.CustomPopups;
using MyExpenses.Smartphones.ContentPages.CustomPopups.CustomPopupActivityIndicator;
using MyExpenses.Smartphones.PackIcons;
using MyExpenses.Smartphones.Resources.Resx.ContentPages.DashBoardContentPage;
using MyExpenses.Smartphones.UserControls.Images;
using MyExpenses.Sql.Context;
using MyExpenses.Sql.Queries;
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

    private List<PopupSearch> VCategoryDerivesFilter { get; } = [];
    private List<PopupSearch> HistoryDescriptions { get; } = [];
    private List<PopupSearch> ModePaymentDeriveFilter { get; } = [];
    private List<PopupSearch> HistoryValues { get; } = [];
    private List<PopupSearch> HistoryChecked { get; } = [];
    private List<PopupSearch> PlaceDeriveFilter { get; } = [];

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
        // ReSharper disable HeapView.DelegateAllocation
        // The Command object is explicitly created here to handle the user's interaction with the UI.
        // This allocation is necessary because `Command` encapsulates the behavior (in this case, `OnBackCommandPressed`)
        // and binds it to the associated UI element, such as a Button or a gesture.
        // This ensures proper separation between the UI and logic layers.
        CollectionViewVHistoryShortPressCommand = new Command(CollectionViewVHistory_OnShortPress);
        CollectionViewVHistoryLongPressCommand = new Command(CollectionViewVHistory_OnLongPress);
        // ReSharper restore HeapView.DelegateAllocation
        // ReSharper restore HeapView.ObjectAllocation.Evident

        Instance = this;

        UpdateMonthLanguage();

        var (currentYear, currentMonth, _) = DateTime.Now;

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The use of `using var context = new DataBaseContext();` is essential here because
        // we need to interact with the database to retrieve the required data.
        // The DataBaseContext instance provides access to execute SQL queries or LINQ
        // operations on the database tables and views. Using the `using` statement ensures
        // proper disposal of resources (like database connections) once the context is no
        // longer needed, optimizing resource management and preventing potential memory leaks.
        using var context = new DataBaseContext();
        Years =
        [
            ..context.GetDistinctYearsFromHistories(SortOrder.Descending)
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

        // ReSharper disable once HeapView.DelegateAllocation
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
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The instantiation of `DetailedRecordContentPage` with `IsNewHistory = true` is necessary
        // to create a new page instance specifically configured for adding a new history record.
        // This allows passing initial parameters or configurations to the page, ensuring that
        // it behaves appropriately for the intended use case. The explicit allocation ensures
        // the page is initialized correctly in an isolated and controlled way.
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
        // ReSharper disable once HeapView.DelegateAllocation
        => _ = RunFilter(sender, FilterCategory);

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private void CategorySvgPath_OnClicked(object? sender, EventArgs e)
        // ReSharper disable once HeapView.DelegateAllocation
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
        // ReSharper disable once HeapView.DelegateAllocation
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
        // ReSharper disable once HeapView.DelegateAllocation
        => _ = RunFilter(sender, FilterDescription);

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private void DescriptionSvgPath_OnClicked(object? sender, EventArgs e)
        // ReSharper disable once HeapView.DelegateAllocation
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
        // ReSharper disable once HeapView.DelegateAllocation
        => _ = RunFilter(sender, FilterPaymentMode);

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private void PaymentModeSvgPath_OnClicked(object? sender, EventArgs e)
        // ReSharper disable once HeapView.DelegateAllocation
        => _ = RunFilter(sender, FilterPaymentMode);

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private void PlaceTapGestureRecognizer_OnTapped(object? sender, TappedEventArgs e)
        // ReSharper disable once HeapView.DelegateAllocation
        => _ = RunFilter(sender, FilterPlace);

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private void PlaceSvgPath_OnClicked(object? sender, EventArgs e)
        // ReSharper disable once HeapView.DelegateAllocation
        => _ = RunFilter(sender, FilterPlace);

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private void PointedTapGestureRecognizer_OnTapped(object? sender, TappedEventArgs e)
        // ReSharper disable once HeapView.DelegateAllocation
        => _ = RunFilter(sender, FilterChecked);

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private void PointedSvgPath_OnClicked(object? sender, EventArgs e)
        // ReSharper disable once HeapView.DelegateAllocation
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
        // ReSharper disable once HeapView.DelegateAllocation
        => _ = RunFilter(sender, FilterValue);

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private void ValueSvgPath_OnClicked(object? sender, EventArgs e)
        // ReSharper disable once HeapView.DelegateAllocation
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

        var popupSearches = context.VCategories
            .Where(s => categoryTypeFk.Contains(s.Id))
            .OrderBy(s => s.CategoryName)
            .Select(s => Mapping.Mapper.Map<PopupSearch>(s))
            .ToList();

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        var popupFilter = new PopupFilter(popupSearches, EPopupSearch.Category, VCategoryDerivesFilter);
        await this.ShowPopupAsync(popupFilter);

        FilterManagement(VCategoryDerivesFilter, popupFilter, eFilter, svgPath);
    }

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private async Task FilterChecked(SvgPath svgPath)
    {
        const EFilter eFilter = EFilter.Checked;

        IEnumerable<PopupSearch> popupSearch;
        if (Filters.Count is 0)
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            popupSearch = VHistories.Select(s => new PopupSearch { BValue = s.IsPointed });
        }
        else
        {
            var items = Filters.Last() == eFilter
                ? OriginalVHistories.Last().AsEnumerable()
                : VHistories.AsEnumerable();

            // ReSharper disable once HeapView.ObjectAllocation.Evident
            popupSearch = items.Select(s => new PopupSearch { BValue = s.IsPointed });
        }

        popupSearch = popupSearch.DistinctBy(s => s.BValue)
            .OrderBy(s => s.BValue);

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        var popupFilter = new PopupFilter(popupSearch, EPopupSearch.MainReason, HistoryChecked);
        await this.ShowPopupAsync(popupFilter);

        FilterManagement(HistoryChecked, popupFilter, eFilter, svgPath);
    }

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private async Task FilterDescription(SvgPath svgPath)
    {
        const EFilter eFilter = EFilter.Description;

        IEnumerable<PopupSearch> popupSearches;
        if (Filters.Count is 0)
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            // The transformation of `VHistories` into a collection of `StringIsChecked` objects is necessary
            // to encapsulate each history's `Description` in a standardized format (`StringIsChecked`).
            // This facilitates consistent handling, processing, and display of descriptions within the context
            // of the application's workflow or UI components.
            popupSearches = VHistories.Select(s => new PopupSearch { Content = s.Description });
        }
        else
        {
            var items = Filters.Last() == eFilter
                ? OriginalVHistories.Last().AsEnumerable()
                : VHistories.AsEnumerable();

            // ReSharper disable once HeapView.ObjectAllocation.Evident
            // The mapping of `items` into `StringIsChecked` objects is essential to wrap each `Description`
            // within a structured format (`StringIsChecked`). This ensures uniform processing and simplifies
            // integration with filtering logic or UI components that rely on this standardized representation.
            popupSearches = items.Select(s => new PopupSearch { Content = s.Description });
        }

        popupSearches = popupSearches.DistinctBy(s => s.Content)
            .OrderBy(s => s.Content);

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        var popupFilter = new PopupFilter(popupSearches, EPopupSearch.MainReason, HistoryDescriptions);
        await this.ShowPopupAsync(popupFilter);

        FilterManagement(HistoryDescriptions, popupFilter, eFilter, svgPath);
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

    private void FilterManagement(List<PopupSearch> collection, PopupFilter popupFilter, EFilter eFilter,
        SvgPath svgPath)
    {
        if (Filters.Count is 0 || Filters.Last() != eFilter)
        {
            Filters.Add(eFilter);
            OriginalVHistories.Add(VHistories.ToList());
        }

        var isActive = RefreshFilter(collection, popupFilter, svgPath);

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

        var popupSearches = context.TModePayments
            .Where(s => modePaymentFk.Contains(s.Id))
            .OrderBy(s => s.Name)
            .Select(s => Mapping.Mapper.Map<PopupSearch>(s))
            .ToList().AsEnumerable();

        popupSearches = popupSearches.DistinctBy(s => s.Content)
            .OrderBy(s => s.Content);

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        var popupFilter = new PopupFilter(popupSearches, EPopupSearch.ModePayment, ModePaymentDeriveFilter);
        await this.ShowPopupAsync(popupFilter);

        FilterManagement(ModePaymentDeriveFilter, popupFilter, eFilter, svgPath);
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

        var popupSearches = context.TPlaces
            .Where(s => placeFk.Contains(s.Id))
            .OrderBy(s => s.Name)
            .Select(s => Mapping.Mapper.Map<PopupSearch>(s))
            .ToList().AsEnumerable();

        popupSearches = popupSearches.DistinctBy(s => s.Content)
            .OrderBy(s => s.Content);

        // // ReSharper disable once HeapView.ObjectAllocation.Evident
        // // The instantiation of `CustomPopupFilterPlaces` is vital for configuring a filter popup
        // // that processes the `placeDerives` collection and applies the `PlaceDeriveFilter` logic.
        // // This ensures effective filtering and management of place-related data within the application.
        // var customPopupFilterPlaces = new CustomPopupFilterPlaces(placeDerives, PlaceDeriveFilter);
        // await this.ShowPopupAsync(customPopupFilterPlaces);
        //
        // FilterManagement(PlaceDeriveFilter, customPopupFilterPlaces, eFilter, svgPath);

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        var popupFilter = new PopupFilter(popupSearches, EPopupSearch.ModePayment, PlaceDeriveFilter);
        await this.ShowPopupAsync(popupFilter);

        FilterManagement(PlaceDeriveFilter, popupFilter, eFilter, svgPath);
    }

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private async Task FilterValue(SvgPath svgPath)
    {
        const EFilter eFilter = EFilter.Value;

        IEnumerable<PopupSearch> popupSearches;
        if (Filters.Count is 0)
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            // Transforming `VHistories` into `DoubleIsChecked` objects is essential for encapsulating each `Value`
            // within a structured format (`DoubleIsChecked`). This provides consistency and simplifies processing
            // or filtering of historical numeric data across the application's components.
            popupSearches = VHistories.Select(s => new PopupSearch { Value = s.Value });
        }
        else
        {
            var items = Filters.Last() == eFilter
                ? OriginalVHistories.Last().AsEnumerable()
                : VHistories.AsEnumerable();

            // ReSharper disable once HeapView.ObjectAllocation.Evident
            // Transforming `VHistories` into `DoubleIsChecked` objects is essential for encapsulating each `Value`
            // within a structured format (`DoubleIsChecked`). This provides consistency and simplifies processing
            // or filtering of historical numeric data across the application's components.
            popupSearches = items.Select(s => new PopupSearch { Value = s.Value });
        }

        popupSearches = popupSearches.DistinctBy(s => s.Value)
            .OrderBy(s => s.Value);

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // // The creation of `CustomPopupFilterDoubleValues` is key to setting up a filter popup
        // // capable of managing `historyValues` and applying the `HistoryValues` filtering logic.
        // // This ensures efficient handling and filtering of numeric data within the application.
        // var customPopupFilterDoubleValues = new CustomPopupFilterDoubleValues(historyValues, HistoryValues);
        // await this.ShowPopupAsync(customPopupFilterDoubleValues);
        //
        // FilterManagement(HistoryValues, customPopupFilterDoubleValues, eFilter, svgPath);

        popupSearches = popupSearches.DistinctBy(s => s.Value)
            .OrderBy(s => s.Value);

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        var popupFilter = new PopupFilter(popupSearches, EPopupSearch.Value, HistoryValues);
        await this.ShowPopupAsync(popupFilter);

        FilterManagement(HistoryValues, popupFilter, eFilter, svgPath);
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

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The initialization of `DetailedRecordContentPage` with `CanBeDeleted` set to `true`
        // allows for creating a detailed record page where records can be marked as deletable.
        // This enables flexibility in managing record actions within the application's user interface.
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

        // ReSharper disable HeapView.DelegateAllocation
        var itemsToDelete = VTotalByAccounts
            .Where(s => newVTotalByAccounts.All(n => n.Id != s.Id)).ToImmutableArray();
        // ReSharper restore HeapView.DelegateAllocation

        foreach (var item in itemsToDelete)
        {
            VTotalByAccounts.Remove(item);
        }

        foreach (var vTotalByAccount in newVTotalByAccounts)
        {
            // ReSharper disable once HeapView.DelegateAllocation
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

        // ReSharper disable once HeapView.DelegateAllocation
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
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The initialization of a `Stopwatch` instance is crucial for accurately measuring
        // the duration of operations or processes. This is useful for performance tracking
        // and optimization within the application.
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
            categoryNames = VCategoryDerivesFilter.Select(s => s.Content!).ToArray();
        }

        string?[]? descriptions = null;
        if (HistoryDescriptions.Count > 0)
        {
            descriptions = HistoryDescriptions.Select(s => s.Content).ToArray();
        }

        string[]? modePayments = null;
        if (ModePaymentDeriveFilter.Count > 0)
        {
            modePayments = ModePaymentDeriveFilter.Select(s => s.Content!).ToArray();
        }

        string[]? places = null;
        if (PlaceDeriveFilter.Count > 0)
        {
            places = PlaceDeriveFilter.Select(s => s.Content!).ToArray();
        }

        double[]? values = null;
        if (HistoryValues.Count > 0)
        {
            values = HistoryValues.Select(s => s.Value!.Value).ToArray();
        }

        bool[]? pointeds = null;
        if (HistoryChecked.Count > 0)
        {
            pointeds = HistoryChecked.Select(s => s.BValue!.Value).ToArray();
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

    private bool RefreshFilter(List<PopupSearch> collection, PopupFilter popupFilter, SvgPath svgPath)
    {
        collection.Clear();
        collection.AddRange(popupFilter.GetFilteredItemChecked());

        var itemCheckedCount = popupFilter.GetFilteredItemCheckedCount();
        var itemCount = popupFilter.GetFilteredItemCount();

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
            // ReSharper disable once HeapView.DelegateAllocation
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