using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using CommunityToolkit.Maui.Views;
using MyExpenses.Maui.Utils;
using MyExpenses.Models.AutoMapper;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Maui.CustomPopupFilter;
using MyExpenses.Models.Sql.Bases.Views;
using MyExpenses.Models.Sql.Derivatives.Tables;
using MyExpenses.Smartphones.ContentPages.CustomPopups;
using MyExpenses.Smartphones.PackIcons;
using MyExpenses.Smartphones.Resources.Resx.ContentPages.BankTransferSummaryContentPage;
using MyExpenses.Smartphones.UserControls.Images;
using MyExpenses.Sql.Context;
using MyExpenses.Utils.Collection;
using MyExpenses.Utils.Strings;

namespace MyExpenses.Smartphones.ContentPages;

public partial class BankTransferSummaryContentPage
{
    public static readonly BindableProperty LabelTextValueFilterProperty =
        BindableProperty.Create(nameof(LabelTextValueFilter), typeof(string), typeof(BankTransferSummaryContentPage),
            default(string));

    public string LabelTextValueFilter
    {
        get => (string)GetValue(LabelTextValueFilterProperty);
        set => SetValue(LabelTextValueFilterProperty, value);
    }

    public static readonly BindableProperty LabelTextToAccountFilterProperty =
        BindableProperty.Create(nameof(LabelTextToAccountFilter), typeof(string),
            typeof(BankTransferSummaryContentPage), default(string));

    public string LabelTextToAccountFilter
    {
        get => (string)GetValue(LabelTextToAccountFilterProperty);
        set => SetValue(LabelTextToAccountFilterProperty, value);
    }

    public static readonly BindableProperty LabelTextFromAccountFilterProperty =
        BindableProperty.Create(nameof(LabelTextFromAccountFilter), typeof(string),
            typeof(BankTransferSummaryContentPage), default(string));

    public string LabelTextFromAccountFilter
    {
        get => (string)GetValue(LabelTextFromAccountFilterProperty);
        set => SetValue(LabelTextFromAccountFilterProperty, value);
    }

    public static readonly BindableProperty LabelTextAdditionalReasonProperty =
        BindableProperty.Create(nameof(LabelTextAdditionalReason), typeof(string),
            typeof(BankTransferSummaryContentPage), default(string));

    public string LabelTextAdditionalReason
    {
        get => (string)GetValue(LabelTextAdditionalReasonProperty);
        set => SetValue(LabelTextAdditionalReasonProperty, value);
    }

    public static readonly BindableProperty LabelTextMainReasonProperty =
        BindableProperty.Create(nameof(LabelTextMainReason), typeof(string), typeof(BankTransferSummaryContentPage),
            default(string));

    public string LabelTextMainReason
    {
        get => (string)GetValue(LabelTextMainReasonProperty);
        set => SetValue(LabelTextMainReasonProperty, value);
    }

    public static readonly BindableProperty LabelTextDateProperty = BindableProperty.Create(nameof(LabelTextDate),
        typeof(string), typeof(BankTransferSummaryContentPage), default(string));

    public string LabelTextDate
    {
        get => (string)GetValue(LabelTextDateProperty);
        set => SetValue(LabelTextDateProperty, value);
    }

    public static readonly BindableProperty LabelTextValueProperty = BindableProperty.Create(nameof(LabelTextValue),
        typeof(string), typeof(BankTransferSummaryContentPage), default(string));

    public string LabelTextValue
    {
        get => (string)GetValue(LabelTextValueProperty);
        set => SetValue(LabelTextValueProperty, value);
    }

    public static readonly BindableProperty ElapsedTimeLoadingDataProperty =
        BindableProperty.Create(nameof(ElapsedTimeLoadingData), typeof(string), typeof(BankTransferSummaryContentPage),
            default(string));

    public string ElapsedTimeLoadingData
    {
        get => (string)GetValue(ElapsedTimeLoadingDataProperty);
        set => SetValue(ElapsedTimeLoadingDataProperty, value);
    }

    public static readonly BindableProperty ElapsedTimeLoadingDataTextProperty =
        BindableProperty.Create(nameof(ElapsedTimeLoadingDataText), typeof(string),
            typeof(BankTransferSummaryContentPage), default(string));

