using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FilterDataGrid;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Models.Sql.Bases.Views;
using MyExpenses.Models.Sql.Queries;
using MyExpenses.Models.Wpf.Charts;
using MyExpenses.Sql.Context;
using MyExpenses.Sql.Queries;
using MyExpenses.Utils;
using MyExpenses.Utils.Collection;
using MyExpenses.Utils.Strings;
using MyExpenses.Wpf.Resources.Resx.Pages.DashBoardPage;
using MyExpenses.Wpf.Utils;
using MyExpenses.Wpf.Utils.FilterDataGrid;
using MyExpenses.Wpf.Windows;
using MyExpenses.Wpf.Windows.MsgBox;
using Serilog;
using SkiaSharp;

namespace MyExpenses.Wpf.Pages;

public partial class DashBoardPage
{
    public ObservableCollection<VHistory> VHistories { get; } = [];
    public ObservableCollection<VTotalByAccount> VTotalByAccounts { get; } = [];

    private DataGridRow? DataGridRow { get; set; }

    #region Button WrapPanel

    public static readonly DependencyProperty ButtonAccountManagementProperty =
        DependencyProperty.Register(nameof(ButtonAccountManagement), typeof(string), typeof(DashBoardPage),
            new PropertyMetadata(default(string)));

    public string ButtonAccountManagement
    {
        get => (string)GetValue(ButtonAccountManagementProperty);
        set => SetValue(ButtonAccountManagementProperty, value);
    }

    public static readonly DependencyProperty ButtonAccountTypeManagementProperty =
        DependencyProperty.Register(nameof(ButtonAccountTypeManagement), typeof(string), typeof(DashBoardPage),
            new PropertyMetadata(default(string)));

    public string ButtonAccountTypeManagement
    {
        get => (string)GetValue(ButtonAccountTypeManagementProperty);
        set => SetValue(ButtonAccountTypeManagementProperty, value);
    }

    public static readonly DependencyProperty ButtonCategoryTypeManagementProperty =
        DependencyProperty.Register(nameof(ButtonCategoryTypeManagement), typeof(string), typeof(DashBoardPage),
            new PropertyMetadata(default(string)));

    public string ButtonCategoryTypeManagement
    {
        get => (string)GetValue(ButtonCategoryTypeManagementProperty);
        set => SetValue(ButtonCategoryTypeManagementProperty, value);
    }

    public static readonly DependencyProperty ButtonLocationManagementProperty =
        DependencyProperty.Register(nameof(ButtonLocationManagement), typeof(string), typeof(DashBoardPage),
            new PropertyMetadata(default(string)));

    public string ButtonLocationManagement
    {
        get => (string)GetValue(ButtonLocationManagementProperty);
        set => SetValue(ButtonLocationManagementProperty, value);
    }

    public static readonly DependencyProperty ButtonColorManagementProperty =
        DependencyProperty.Register(nameof(ButtonColorManagement), typeof(string), typeof(DashBoardPage),
            new PropertyMetadata(default(string)));

    public string ButtonColorManagement
    {
        get => (string)GetValue(ButtonColorManagementProperty);
        set => SetValue(ButtonColorManagementProperty, value);
    }

    public static readonly DependencyProperty ButtonCurrencyManagementProperty =
        DependencyProperty.Register(nameof(ButtonCurrencyManagement), typeof(string), typeof(DashBoardPage),
            new PropertyMetadata(default(string)));

    public string ButtonCurrencyManagement
    {
        get => (string)GetValue(ButtonCurrencyManagementProperty);
        set => SetValue(ButtonCurrencyManagementProperty, value);
    }

    public static readonly DependencyProperty ButtonModePaymentManagementProperty =
        DependencyProperty.Register(nameof(ButtonModePaymentManagement), typeof(string), typeof(DashBoardPage),
            new PropertyMetadata(default(string)));

    public string ButtonModePaymentManagement
    {
        get => (string)GetValue(ButtonModePaymentManagementProperty);
        set => SetValue(ButtonModePaymentManagementProperty, value);
    }

