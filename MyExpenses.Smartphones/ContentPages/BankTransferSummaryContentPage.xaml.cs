using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Versioning;
using CommunityToolkit.Maui.Extensions;
using MyExpenses.Maui.Utils;
using MyExpenses.Models.AutoMapper;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Maui.CustomPopup;
using MyExpenses.Models.Sql.Bases.Views;
using MyExpenses.Models.Sql.Queries;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.SharedUtils.Resources.Resx.BankTransferSummaryManagement;
using MyExpenses.Smartphones.ContentPages.CustomPopups;
using MyExpenses.Smartphones.PackIcons;
using MyExpenses.Smartphones.UserControls.Images;
using MyExpenses.Sql.Context;
using MyExpenses.Sql.Queries;
using MyExpenses.Utils.Strings;

namespace MyExpenses.Smartphones.ContentPages;

public partial class BankTransferSummaryContentPage
{
    public static readonly BindableProperty LabelTextCategoryFilterProperty =
        BindableProperty.Create(nameof(LabelTextCategoryFilter), typeof(string), typeof(BankTransferSummaryContentPage));

    public string LabelTextCategoryFilter
    {
        get => (string)GetValue(LabelTextCategoryFilterProperty);
        set => SetValue(LabelTextCategoryFilterProperty, value);
    }

    public static readonly BindableProperty LabelTextToAdditionalReasonFilterProperty =
        BindableProperty.Create(nameof(LabelTextToAdditionalReasonFilter), typeof(string),
            typeof(BankTransferSummaryContentPage));

    public string LabelTextToAdditionalReasonFilter
    {
        get => (string)GetValue(LabelTextToAdditionalReasonFilterProperty);
        set => SetValue(LabelTextToAdditionalReasonFilterProperty, value);
    }

    public static readonly BindableProperty LabelTextMainReasonFilterProperty =
        BindableProperty.Create(nameof(LabelTextMainReasonFilter), typeof(string),
            typeof(BankTransferSummaryContentPage));

    public string LabelTextMainReasonFilter
    {
        get => (string)GetValue(LabelTextMainReasonFilterProperty);
        set => SetValue(LabelTextMainReasonFilterProperty, value);
    }

    public static readonly BindableProperty LabelTextValueFilterProperty =
        BindableProperty.Create(nameof(LabelTextValueFilter), typeof(string), typeof(BankTransferSummaryContentPage));

    public string LabelTextValueFilter
    {
        get => (string)GetValue(LabelTextValueFilterProperty);
        set => SetValue(LabelTextValueFilterProperty, value);
    }

    public static readonly BindableProperty LabelTextToAccountFilterProperty =
        BindableProperty.Create(nameof(LabelTextToAccountFilter), typeof(string),
            typeof(BankTransferSummaryContentPage));

    public string LabelTextToAccountFilter
    {
        get => (string)GetValue(LabelTextToAccountFilterProperty);
        set => SetValue(LabelTextToAccountFilterProperty, value);
    }

    public static readonly BindableProperty LabelTextFromAccountFilterProperty =
        BindableProperty.Create(nameof(LabelTextFromAccountFilter), typeof(string),
            typeof(BankTransferSummaryContentPage));

    public string LabelTextFromAccountFilter
    {
        get => (string)GetValue(LabelTextFromAccountFilterProperty);
        set => SetValue(LabelTextFromAccountFilterProperty, value);
    }

    public static readonly BindableProperty LabelTextAdditionalReasonProperty =
        BindableProperty.Create(nameof(LabelTextAdditionalReason), typeof(string),
            typeof(BankTransferSummaryContentPage));

    public string LabelTextAdditionalReason
    {
        get => (string)GetValue(LabelTextAdditionalReasonProperty);
        set => SetValue(LabelTextAdditionalReasonProperty, value);
    }

    public static readonly BindableProperty LabelTextMainReasonProperty =
        BindableProperty.Create(nameof(LabelTextMainReason), typeof(string), typeof(BankTransferSummaryContentPage));