    public string ElapsedTimeLoadingDataText
    {
        get => (string)GetValue(ElapsedTimeLoadingDataTextProperty);
        set => SetValue(ElapsedTimeLoadingDataTextProperty, value);
    }

    public static readonly BindableProperty RowTotalCountProperty = BindableProperty.Create(nameof(RowTotalCount),
        typeof(int), typeof(BankTransferSummaryContentPage), default(int));

    public int RowTotalCount
    {
        get => (int)GetValue(RowTotalCountProperty);
        set => SetValue(RowTotalCountProperty, value);
    }

    public static readonly BindableProperty RecordFoundOnProperty = BindableProperty.Create(nameof(RecordFoundOn),
        typeof(string), typeof(BankTransferSummaryContentPage), default(string));

    public string RecordFoundOn
    {
        get => (string)GetValue(RecordFoundOnProperty);
        set => SetValue(RecordFoundOnProperty, value);
    }

    public static readonly BindableProperty RowTotalFilteredCountProperty =
        BindableProperty.Create(nameof(RowTotalFilteredCount), typeof(int), typeof(BankTransferSummaryContentPage),
            default(int));

    public int RowTotalFilteredCount
    {
        get => (int)GetValue(RowTotalFilteredCountProperty);
        set => SetValue(RowTotalFilteredCountProperty, value);
    }

    public static readonly BindableProperty LabelTextToAccountProperty =
        BindableProperty.Create(nameof(LabelTextToAccount), typeof(string), typeof(BankTransferSummaryContentPage),
            default(string));

    public string LabelTextToAccount
    {
        get => (string)GetValue(LabelTextToAccountProperty);
        set => SetValue(LabelTextToAccountProperty, value);
    }

    public static readonly BindableProperty LabelTextAfterProperty = BindableProperty.Create(nameof(LabelTextAfter),
        typeof(string), typeof(BankTransferSummaryContentPage), default(string));

    public string LabelTextAfter
    {
        get => (string)GetValue(LabelTextAfterProperty);
        set => SetValue(LabelTextAfterProperty, value);
    }

    public static readonly BindableProperty LabelTextBeforeProperty = BindableProperty.Create(nameof(LabelTextBefore),
        typeof(string), typeof(BankTransferSummaryContentPage), default(string));

    public string LabelTextBefore
    {
        get => (string)GetValue(LabelTextBeforeProperty);
        set => SetValue(LabelTextBeforeProperty, value);
    }

    public static readonly BindableProperty LabelTextBalanceProperty = BindableProperty.Create(nameof(LabelTextBalance),
        typeof(string), typeof(BankTransferSummaryContentPage), default(string));

    public string LabelTextBalance
    {
        get => (string)GetValue(LabelTextBalanceProperty);
        set => SetValue(LabelTextBalanceProperty, value);
    }

    public static readonly BindableProperty LabelTextFromAccountProperty =
        BindableProperty.Create(nameof(LabelTextFromAccount), typeof(string), typeof(BankTransferSummaryContentPage),
            default(string));

    public string LabelTextFromAccount
    {
        get => (string)GetValue(LabelTextFromAccountProperty);
        set => SetValue(LabelTextFromAccountProperty, value);
    }

    public static readonly BindableProperty ComboBoxMonthHintAssistProperty =
        BindableProperty.Create(nameof(ComboBoxMonthHintAssist), typeof(string), typeof(BankTransferSummaryContentPage),
            default(string));

    public string ComboBoxMonthHintAssist
    {
        get => (string)GetValue(ComboBoxMonthHintAssistProperty);
        set => SetValue(ComboBoxMonthHintAssistProperty, value);
    }

    public static readonly BindableProperty ComboBoxYearsHintAssistProperty =
        BindableProperty.Create(nameof(ComboBoxYearsHintAssist), typeof(string), typeof(BankTransferSummaryContentPage),
            default(string));

    public string ComboBoxYearsHintAssist
    {
        get => (string)GetValue(ComboBoxYearsHintAssistProperty);
        set => SetValue(ComboBoxYearsHintAssistProperty, value);
    }