    public static readonly DependencyProperty ButtonMakeBankTransferProperty =
        DependencyProperty.Register(nameof(ButtonMakeBankTransfer), typeof(string), typeof(DashBoardPage),
            new PropertyMetadata(default(string)));

    public string ButtonMakeBankTransfer
    {
        get => (string)GetValue(ButtonMakeBankTransferProperty);
        set => SetValue(ButtonMakeBankTransferProperty, value);
    }

    public static readonly DependencyProperty ButtonRecordExpenseProperty =
        DependencyProperty.Register(nameof(ButtonRecordExpense), typeof(string), typeof(DashBoardPage),
            new PropertyMetadata(default(string)));

    public string ButtonRecordExpense
    {
        get => (string)GetValue(ButtonRecordExpenseProperty);
        set => SetValue(ButtonRecordExpenseProperty, value);
    }

    public static readonly DependencyProperty ButtonAnalyticsProperty =
        DependencyProperty.Register(nameof(ButtonAnalytics), typeof(string), typeof(DashBoardPage),
            new PropertyMetadata(default(string)));

    public string ButtonAnalytics
    {
        get => (string)GetValue(ButtonAnalyticsProperty);
        set => SetValue(ButtonAnalyticsProperty, value);
    }

    public static readonly DependencyProperty ButtonRecurrentExpenseProperty =
        DependencyProperty.Register(nameof(ButtonRecurrentExpense), typeof(string), typeof(DashBoardPage),
            new PropertyMetadata(default(string)));

    public string ButtonRecurrentExpense
    {
        get => (string)GetValue(ButtonRecurrentExpenseProperty);
        set => SetValue(ButtonRecurrentExpenseProperty, value);
    }

    #endregion

    #region DataGrid

    public static readonly DependencyProperty DataGridMenuItemHeaderEditRecordProperty =
        DependencyProperty.Register(nameof(DataGridMenuItemHeaderEditRecord), typeof(string), typeof(DashBoardPage),
            new PropertyMetadata(default(string)));

    public string DataGridMenuItemHeaderEditRecord
    {
        get => (string)GetValue(DataGridMenuItemHeaderEditRecordProperty);
        set => SetValue(DataGridMenuItemHeaderEditRecordProperty, value);
    }

    public static readonly DependencyProperty DataGridMenuItemHeaderDeleteRecordProperty =
        DependencyProperty.Register(nameof(DataGridMenuItemHeaderDeleteRecord), typeof(string), typeof(DashBoardPage),
            new PropertyMetadata(default(string)));

    public string DataGridMenuItemHeaderDeleteRecord
    {
        get => (string)GetValue(DataGridMenuItemHeaderDeleteRecordProperty);
        set => SetValue(DataGridMenuItemHeaderDeleteRecordProperty, value);
    }

    public static readonly DependencyProperty DataGridCheckBoxColumnPointedProperty =
        DependencyProperty.Register(nameof(DataGridCheckBoxColumnPointed), typeof(string), typeof(DashBoardPage),
            new PropertyMetadata(default(string)));

    public string DataGridCheckBoxColumnPointed
    {
        get => (string)GetValue(DataGridCheckBoxColumnPointedProperty);
        set => SetValue(DataGridCheckBoxColumnPointedProperty, value);
    }

    public static readonly DependencyProperty ButtonContentEditRecordProperty =
        DependencyProperty.Register(nameof(ButtonContentEditRecord), typeof(string), typeof(DashBoardPage),
            new PropertyMetadata(default(string)));

    public string ButtonContentEditRecord
    {
        get => (string)GetValue(ButtonContentEditRecordProperty);
        set => SetValue(ButtonContentEditRecordProperty, value);
    }

    public static readonly DependencyProperty ButtonContentDeleteRecordProperty =
        DependencyProperty.Register(nameof(ButtonContentDeleteRecord), typeof(string), typeof(DashBoardPage),
            new PropertyMetadata(default(string)));