    public string LabelTextMainReason
    {
        get => (string)GetValue(LabelTextMainReasonProperty);
        set => SetValue(LabelTextMainReasonProperty, value);
    }

    public static readonly BindableProperty LabelTextDateProperty = BindableProperty.Create(nameof(LabelTextDate),
        typeof(string), typeof(BankTransferSummaryContentPage));

    public string LabelTextDate
    {
        get => (string)GetValue(LabelTextDateProperty);
        set => SetValue(LabelTextDateProperty, value);
    }

    public static readonly BindableProperty LabelTextValueProperty = BindableProperty.Create(nameof(LabelTextValue),
        typeof(string), typeof(BankTransferSummaryContentPage));

    public string LabelTextValue
    {
        get => (string)GetValue(LabelTextValueProperty);
        set => SetValue(LabelTextValueProperty, value);
    }

    public static readonly BindableProperty ElapsedTimeLoadingDataProperty =
        BindableProperty.Create(nameof(ElapsedTimeLoadingData), typeof(string), typeof(BankTransferSummaryContentPage));

    public string ElapsedTimeLoadingData
    {
        get => (string)GetValue(ElapsedTimeLoadingDataProperty);
        set => SetValue(ElapsedTimeLoadingDataProperty, value);
    }

    public static readonly BindableProperty ElapsedTimeLoadingDataTextProperty =
        BindableProperty.Create(nameof(ElapsedTimeLoadingDataText), typeof(string),
            typeof(BankTransferSummaryContentPage));

    public string ElapsedTimeLoadingDataText
    {
        get => (string)GetValue(ElapsedTimeLoadingDataTextProperty);
        set => SetValue(ElapsedTimeLoadingDataTextProperty, value);
    }

    public static readonly BindableProperty RowTotalCountProperty = BindableProperty.Create(nameof(RowTotalCount),
        // ReSharper disable once HeapView.BoxingAllocation
        typeof(int), typeof(BankTransferSummaryContentPage), 0);