    public static readonly BindableProperty SelectedYearProperty = BindableProperty.Create(nameof(SelectedYear),
        typeof(string), typeof(BankTransferSummaryContentPage), default(string));

    public string SelectedYear
    {
        get => (string)GetValue(SelectedYearProperty);
        set => SetValue(SelectedYearProperty, value);
    }

    public static readonly BindableProperty SelectedMonthProperty = BindableProperty.Create(nameof(SelectedMonth),
        typeof(string), typeof(BankTransferSummaryContentPage), default(string));

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

    private List<TAccountDerive> BankTransferFromAccountsFilters { get; } = [];
    private List<TAccountDerive> BankTransferToAccountsFilters { get; } = [];
    private List<DoubleIsChecked> BankTransferValuesFilters { get; } = [];

    public BankTransferSummaryContentPage()
    {
        UpdateMonthLanguage();

        using var context = new DataBaseContext();
        Years =
        [
            ..context
                .TBankTransfers
                .Where(s => s.Date.HasValue)
                .Select(s => s.Date!.Value.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .Select(y => y.ToString())
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

        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    #region Action

    private async void ButtonAddMonth_OnClick(object? sender, EventArgs e)
    {
        var date = GetDateOnlyFilter();
        date = date.AddMonths(1);

        var result = UpdateFilterDate(date);

        if (result) return;

        await DisplayAlert(BankTransferSummaryContentPageResources.MessageBoxAddMonthErrorTitle,
            BankTransferSummaryContentPageResources.MessageBoxAddMonthErrorMessage,
            BankTransferSummaryContentPageResources.MessageBoxAddMonthErrorOkButton);
    }

    private void ButtonDateNow_OnClick(object? sender, EventArgs e)
    {
        var now = DateOnly.FromDateTime(DateTime.Now);
        UpdateFilterDate(now);
    }

    private async void ButtonRemoveMonth_OnClick(object? sender, EventArgs e)
    {
        var date = GetDateOnlyFilter();
        date = date.AddMonths(-1);

        var result = UpdateFilterDate(date);

        if (result) return;

        await DisplayAlert(BankTransferSummaryContentPageResources.MessageBoxRemoveMonthErrorTitle,
            BankTransferSummaryContentPageResources.MessageBoxRemoveMonthErrorMessage,
            BankTransferSummaryContentPageResources.MessageBoxRemoveMonthErrorOkButton);
    }

    private void CustomPicker_OnSelectedIndexChanged(object? sender, EventArgs e)
        => RefreshDataGrid();

    private async void FromAccountSvgPath_OnClicked(object? sender, EventArgs e)
    {
        var svgPath = FindSvgPath(sender);
        if (svgPath is null) return;

        await FilterFromAccount(svgPath);
    }

    private async void FromAccountTapGestureRecognizer_Tapped(object? sender, TappedEventArgs e)
    {
        var svgPath = FindSvgPath(sender);
        if (svgPath is null) return;

        await FilterFromAccount(svgPath);
    }

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
    {
        UpdateLanguage();
        UpdateMonthLanguage();
    }

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

        RefreshDataGrid();
    }

    private async void ToAccountSvgPath_OnClicked(object? sender, EventArgs e)
    {
        var svgPath = FindSvgPath(sender);
        if (svgPath is null) return;

        await FilterToAccount(svgPath);
    }

    private async void ToAccountTapGestureRecognizer_Tapped(object? sender, TappedEventArgs e)
    {
        var svgPath = FindSvgPath(sender);
        if (svgPath is null) return;

        await FilterToAccount(svgPath);
    }

    private async void ValueSvgPath_OnClicked(object? sender, EventArgs e)
    {
        var svgPath = FindSvgPath(sender);
        if (svgPath is null) return;

        await FilterValue(svgPath);
    }

    private async void ValueTapGestureRecognizer_OnTapped(object? sender, TappedEventArgs e)
    {
        var svgPath = FindSvgPath(sender);
        if (svgPath is null) return;

        await FilterValue(svgPath);
    }

    #endregion

    #region Function

    private async Task FilterFromAccount(SvgPath svgPath)
    {
        const EFilter eFilter = EFilter.FromAccounts;

        IEnumerable<int> transferIds;
        if (Filters.Count is 0) transferIds = BankTransferSummaries.Select(s => s.Id);
        else
        {
            var items = Filters.Last() == eFilter
                ? OriginalVBankTransferSummary.Last().AsEnumerable()
                : BankTransferSummaries.AsEnumerable();

            transferIds = items.Select(s => s.Id);
        }

        var mapper = Mapping.Mapper;
        await using var context = new DataBaseContext();
        var fromAccountFk = context.TBankTransfers
            .Where(s => transferIds.Contains(s.Id))
            .Select(s => s.FromAccountFk!)
            .Distinct()
            .ToList();

        var accountDerives = context.TAccounts
            .Where(s => fromAccountFk.Contains(s.Id))
            .OrderBy(s => s.Name)
            .Select(s => mapper.Map<TAccountDerive>(s))
            .ToList();

        var customPopupFilterAccount = new CustomPopupFilterAccount(accountDerives, BankTransferFromAccountsFilters);
        await this.ShowPopupAsync(customPopupFilterAccount);

        FilterManagement(BankTransferFromAccountsFilters, customPopupFilterAccount, eFilter, svgPath);
    }

    private async Task FilterToAccount(SvgPath svgPath)
    {
        const EFilter eFilter = EFilter.ToAccounts;

        IEnumerable<int> transferIds;
        if (Filters.Count is 0) transferIds = BankTransferSummaries.Select(s => s.Id);
        else
        {
            var items = Filters.Last() == eFilter
                ? OriginalVBankTransferSummary.Last().AsEnumerable()
                : BankTransferSummaries.AsEnumerable();

            transferIds = items.Select(s => s.Id);
        }

        var mapper = Mapping.Mapper;
        await using var context = new DataBaseContext();
        var toAccountFk = context.TBankTransfers
            .Where(s => transferIds.Contains(s.Id))
            .Select(s => s.ToAccountFk!)
            .Distinct()
            .ToList();

        var accountDerives = context.TAccounts
            .Where(s => toAccountFk.Contains(s.Id))
            .OrderBy(s => s.Name)
            .Select(s => mapper.Map<TAccountDerive>(s))
            .ToList();

        var customPopupFilterAccount = new CustomPopupFilterAccount(accountDerives, BankTransferToAccountsFilters);
        await this.ShowPopupAsync(customPopupFilterAccount);

        FilterManagement(BankTransferToAccountsFilters, customPopupFilterAccount, eFilter, svgPath);
    }

    private void FilterManagement<T>(List<T> collection, ICustomPopupFilter<T> customPopupFilter, EFilter eFilter,
        SvgPath svgPath)
    {
        if (Filters.Count is 0 || Filters.Last() != eFilter)
        {
            Filters.Add(eFilter);
            OriginalVBankTransferSummary.Add(BankTransferSummaries.ToList());
        }

        var isActive = RefreshFilter(collection, customPopupFilter, svgPath);

        if (!isActive && Filters.Last() == eFilter)
        {
            var lastIndex = Filters.Count - 1;
            Filters.RemoveAt(lastIndex);

            lastIndex = OriginalVBankTransferSummary.Count - 1;
            OriginalVBankTransferSummary.RemoveAt(lastIndex);
        }
    }

    private async Task FilterValue(SvgPath svgPath)
    {
        const EFilter eFilter = EFilter.Values;

        IEnumerable<DoubleIsChecked> values;
        if (Filters.Count is 0)
            values = BankTransferSummaries.Select(s => new DoubleIsChecked { DoubleValue = s.Value });
        else
        {
            var items = Filters.Last() == eFilter
                ? OriginalVBankTransferSummary.Last().AsEnumerable()
                : BankTransferSummaries.AsEnumerable();

            values = items.Select(s => new DoubleIsChecked { DoubleValue = s.Value });
        }

        values = values.Distinct();
        values = values.OrderBy(s => s.DoubleValue);

        var customPopupFilterHistoryValues = new CustomPopupFilterDoubleValues(values, BankTransferValuesFilters);
        await this.ShowPopupAsync(customPopupFilterHistoryValues);

        FilterManagement(BankTransferValuesFilters, customPopupFilterHistoryValues, eFilter, svgPath);
    }

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
        ComboBoxYearsHintAssist = BankTransferSummaryContentPageResources.ComboBoxYearsHintAssist;
        ComboBoxMonthHintAssist = BankTransferSummaryContentPageResources.ComboBoxMonthHintAssist;

        LabelTextFromAccountFilter = BankTransferSummaryContentPageResources.LabelTextFromAccountFilter;
        LabelTextToAccountFilter = BankTransferSummaryContentPageResources.LabelTextToAccountFilter;
        LabelTextValueFilter = BankTransferSummaryContentPageResources.LabelTextValueFilter;

        LabelTextFromAccount = BankTransferSummaryContentPageResources.LabelTextFromAccount;
        LabelTextToAccount = BankTransferSummaryContentPageResources.LabelTextToAccount;
        LabelTextBalance = BankTransferSummaryContentPageResources.LabelTextBalance;
        LabelTextBefore = BankTransferSummaryContentPageResources.LabelTextBefore;
        LabelTextAfter = BankTransferSummaryContentPageResources.LabelTextAfter;

        LabelTextValue = BankTransferSummaryContentPageResources.LabelTextValue;
        LabelTextDate = BankTransferSummaryContentPageResources.LabelTextDate;

        LabelTextMainReason = $"{BankTransferSummaryContentPageResources.LabelTextMainReason} ";
        LabelTextAdditionalReason = $"{BankTransferSummaryContentPageResources.LabelTextAdditionalReason} ";

        ElapsedTimeLoadingDataText = $"{BankTransferSummaryContentPageResources.ElapsedTimeLoadingDataText} ";
        RecordFoundOn = $" {BankTransferSummaryContentPageResources.RecordFoundOn} ";
    }

    private void RefreshDataGrid()
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        using var context = new DataBaseContext();

        IQueryable<VBankTransferSummary>? query = context.VBankTransferSummaries;
        if (query.Count() is 0) return;

        if (!string.IsNullOrEmpty(SelectedMonth))
        {
            var monthInt = Months.IndexOf(SelectedMonth) + 1;
            query = query.Where(s => s.Date!.Value.Month.Equals(monthInt));
        }

        if (!string.IsNullOrEmpty(SelectedYear))
        {
            _ = SelectedYear.ToInt(out var yearInt);
            query = query.Where(s => s.Date!.Value.Year.Equals(yearInt));
        }

        RowTotalCount = query.Count();

        if (BankTransferFromAccountsFilters.Count > 0)
        {
            var accountNames = BankTransferFromAccountsFilters.Select(s => s.Name!);
            query = query.Where(s => accountNames.Contains(s.FromAccountName));
        }

        if (BankTransferToAccountsFilters.Count > 0)
        {
            var accountNames = BankTransferToAccountsFilters.Select(s => s.Name);
            query = query.Where(s => accountNames.Contains(s.ToAccountName));
        }

        if (BankTransferValuesFilters.Count > 0)
        {
            var values = BankTransferValuesFilters.Select(s => s.DoubleValue);
            query = query.Where(s => values.Contains(s.Value));
        }

        RowTotalFilteredCount = query.Count();

        var records = query
            .OrderByDescending(s => s.Date);

        stopwatch.Stop();
        ElapsedTimeLoadingData = stopwatch.Elapsed.ToString("hh\\:mm\\:ss");

        BankTransferSummaries.Clear();
        BankTransferSummaries.AddRange(records);
    }

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

    private void UpdateMonthLanguage()
    {
        var currentCulture = CultureInfo.CurrentCulture;

        var months = currentCulture.DateTimeFormat.MonthNames
            .Where(s => !string.IsNullOrEmpty(s))
            .Select(s => s.ToFirstCharUpper()).ToImmutableArray();

        if (Months.Count is 0)
        {
            Months.AddRange(months);
        }
        else
        {
            var selectedMonth = Months.FirstOrDefault(month => month.Equals(SelectedMonth)) ?? string.Empty;
            for (var i = 0; i < months.Length; i++)
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
        Values
    }
}