    public string ButtonContentDeleteRecord
    {
        get => (string)GetValue(ButtonContentDeleteRecordProperty);
        set => SetValue(ButtonContentDeleteRecordProperty, value);
    }

    public static readonly DependencyProperty ButtonContentPointedRecordProperty =
        DependencyProperty.Register(nameof(ButtonContentPointedRecord), typeof(string), typeof(DashBoardPage),
            new PropertyMetadata(default(string)));

    public string ButtonContentPointedRecord
    {
        get => (string)GetValue(ButtonContentPointedRecordProperty);
        set => SetValue(ButtonContentPointedRecordProperty, value);
    }

    #endregion

    public static readonly DependencyProperty ComboBoxYearsHintAssistProperty =
        DependencyProperty.Register(nameof(ComboBoxYearsHintAssist), typeof(string), typeof(DashBoardPage),
            new PropertyMetadata(default(string)));

    public string ComboBoxYearsHintAssist
    {
        get => (string)GetValue(ComboBoxYearsHintAssistProperty);
        set => SetValue(ComboBoxYearsHintAssistProperty, value);
    }

    public static readonly DependencyProperty ComboBoxMonthHintAssistProperty =
        DependencyProperty.Register(nameof(ComboBoxMonthHintAssist), typeof(string), typeof(DashBoardPage),
            new PropertyMetadata(default(string)));

    public string ComboBoxMonthHintAssist
    {
        get => (string)GetValue(ComboBoxMonthHintAssistProperty);
        set => SetValue(ComboBoxMonthHintAssistProperty, value);
    }

    public ObservableCollection<CategoryTotal> CategoryTotals { get; } = [];
    public ObservableCollection<string> Years { get; }
    public ObservableCollection<string> Months { get; } = [];

    public static readonly DependencyProperty SelectedYearProperty = DependencyProperty.Register(nameof(SelectedYear),
        typeof(string), typeof(DashBoardPage), new PropertyMetadata(default(string)));

    public string SelectedYear
    {
        get => (string)GetValue(SelectedYearProperty);
        set => SetValue(SelectedYearProperty, value);
    }

    public static readonly DependencyProperty SelectedMonthProperty = DependencyProperty.Register(nameof(SelectedMonth),
        typeof(string), typeof(DashBoardPage), new PropertyMetadata(default(string)));

    public string SelectedMonth
    {
        get => (string)GetValue(SelectedMonthProperty);
        set => SetValue(SelectedMonthProperty, value);
    }

    // ReSharper disable once HeapView.BoxingAllocation
    public static readonly DependencyProperty LocalLanguageProperty = DependencyProperty.Register(nameof(LocalLanguage),
        typeof(Local), typeof(DashBoardPage), new PropertyMetadata(default(Local)));