    public int RowTotalCount
    {
        get => (int)GetValue(RowTotalCountProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        set => SetValue(RowTotalCountProperty, value);
    }

    public static readonly BindableProperty RecordFoundOnProperty = BindableProperty.Create(nameof(RecordFoundOn),
        typeof(string), typeof(BankTransferSummaryContentPage));

    public string RecordFoundOn
    {
        get => (string)GetValue(RecordFoundOnProperty);
        set => SetValue(RecordFoundOnProperty, value);
    }

    public static readonly BindableProperty RowTotalFilteredCountProperty =
        // ReSharper disable once HeapView.BoxingAllocation
        BindableProperty.Create(nameof(RowTotalFilteredCount), typeof(int), typeof(BankTransferSummaryContentPage), 0);

    public int RowTotalFilteredCount
    {
        get => (int)GetValue(RowTotalFilteredCountProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        set => SetValue(RowTotalFilteredCountProperty, value);
    }

    public static readonly BindableProperty LabelTextToAccountProperty =
        BindableProperty.Create(nameof(LabelTextToAccount), typeof(string), typeof(BankTransferSummaryContentPage));

    public string LabelTextToAccount
    {
        get => (string)GetValue(LabelTextToAccountProperty);
        set => SetValue(LabelTextToAccountProperty, value);
    }

    public static readonly BindableProperty LabelTextAfterProperty = BindableProperty.Create(nameof(LabelTextAfter),
        typeof(string), typeof(BankTransferSummaryContentPage));

    public string LabelTextAfter
    {
        get => (string)GetValue(LabelTextAfterProperty);
        set => SetValue(LabelTextAfterProperty, value);
    }

    public static readonly BindableProperty LabelTextBeforeProperty = BindableProperty.Create(nameof(LabelTextBefore),
        typeof(string), typeof(BankTransferSummaryContentPage));

    public string LabelTextBefore
    {
        get => (string)GetValue(LabelTextBeforeProperty);
        set => SetValue(LabelTextBeforeProperty, value);
    }

    public static readonly BindableProperty LabelTextBalanceProperty = BindableProperty.Create(nameof(LabelTextBalance),
        typeof(string), typeof(BankTransferSummaryContentPage));

    public string LabelTextBalance
    {
        get => (string)GetValue(LabelTextBalanceProperty);
        set => SetValue(LabelTextBalanceProperty, value);
    }

    public static readonly BindableProperty LabelTextFromAccountProperty =
        BindableProperty.Create(nameof(LabelTextFromAccount), typeof(string), typeof(BankTransferSummaryContentPage));

    public string LabelTextFromAccount
    {
        get => (string)GetValue(LabelTextFromAccountProperty);
        set => SetValue(LabelTextFromAccountProperty, value);
    }

    public static readonly BindableProperty ComboBoxMonthHintAssistProperty =
        BindableProperty.Create(nameof(ComboBoxMonthHintAssist), typeof(string), typeof(BankTransferSummaryContentPage));

    public string ComboBoxMonthHintAssist
    {
        get => (string)GetValue(ComboBoxMonthHintAssistProperty);
        set => SetValue(ComboBoxMonthHintAssistProperty, value);
    }

    public static readonly BindableProperty ComboBoxYearsHintAssistProperty =
        BindableProperty.Create(nameof(ComboBoxYearsHintAssist), typeof(string), typeof(BankTransferSummaryContentPage));

    public string ComboBoxYearsHintAssist
    {
        get => (string)GetValue(ComboBoxYearsHintAssistProperty);
        set => SetValue(ComboBoxYearsHintAssistProperty, value);
    }

    public static readonly BindableProperty SelectedYearProperty = BindableProperty.Create(nameof(SelectedYear),
        typeof(string), typeof(BankTransferSummaryContentPage));

    public string SelectedYear
    {
        get => (string)GetValue(SelectedYearProperty);
        set => SetValue(SelectedYearProperty, value);
    }

    public static readonly BindableProperty SelectedMonthProperty = BindableProperty.Create(nameof(SelectedMonth),
        typeof(string), typeof(BankTransferSummaryContentPage));

    public string SelectedMonth
    {
        get => (string)GetValue(SelectedMonthProperty);
        set => SetValue(SelectedMonthProperty, value);
    }

    public ObservableCollection<string> Years { get; }
    public ObservableCollection<string> Months { get; } = [];

    public ObservableCollection<VBankTransferSummary> BankTransferSummaries { get; } = [];

    private List<EFilter> Filters { get; } = [];
    private List<List<VBankTransferSummary>> OriginalVBankTransferSummary { get; } = [];

    private List<PopupSearch> BankTransferFromAccountsFilters { get; } = [];
    private List<PopupSearch> BankTransferToAccountsFilters { get; } = [];
    private List<PopupSearch> BankTransferValuesFilters { get; } = [];
    private List<PopupSearch> BankTransferMainReasonFilters { get; } = [];
    private List<PopupSearch> BankTransferAdditionalReasonFilters { get; } = [];
    private List<PopupSearch> VCategoryDerivesFilter { get; } = [];

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    // TaskCompletionSource is intentionally allocated here as it is the fundamental mechanism
    // for creating and controlling the completion of the Task exposed by `ResultDialog`.
    // This object is required to manually signal task completion (`SetResult`, `SetException`, etc.)
    // when the operation is resolved, ensuring proper asynchronous flow.
    private readonly TaskCompletionSource<bool> _taskCompletionSource = new();

    public Task<bool> ResultDialog
        => _taskCompletionSource.Task;

    public BankTransferSummaryContentPage()
    {
        UpdateMonthLanguage();

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The creation of a new instance of DataBaseContext within a `using` statement ensures that the database context is properly
        // managed and disposed of after use. This allocation is necessary to interact with the database for the operation being
        // performed. The `using` statement guarantees that resources, such as database connections, are released promptly, minimizing
        // the risk of resource leaks and ensuring optimal performance in scenarios where multiple database operations might occur
        // in parallel or sequentially.
        using var context = new DataBaseContext();
        Years =
        [
            ..context.GetDistinctYearsFromBankTransfer(SortOrder.Descending)
                .Select(s => s.ToString())
        ];

        if (Years.Count.Equals(0))
        {
            Years.Add(DateTime.Now.Year.ToString());
        }

        var now = DateTime.Now;
        SelectedYear = now.Year.ToString();
        SelectedMonth = Months[now.Month - 1];

        UpdateLanguage();
        InitializeComponent();

        // ReSharper disable once HeapView.DelegateAllocation
        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    #region Action

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private void AdditionalReasonSvgPath_OnClicked(object? sender, EventArgs e)
        // ReSharper disable once HeapView.DelegateAllocation
        => _ = RunFilter(sender, FilterAdditionalReason);

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private void AdditionalReasonTapGestureRecognizer_Tapped(object? sender, TappedEventArgs e)
        // ReSharper disable once HeapView.DelegateAllocation
        => _ = RunFilter(sender, FilterAdditionalReason);

    private void ButtonAddMonth_OnClick(object? sender, EventArgs e)
        => _ = HandleButtonAddMonth();

    private void ButtonDateNow_OnClick(object? sender, EventArgs e)
    {
        var now = DateOnly.FromDateTime(DateTime.Now);
        UpdateFilterDate(now);
    }

    private void ButtonRemoveMonth_OnClick(object? sender, EventArgs e)
        => _ = HandleButtonRemoveMonth();

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
    private void CategoryTapGestureRecognizer_Tapped(object? sender, TappedEventArgs e)
        // ReSharper disable once HeapView.DelegateAllocation
        => _ = RunFilter(sender, FilterCategory);

    private void CustomPicker_OnSelectedIndexChanged(object? sender, EventArgs e)
        => RefreshDataGrid();

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private void FromAccountSvgPath_OnClicked(object? sender, EventArgs e)
        // ReSharper disable once HeapView.DelegateAllocation
        => _ = RunFilter(sender, FilterFromAccount);

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private void FromAccountTapGestureRecognizer_Tapped(object? sender, TappedEventArgs e)
        // ReSharper disable once HeapView.DelegateAllocation
        => _ = RunFilter(sender, FilterFromAccount);

    private void Interface_OnLanguageChanged()
    {
        UpdateLanguage();
        UpdateMonthLanguage();
    }

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private void MainReasonSvgPath_OnClicked(object? sender, EventArgs e)
        // ReSharper disable once HeapView.DelegateAllocation
        => _ = RunFilter(sender, FilterMainReason);

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private void MainReasonTapGestureRecognizer_Tapped(object? sender, TappedEventArgs e)
        // ReSharper disable once HeapView.DelegateAllocation
        => _ = RunFilter(sender, FilterMainReason);

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

        BankTransferFromAccountsFilters.Clear();
        BankTransferToAccountsFilters.Clear();
        BankTransferValuesFilters.Clear();
        BankTransferMainReasonFilters.Clear();
        BankTransferAdditionalReasonFilters.Clear();
        VCategoryDerivesFilter.Clear();

        RefreshDataGrid();
    }

    private void TapGestureRecognizer_OnTapped(object? sender, TappedEventArgs e)
        => _ = HandleTapGestureRecognizer(sender);

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private void ToAccountSvgPath_OnClicked(object? sender, EventArgs e)
        // ReSharper disable once HeapView.DelegateAllocation
        => _ = RunFilter(sender, FilterToAccount);

    [SupportedOSPlatform("Android21.0")]
    [SupportedOSPlatform("iOS13.0")]
    [SupportedOSPlatform("MacCatalyst13.0")]
    [SupportedOSPlatform("Windows")]
    private void ToAccountTapGestureRecognizer_Tapped(object? sender, TappedEventArgs e)
        // ReSharper disable once HeapView.DelegateAllocation
        => _ = RunFilter(sender, FilterToAccount);

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
    private void ValueTapGestureRecognizer_OnTapped(object? sender, TappedEventArgs e)
        // ReSharper disable once HeapView.DelegateAllocation
        => _ = RunFilter(sender, FilterValue);

    #endregion

    #region Function

    private async Task FilterAdditionalReason(SvgPath svgPath)
    {
        const EFilter eFilter = EFilter.AdditionalReason;

        IEnumerable<PopupSearch> popupSearches;
        if (Filters.Count is 0)
        {
            popupSearches = BankTransferSummaries
                // ReSharper disable once HeapView.ObjectAllocation.Evident
                // This LINQ expression selects the 'AdditionalReason' property from each item in the BankTransferSummaries collection
                // and creates a new instance of the StringIsChecked class for each entry. This approach is necessary to map the raw data
                // from the summaries into a format suitable for further filtering or manipulation while maintaining separation of concerns
                // and ensuring immutability of the original dataset.
                .Select(s => new PopupSearch { Content = s.AdditionalReason ?? string.Empty });
        }
        else
        {
            var items = Filters.Last() == eFilter
                ? OriginalVBankTransferSummary.Last().AsEnumerable()
                : BankTransferSummaries.AsEnumerable();

            popupSearches = items
                // ReSharper disable once HeapView.ObjectAllocation.Evident
                // This LINQ expression iterates over the 'items' collection and transforms each element into a new StringIsChecked object,
                // initializing it with the 'AdditionalReason' property of the current item. This transformation is essential to prepare the data
                // for filtering or display purposes, encapsulating the string value within a specific structure (StringIsChecked) that
                // includes additional context or state (e.g., a "checked" property for selection).
                .Select(s => new PopupSearch { Content = s.AdditionalReason ?? string.Empty });
        }

        popupSearches = popupSearches.DistinctBy(s => s.Content)
            .OrderBy(s => s.Content);

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        var popupFilter = new PopupFilter(popupSearches, EPopupSearch.AdditionalReason, BankTransferAdditionalReasonFilters);
        await this.ShowPopupAsync(popupFilter);

        FilterManagement(BankTransferAdditionalReasonFilters, popupFilter, eFilter, svgPath);
    }

    private async Task FilterCategory(SvgPath svgPath)
    {
        const EFilter eFilter = EFilter.Category;

        // ReSharper disable once HeapView.ClosureAllocation
        var transferIds = Filters.Count is 0
            ? BankTransferSummaries.Select(s => s.Id)
            : Filters.Last() == eFilter
                ? OriginalVBankTransferSummary.Last().AsEnumerable().Select(s => s.Id)
                : BankTransferSummaries.AsEnumerable().Select(s => s.Id);

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The creation of a new DataBaseContext instance (via `new DataBaseContext()`) is necessary to interact with the database.
        // This context provides the connection to the database and allows querying or updating data.
        // The `using` statement ensures that the context is disposed of properly after its use, freeing up resources like database connections.
        await using var context = new DataBaseContext();
        var categoryTypeFk = context.THistories
            .Where(s => s.BankTransferFk != null)
            .Where(s => transferIds.Contains(s.BankTransferFk!.Value))
            .Select(s => s.CategoryTypeFk)
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

    private async Task FilterFromAccount(SvgPath svgPath)
    {
        const EFilter eFilter = EFilter.FromAccounts;

        // ReSharper disable once HeapView.ClosureAllocation
        var transferIds = Filters.Count is 0
            ? BankTransferSummaries.Select(s => s.Id)
            : Filters.Last() == eFilter
                ? OriginalVBankTransferSummary.Last().AsEnumerable().Select(s => s.Id)
                : BankTransferSummaries.AsEnumerable().Select(s => s.Id);

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The creation of a new DataBaseContext instance (via `new DataBaseContext()`) is necessary to interact with the database.
        // This context provides the connection to the database and allows querying or updating data.
        // The `using` statement ensures that the context is disposed of properly after its use, freeing up resources like database connections.
        await using var context = new DataBaseContext();
        var fromAccountFk = context.TBankTransfers
            .Where(s => transferIds.Contains(s.Id))
            .Select(s => s.FromAccountFk!)
            .Distinct()
            .ToList();

        var popupSearches = context.TAccounts
            .Where(s => fromAccountFk.Contains(s.Id))
            .OrderBy(s => s.Name)
            .Select(s => Mapping.Mapper.Map<PopupSearch>(s))
            .ToList();

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        var popupFilter = new PopupFilter(popupSearches, EPopupSearch.Account, BankTransferFromAccountsFilters);
        await this.ShowPopupAsync(popupFilter);

        FilterManagement(BankTransferFromAccountsFilters, popupFilter, eFilter, svgPath);
    }

    private async Task FilterMainReason(SvgPath svgPath)
    {
        const EFilter eFilter = EFilter.MainReason;

        IEnumerable<PopupSearch> popupSearches;
        if (Filters.Count is 0)
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            // This LINQ expression selects the 'MainReason' property from each item in the BankTransferSummaries collection
            // and creates a new instance of the StringIsChecked class for each entry. This approach is necessary to map the raw data
            // from the summaries into a format suitable for further filtering or manipulation while maintaining separation of concerns
            // and ensuring immutability of the original dataset.
            popupSearches = BankTransferSummaries.Select(s => new PopupSearch { Content = s.MainReason });
        }
        else
        {
            var items = Filters.Last() == eFilter
                ? OriginalVBankTransferSummary.Last().AsEnumerable()
                : BankTransferSummaries.AsEnumerable();

            popupSearches = items
                    // ReSharper disable once HeapView.ObjectAllocation.Evident
                    // This LINQ expression iterates over the 'items' collection and transforms each element into a new StringIsChecked object,
                    // initializing it with the 'MainReason' property of the current item. This transformation is essential to prepare the data
                    // for filtering or display purposes, encapsulating the string value within a specific structure (StringIsChecked) that
                    // includes additional context or state (e.g., a "checked" property for selection).
                .Select(s => new PopupSearch { Content = s.MainReason });
        }

        popupSearches = popupSearches.DistinctBy(s => s.Content)
            .OrderBy(s => s.Content);

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        var popupFilter = new PopupFilter(popupSearches, EPopupSearch.MainReason, BankTransferAdditionalReasonFilters);
        await this.ShowPopupAsync(popupFilter);

        FilterManagement(BankTransferMainReasonFilters, popupFilter, eFilter, svgPath);
    }

    private void FilterManagement(List<PopupSearch> collection, PopupFilter popupFilter, EFilter eFilter,
        SvgPath svgPath)
    {
        if (Filters.Count is 0 || Filters.Last() != eFilter)
        {
            Filters.Add(eFilter);
            OriginalVBankTransferSummary.Add(BankTransferSummaries.ToList());
        }

        var isActive = RefreshFilter(collection, popupFilter, svgPath);

        if (!isActive && Filters.Last() == eFilter)
        {
            var lastIndex = Filters.Count - 1;
            Filters.RemoveAt(lastIndex);

            lastIndex = OriginalVBankTransferSummary.Count - 1;
            OriginalVBankTransferSummary.RemoveAt(lastIndex);
        }
    }

    private async Task FilterToAccount(SvgPath svgPath)
    {
        const EFilter eFilter = EFilter.ToAccounts;

        // ReSharper disable once HeapView.ClosureAllocation
        var transferIds = Filters.Count is 0
            ? BankTransferSummaries.Select(s => s.Id)
            : Filters.Last() == eFilter
                ? OriginalVBankTransferSummary.Last().AsEnumerable().Select(s => s.Id)
                : BankTransferSummaries.AsEnumerable().Select(s => s.Id);

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The creation of a new DataBaseContext instance (via `new DataBaseContext()`) is necessary to interact with the database.
        // This context provides the connection to the database and allows querying or updating data.
        // The `using` statement ensures that the context is disposed of properly after its use, freeing up resources like database connections.
        await using var context = new DataBaseContext();
        var toAccountFk = context.TBankTransfers
            .Where(s => transferIds.Contains(s.Id))
            .Select(s => s.ToAccountFk!)
            .Distinct()
            .ToList();

        var popupSearches = context.TAccounts
            .Where(s => toAccountFk.Contains(s.Id))
            .OrderBy(s => s.Name)
            .Select(s => Mapping.Mapper.Map<PopupSearch>(s))
            .ToList();

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        var popupFilter = new PopupFilter(popupSearches, EPopupSearch.Account, BankTransferToAccountsFilters);
        await this.ShowPopupAsync(popupFilter);

        FilterManagement(BankTransferToAccountsFilters, popupFilter, eFilter, svgPath);
    }

    private async Task FilterValue(SvgPath svgPath)
    {
        const EFilter eFilter = EFilter.Values;

        IEnumerable<PopupSearch> popupSearches;
        if (Filters.Count is 0)
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            popupSearches = BankTransferSummaries.Select(s => new PopupSearch { Value = s.Value });
        }
        else
        {
            var items = Filters.Last() == eFilter
                ? OriginalVBankTransferSummary.Last().AsEnumerable()
                : BankTransferSummaries.AsEnumerable();

            popupSearches = items
                // ReSharper disable once HeapView.ObjectAllocation.Evident
                .Select(s => new PopupSearch { Value = s.Value });
        }

        popupSearches = popupSearches.DistinctBy(s => s.Value)
            .OrderBy(s => s.Value);

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        var popupFilter = new PopupFilter(popupSearches, EPopupSearch.Value, BankTransferValuesFilters);
        await this.ShowPopupAsync(popupFilter);

        FilterManagement(BankTransferValuesFilters, popupFilter, eFilter, svgPath);
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

        await DisplayAlert(BankTransferSummaryManagementResources.MessageBoxAddMonthErrorTitle,
            BankTransferSummaryManagementResources.MessageBoxAddMonthErrorMessage,
            BankTransferSummaryManagementResources.MessageBoxAddMonthErrorOkButton);
    }

    private async Task HandleButtonRemoveMonth()
    {
        var date = GetDateOnlyFilter();
        date = date.AddMonths(-1);

        var result = UpdateFilterDate(date);

        if (result) return;

        await DisplayAlert(BankTransferSummaryManagementResources.MessageBoxRemoveMonthErrorTitle,
            BankTransferSummaryManagementResources.MessageBoxRemoveMonthErrorMessage,
            BankTransferSummaryManagementResources.MessageBoxRemoveMonthErrorOkButton);
    }

    private async Task HandleTapGestureRecognizer(object? sender)
    {
        if (sender is not Border border) return;
        if (border.BindingContext is not VBankTransferSummary vBankTransferSummary) return;

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // An instance of AddEditBankTransferContentPage is created here with the property CanBeDeleted set to true.
        // This setup is necessary to define the initial state of the page, specifying that the bank transfer can
        // be deleted. It ensures proper configuration of the content page, aligning with the expected functionality
        // for adding or editing bank transfer entries.
        var addEditBankTransferContentPage = new AddEditBankTransferContentPage { CanBeDeleted = true };
        addEditBankTransferContentPage.SetVBankTransferSummary(vBankTransferSummary);

        await addEditBankTransferContentPage.NavigateToAsync();

        var success = await addEditBankTransferContentPage.ResultDialog;
        if (!success) return;

        _taskCompletionSource.SetResult(true);
        RefreshDataGrid();
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
        ComboBoxYearsHintAssist = BankTransferSummaryManagementResources.ComboBoxYearsHintAssist;
        ComboBoxMonthHintAssist = BankTransferSummaryManagementResources.ComboBoxMonthHintAssist;

        LabelTextFromAccountFilter = BankTransferSummaryManagementResources.LabelTextFromAccountFilter;
        LabelTextToAccountFilter = BankTransferSummaryManagementResources.LabelTextToAccountFilter;
        LabelTextValueFilter = BankTransferSummaryManagementResources.LabelTextValueFilter;
        LabelTextMainReasonFilter = BankTransferSummaryManagementResources.LabelTextMainReasonFilter;
        LabelTextToAdditionalReasonFilter = BankTransferSummaryManagementResources.LabelTextToAdditionalReasonFilter;
        LabelTextCategoryFilter = BankTransferSummaryManagementResources.LabelTextCategoryFilter;

        LabelTextFromAccount = BankTransferSummaryManagementResources.LabelTextFromAccount;
        LabelTextToAccount = BankTransferSummaryManagementResources.LabelTextToAccount;
        LabelTextBalance = BankTransferSummaryManagementResources.LabelTextBalance;
        LabelTextBefore = BankTransferSummaryManagementResources.LabelTextBefore;
        LabelTextAfter = BankTransferSummaryManagementResources.LabelTextAfter;

        LabelTextValue = BankTransferSummaryManagementResources.LabelTextValue;
        LabelTextDate = BankTransferSummaryManagementResources.LabelTextDate;

        LabelTextMainReason = $"{BankTransferSummaryManagementResources.LabelTextMainReason} ";
        LabelTextAdditionalReason = $"{BankTransferSummaryManagementResources.LabelTextAdditionalReason} ";

        ElapsedTimeLoadingDataText = $"{BankTransferSummaryManagementResources.ElapsedTimeLoadingDataText} ";
        RecordFoundOn = $" {BankTransferSummaryManagementResources.RecordFoundOn} ";
    }

    private void RefreshDataGrid()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // An instance of Stopwatch is created here to measure the elapsed time for a specific operation or
        // process. This is useful for performance monitoring, debugging, or optimizing code execution by
        // tracking the duration of the targeted operation with precision.
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        int? yearInt = null;
        if (!string.IsNullOrEmpty(SelectedYear)) _ = SelectedYear.ToInt(out yearInt);

        int? monthInt = null;
        if (!string.IsNullOrEmpty(SelectedMonth)) monthInt = Months.IndexOf(SelectedMonth) + 1;

        string[]? fromAccounts = null;
        if (BankTransferFromAccountsFilters.Count > 0) fromAccounts = BankTransferFromAccountsFilters.Select(s => s.Content!).ToArray();

        string[]? toAccounts = null;
        if (BankTransferToAccountsFilters.Count > 0) toAccounts = BankTransferToAccountsFilters.Select(s => s.Content!).ToArray();

        double[]? values = null;
        if (BankTransferValuesFilters.Count > 0) values = BankTransferValuesFilters.Select(s => s.Value!.Value).ToArray();

        string[]? mainReasons = null;
        if (BankTransferMainReasonFilters.Count > 0) mainReasons = BankTransferMainReasonFilters.Select(s => s.Content!).ToArray();

        string?[]? additionalReasons = null;
        if (BankTransferAdditionalReasonFilters.Count > 0) additionalReasons = BankTransferAdditionalReasonFilters.Select(s => s.Content).ToArray();

        string[]? categories = null;
        if (VCategoryDerivesFilter.Count > 0) categories = VCategoryDerivesFilter.Select(s => s.Content!).ToArray();

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The creation of a new DataBaseContext instance (via `new DataBaseContext()`) is necessary to interact with the database.
        // This context provides the connection to the database and allows querying or updating data.
        // The `using` statement ensures that the context is disposed of properly after its use, freeing up resources like database connections.
        using var context = new DataBaseContext();
        var result = context.GetFilteredBankTransfers(yearInt, monthInt, fromAccounts, toAccounts, values, mainReasons, additionalReasons, categories);

        RowTotalCount = result.TotalRowCount;
        RowTotalFilteredCount = result.TotalFilteredRowCount;

        stopwatch.Stop();
        ElapsedTimeLoadingData = stopwatch.Elapsed.ToString("hh\\:mm\\:ss");

        BankTransferSummaries.Clear();
        BankTransferSummaries.AddRange(result.BankTransferSummaries);
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

    private enum EFilter
    {
        FromAccounts,
        ToAccounts,
        Values,
        MainReason,
        AdditionalReason,
        Category
    }

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
}