using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Views;
using MyExpenses.Smartphones.Resources.Resx.ContentPages.BankTransferSummaryContentPage;
using MyExpenses.Sql.Context;
using MyExpenses.Utils.Collection;
using MyExpenses.Utils.Strings;

namespace MyExpenses.Smartphones.ContentPages;

public partial class BankTransferSummaryContentPage
{
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

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
    {
        UpdateLanguage();
        UpdateMonthLanguage();
    }

    private void UpdateLanguage()
    {
        ComboBoxYearsHintAssist = BankTransferSummaryContentPageResources.ComboBoxYearsHintAssist;
        ComboBoxMonthHintAssist = BankTransferSummaryContentPageResources.ComboBoxMonthHintAssist;

        LabelTextFromAccount = BankTransferSummaryContentPageResources.LabelTextFromAccount;
        LabelTextToAccount = BankTransferSummaryContentPageResources.LabelTextToAccount;
        LabelTextBalance = BankTransferSummaryContentPageResources.LabelTextBalance;
        LabelTextBefore = BankTransferSummaryContentPageResources.LabelTextBefore;
        LabelTextAfter = BankTransferSummaryContentPageResources.LabelTextAfter;

        ElapsedTimeLoadingDataText = $"{BankTransferSummaryContentPageResources.ElapsedTimeLoadingDataText} ";
        RecordFoundOn = $" {BankTransferSummaryContentPageResources.RecordFoundOn} ";
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

    private void CustomPicker_OnSelectedIndexChanged(object? sender, EventArgs e)
        => RefreshDataGrid();


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

        // if (VCategoryDerivesFilter.Count > 0)
        // {
        //     var categoryName = VCategoryDerivesFilter.Select(s => s.CategoryName!);
        //     query = query.Where(s => categoryName.Contains(s.Category));
        // }
        //
        // if (HistoryDescriptions.Count > 0)
        // {
        //     var historyDescriptions = HistoryDescriptions.Select(s => s.StringValue);
        //     query = query.Where(s => historyDescriptions.Contains(s.Description));
        // }
        //
        // if (ModePaymentDeriveFilter.Count > 0)
        // {
        //     var modePayments = ModePaymentDeriveFilter.Select(s => s.Name);
        //     query = query.Where(s => modePayments.Contains(s.ModePayment));
        // }
        //
        // if (HistoryValues.Count > 0)
        // {
        //     var historyValues = HistoryValues.Select(s => s.DoubleValue);
        //     query = query.Where(s => historyValues.Contains(s.Value));
        // }
        //
        // if (HistoryChecked.Count > 0)
        // {
        //     var historyChecked = HistoryChecked.Select(s => s.BoolValue);
        //     query = query.Where(s => historyChecked.Contains((bool)s.IsPointed!));
        // }
        //
        // if (PlaceDeriveFilter.Count > 0)
        // {
        //     var historyPlaces = PlaceDeriveFilter.Select(s => s.Name);
        //     query = query.Where(s => historyPlaces.Contains(s.Place));
        // }

        RowTotalFilteredCount = query.Count();

        var records = query
            .OrderByDescending(s => s.Date);

        stopwatch.Stop();
        ElapsedTimeLoadingData = stopwatch.Elapsed.ToString("hh\\:mm\\:ss");

        BankTransferSummaries.Clear();
        BankTransferSummaries.AddRange(records);
    }
}