    public Local LocalLanguage
    {
        get => (Local)GetValue(LocalLanguageProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        set => SetValue(LocalLanguageProperty, value);
    }

    public static readonly DependencyProperty DateFormatStringProperty =
        DependencyProperty.Register(nameof(DateFormatString), typeof(string), typeof(DashBoardPage),
            new PropertyMetadata(default(string)));

    public string DateFormatString
    {
        get => (string)GetValue(DateFormatStringProperty);
        set => SetValue(DateFormatStringProperty, value);
    }

    public static readonly DependencyProperty CurrentVTotalByAccountProperty =
        DependencyProperty.Register(nameof(CurrentVTotalByAccount), typeof(VTotalByAccount), typeof(DashBoardPage),
            new PropertyMetadata(default(VTotalByAccount)));

    public VTotalByAccount? CurrentVTotalByAccount
    {
        get => (VTotalByAccount)GetValue(CurrentVTotalByAccountProperty);
        set => SetValue(CurrentVTotalByAccountProperty, value);
    }

    private static DashBoardPage Instance { get; set; } = null!;

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

    public DashBoardPage()
    {
        Instance = this;

        var (currentYear, currentMonth, _) = DateTime.Now;
        using var context = new DataBaseContext();
        var recurrences = context.GetActiveRecurrencesForCurrentMonth(currentYear, currentMonth);

        if (recurrences.Any())
        {
            var mainWindow = Application.Current.MainWindow!;
            var actualWidth = mainWindow.ActualWidth;
            var actualHeight = mainWindow.ActualHeight;
            var size = new Size(actualWidth, actualHeight);

            var recurrentAddWindow = new RecurrentAddWindow(size);
            recurrentAddWindow.ShowDialog();
        }

        UpdateMonthLanguage();

        Years =
        [
            ..context.GetDistinctYearsFromHistories(SortOrder.Descending)
                .Select(s => s.ToString())
        ];

        if (Years.Count.Equals(0)) Years.Add(currentYear.ToString());
        var lastYear = int.Parse(Years.Max()!);
        for (var year = lastYear + 1; year <= currentYear; year++)
        {
            Years.Insert(0, year.ToString());
        }

        SelectedYear = currentYear.ToString();
        SelectedMonth = Months[currentMonth - 1];

        RefreshAccountTotal();

        InitializeComponent();
        UpdateLanguage();

        UpdatePieChartLegendTextPaint();

        Interface.ThemeChanged += Interface_OnThemeChanged;
        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    #region Action

    #region ButtonNavigate

    private void ButtonAccountManagement_OnClick(object sender, RoutedEventArgs e)
    {
        var page = new AccountManagementPage { DashBoardPage = this };
        nameof(MainWindow.FrameBody).NavigateTo(page);
    }

    private void ButtonAccountTypeManagement_OnClick(object sender, RoutedEventArgs e)
        => nameof(MainWindow.FrameBody).NavigateTo(typeof(AccountTypeManagementPage));

    private void ButtonAnalytics_OnClick(object sender, RoutedEventArgs e)
        => nameof(MainWindow.FrameBody).NavigateTo(typeof(AnalyticsPage));

    private void ButtonCategoryTypeManagement_OnClick(object sender, RoutedEventArgs e)
        => nameof(MainWindow.FrameBody).NavigateTo(typeof(CategoryTypeManagementPage));

    private void ButtonColorManagement_OnClick(object sender, RoutedEventArgs e)
        => nameof(MainWindow.FrameBody).NavigateTo(typeof(ColorManagementPage));

    private void ButtonCurrencyManagement_OnClick(object sender, RoutedEventArgs e)
        => nameof(MainWindow.FrameBody).NavigateTo(typeof(CurrencyManagementPage));

    private void ButtonLocationManagement_OnClick(object sender, RoutedEventArgs e)
        => nameof(MainWindow.FrameBody).NavigateTo(typeof(LocationManagementPage));

    private void ButtonModePaymentManagement_OnClick(object sender, RoutedEventArgs e)
        => nameof(MainWindow.FrameBody).NavigateTo(typeof(ModePaymentManagementPage));

    private void ButtonMakeBankTransfer_OnClick(object sender, RoutedEventArgs e)
        => nameof(MainWindow.FrameBody).NavigateTo(typeof(BankTransferPage));

    private void ButtonRecordExpense_OnClick(object sender, RoutedEventArgs e)
        => nameof(MainWindow.FrameBody).NavigateTo(typeof(RecordExpensePage));

    private void ButtonRecurrentExpense_OnClick(object sender, RoutedEventArgs e)
        => nameof(MainWindow.FrameBody).NavigateTo(typeof(RecurrentExpensePage));

    #endregion

    private void ButtonAddMonth_OnClick(object sender, RoutedEventArgs e)
    {
        var date = GetDateOnlyFilter();
        date = date.AddMonths(1);

        var result = UpdateFilterDate(date);

        if (result) return;

        MsgBox.Show(DashBoardPageResources.MessageBoxAddMonthError, MsgBoxImage.Warning, MessageBoxButton.OK);
    }

    private void ButtonDateNow_OnClick(object sender, RoutedEventArgs e)
    {
        var now = DateOnly.FromDateTime(DateTime.Now);
        UpdateFilterDate(now);
    }

    private void ButtonDeleteRecord_OnClick(object sender, RoutedEventArgs e)
    {
        var response = MsgBox.Show(DashBoardPageResources.MessageBoxDeleteRecordQuestion,
            MsgBoxImage.Question, MessageBoxButton.YesNoCancel);

        if (response is not MessageBoxResult.Yes) return;

        var button = (Button)sender;
        if (button.DataContext is not VHistory vHistory) return;

        DeleteRecord(vHistory);
    }

    private void ButtonEditRecord_OnClick(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        if (button.DataContext is not VHistory vHistory) return;

        EditRecord(vHistory);
    }

    private void ButtonPointedRecord_OnClick(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        if (button.DataContext is not VHistory vHistory) return;

        PointRecord(vHistory);
    }

    private void ButtonRemoveMonth_OnClick(object sender, RoutedEventArgs e)
    {
        var date = GetDateOnlyFilter();
        date = date.AddMonths(-1);

        var result = UpdateFilterDate(date);

        if (result) return;

        MsgBox.Show(DashBoardPageResources.MessageBoxRemoveMonthError, MsgBoxImage.Warning, MessageBoxButton.OK);
    }

    private void DataGridRow_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        => DataGridRow = sender as DataGridRow;

    private void Interface_OnThemeChanged()
        => UpdatePieChartLegendTextPaint();

    private void Interface_OnLanguageChanged()
    {
        UpdateLanguage();
        UpdateMonthLanguage();
    }

    private void ItemsControlVTotalAccount_OnLoaded(object sender, RoutedEventArgs e)
        => RefreshRadioButtonSelected();

    private void MenuItemDeleteRecord_OnClick(object sender, RoutedEventArgs e)
    {
        var response = MsgBox.Show(DashBoardPageResources.MessageBoxDeleteRecordQuestion,
            MsgBoxImage.Question, MessageBoxButton.YesNoCancel);

        if (response is not MessageBoxResult.Yes) return;

        if (DataGridRow!.DataContext is not VHistory vHistory) return;

        DeleteRecord(vHistory);
    }

    private void MenuItemEditRecord_OnClick(object sender, RoutedEventArgs e)
    {
        if (DataGridRow!.DataContext is not VHistory vHistory) return;

        EditRecord(vHistory);
    }

    private void MenuItemPointed_OnClick(object sender, RoutedEventArgs e)
    {
        if (DataGridRow!.DataContext is not VHistory vHistory) return;
        PointRecord(vHistory);
    }

    private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        => RefreshDataGrid();

    private void ToggleButtonVTotalAccount_OnChecked(object sender, RoutedEventArgs e)
    {
        var button = (RadioButton)sender;
        if (button.DataContext is not VTotalByAccount vTotalByAccount) return;

        StaticVTotalByAccount = vTotalByAccount;

        // Total = vTotalByAccount.Total;
        // Symbol = vTotalByAccount.Symbol;

        var name = vTotalByAccount.Name;
        if (string.IsNullOrEmpty(name)) return;

        RefreshDataGrid(name);
    }

    #endregion

    #region Function

    private void DeleteRecord(VHistory vHistory)
    {
        var history = vHistory.Id.ToISql<THistory>();

        history?.Delete(true);

        VHistories.Remove(vHistory);

        var accountName = vHistory.Account!;

        RefreshDataGrid(accountName);
        RefreshAccountTotal(CurrentVTotalByAccount!.Id);
    }

    private static void EditRecord(VHistory vHistory)
    {
        var history = vHistory.Id.ToISql<THistory>();
        if (history is null) return;

        var recordExpensePage = new RecordExpensePage();
        recordExpensePage.SetTHistory(history);

        nameof(MainWindow.FrameBody).NavigateTo(recordExpensePage);
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

    private void PointRecord(VHistory vHistory)
    {
        var history = vHistory.Id.ToISql<THistory>()!;

        history.IsPointed = !history.IsPointed;

        if (history.IsPointed) history.DatePointed = DateTime.Now;
        else history.DatePointed = null;

        Log.Information("Attention to pointed record, id: \"{HistoryId}\"", history.Id);
        history.AddOrEdit();
        Log.Information("The recording was successfully pointed");

        RefreshDataGrid();
    }

    private void RefreshAccountTotal(int id)
    {
        using var context = new DataBaseContext();
        var newVTotalByAccount = context.VTotalByAccounts.FirstOrDefault(s => s.Id.Equals(id));
        if (newVTotalByAccount is null) return;

        var vTotalByAccount = VTotalByAccounts.FirstOrDefault(s => s.Id.Equals(id));
        if (vTotalByAccount is null) return;

        newVTotalByAccount.CopyPropertiesTo(vTotalByAccount);
    }

    internal void RefreshAccountTotal()
    {
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

    private void RefreshDataGrid(string? accountName = null)
    {
        if (string.IsNullOrEmpty(accountName)) accountName = GetAccountName();

        if (string.IsNullOrEmpty(accountName)) return;

        VHistories.Clear();

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

        var results = accountName.GetFilteredHistories(monthInt, yearInt);
        VHistories.AddRange(results.Histories);

        var filteredData = accountName.GetFilteredVDetailTotalCategories(monthInt, yearInt);
        var categoriesTotals = CalculateCategoryTotals(filteredData, out var grandTotal);
        UpdateChartUi(categoriesTotals, grandTotal);
    }

    private void RefreshRadioButtonSelected()
    {
        var radioButtons = ItemsControlVTotalAccount.FindVisualChildren<RadioButton>().ToList();

        var radioButton = StaticVTotalByAccount is null
            ? radioButtons.FirstOrDefault()
            : radioButtons.FirstOrDefault(rb =>
                rb.DataContext is VTotalByAccount vTotalByAccount &&
                vTotalByAccount.Id.Equals(StaticVTotalByAccount.Id));

        StaticVTotalByAccount = radioButton?.DataContext as VTotalByAccount;

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

    private string? GetAccountName(string? accountName = null)
    {
        if (!string.IsNullOrEmpty(accountName)) return accountName;

        var radioButtons = ItemsControlVTotalAccount?.FindVisualChildren<RadioButton>().ToList() ?? [];
        if (radioButtons.Count == 0) return null;

        return radioButtons.FirstOrDefault(s => s.IsChecked == true)?.Content as string;
    }

    private static List<CategoryTotalData> CalculateCategoryTotals(IEnumerable<VDetailTotalCategory> data, out double grandTotal)
    {
        var categoriesTotals = data
            .GroupBy(s => s.Category)
            .Select(g => new CategoryTotalData
            {
                Category = g.Key,
                Total = Math.Round(g.Sum(s => s.Value) ?? 0d, 2),
                Symbol = g.First().Symbol,
                HexadecimalColorCode = g.First().HexadecimalColorCode
            })
            .OrderByDescending(s => Math.Abs(s.Total))
            .ToList();

        grandTotal = Math.Round(categoriesTotals.Sum(ct => Math.Abs(ct.Total)), 2);

        return categoriesTotals;
    }

    private void UpdateChartUi(IEnumerable<CategoryTotalData> categoriesTotals, double grandTotal)
    {
        CategoryTotals.Clear();

        var series = new List<PieSeries<double>>();
        foreach (var categoryTotal in categoriesTotals)
        {
            var absTotal = Math.Abs(categoryTotal.Total);
            var percentage = Math.Round(absTotal / grandTotal * 100, 2);

            var pieSeries = new PieSeries<double>
            {
                Values = new ObservableCollection<double> { absTotal },
                Name = $"{categoryTotal.Category} ({percentage}%)",
                ToolTipLabelFormatter = _ => $"{categoryTotal.Total:F2} {categoryTotal.Symbol}",
                Tag = categoryTotal.Category
            };

            if (!string.IsNullOrEmpty(categoryTotal.HexadecimalColorCode))
            {
                var skColor = categoryTotal.HexadecimalColorCode.ToSkColor();
                if (skColor is not null)
                {
                    pieSeries.Fill = new SolidColorPaint((SKColor)skColor);
                }
            }

            series.Add(pieSeries);

            CategoryTotals.Add(new CategoryTotal
            {
                Name = categoryTotal.Category,
                HexadecimalColor = categoryTotal.HexadecimalColorCode,
                Percentage = percentage,
                Value = categoryTotal.Total,
                Symbol = categoryTotal.Symbol
            });
        }

        PieChart.Series = series;
    }

    private void UpdateMonthLanguage()
    {
        var currentCulture = CultureInfo.CurrentCulture;
        LocalLanguage = currentCulture.ToLocal();

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

    private void UpdateLanguage()
    {
        ButtonAccountManagement = DashBoardPageResources.ButtonAccountManagement;
        ButtonAccountTypeManagement = DashBoardPageResources.ButtonAccountTypeManagement;
        ButtonCategoryTypeManagement = DashBoardPageResources.ButtonCategoryTypeManagement;
        ButtonColorManagement = DashBoardPageResources.ButtonColorManagement;
        ButtonCurrencyManagement = DashBoardPageResources.ButtonCurrencyManagement;
        ButtonLocationManagement = DashBoardPageResources.ButtonLocationManagement;
        ButtonModePaymentManagement = DashBoardPageResources.ButtonModePaymentManagement;
        ButtonMakeBankTransfer = DashBoardPageResources.ButtonMakeBankTransfer;
        ButtonRecordExpense = DashBoardPageResources.ButtonRecordExpense;
        ButtonAnalytics = DashBoardPageResources.ButtonAnalytics;
        ButtonRecurrentExpense = DashBoardPageResources.ButtonRecurrentExpense;

        TextColumnAccount.Header = DashBoardPageResources.DataGridTextColumnAccount;
        TextColumnDescription.Header = DashBoardPageResources.DataGridTextColumnDescription;
        TemplateColumnCategory.Header = DashBoardPageResources.DataGridTextColumnCategory;
        TextColumnModePayment.Header = DashBoardPageResources.DataGridTextColumnModePayment;
        TemplateColumnValue.Header = DashBoardPageResources.DataGridTextColumnValue;
        TextColumnDate.Header = DashBoardPageResources.DataGridTextColumnDate;
        TextColumnPlace.Header = DashBoardPageResources.DataGridTextColumnPlace;
        CheckBoxColumnPointed.Header = DashBoardPageResources.DataGridTextColumnPointed;
        TemplateColumnActions.Header = DashBoardPageResources.DataGridTemplateColumnActionsHeader;
        ButtonContentEditRecord = DashBoardPageResources.ButtonContentEditRecord;
        ButtonContentPointedRecord = DashBoardPageResources.DataGridTextColumnPointed;
        ButtonContentDeleteRecord = DashBoardPageResources.ButtonContentDeleteRecord;

        DataGridCheckBoxColumnPointed = DashBoardPageResources.DataGridTextColumnPointed;
        DataGridMenuItemHeaderEditRecord = DashBoardPageResources.DataGridMenuItemHeaderEditRecord;
        DataGridMenuItemHeaderDeleteRecord = DashBoardPageResources.DataGridMenuItemHeaderDeleteRecord;

        ComboBoxYearsHintAssist = DashBoardPageResources.ComboBoxYearsHintAssist;
        ComboBoxMonthHintAssist = DashBoardPageResources.ComboBoxMonthHintAssist;

        DateFormatString = DashBoardPageResources.FilterDataGridDateFormatString;
    }

    private void UpdatePieChartLegendTextPaint()
    {
        var wpfColor = MyExpenses.Wpf.Utils.Resources.GetMaterialDesignBodySkColor();
        PieChart.LegendTextPaint = new SolidColorPaint(wpfColor);
    }

    #endregion
}