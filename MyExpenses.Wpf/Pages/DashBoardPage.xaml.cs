using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FilterDataGrid;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Models.Sql.Bases.Views;
using MyExpenses.Models.Wpf.Charts;
using MyExpenses.Sql.Context;
using MyExpenses.Utils.Collection;
using MyExpenses.Utils.Strings;
using MyExpenses.Wpf.Resources.Resx.Pages.DashBoardPage;
using MyExpenses.Wpf.Utils;
using MyExpenses.Wpf.Utils.FilterDataGrid;
using MyExpenses.Wpf.Windows;
using MyExpenses.Wpf.Windows.MsgBox;
using SkiaSharp;

namespace MyExpenses.Wpf.Pages;

public partial class DashBoardPage
{
    public ObservableCollection<VHistory> VHistories { get; }
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

    public static readonly DependencyProperty LocalLanguageProperty = DependencyProperty.Register(nameof(LocalLanguage),
        typeof(Local), typeof(DashBoardPage), new PropertyMetadata(default(Local)));

    public Local LocalLanguage
    {
        get => (Local)GetValue(LocalLanguageProperty);
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

    private static VTotalByAccount? _staticVTotalByAccount;

    public static readonly DependencyProperty CurrentVTotalByAccountProperty =
        DependencyProperty.Register(nameof(CurrentVTotalByAccount), typeof(VTotalByAccount), typeof(DashBoardPage),
            new PropertyMetadata(default(VTotalByAccount)));

    public VTotalByAccount? CurrentVTotalByAccount
    {
        get => (VTotalByAccount)GetValue(CurrentVTotalByAccountProperty);
        set => SetValue(CurrentVTotalByAccountProperty, value);
    }

    private static DashBoardPage Instance { get; set; } = null!;

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

        var now = DateTime.Now;
        using var context = new DataBaseContext();
        var recurrences = context.TRecursiveExpenses
            .Where(s => (bool)s.IsActive!)
            .Where(s => s.NextDueDate.Year.Equals(now.Year) && s.NextDueDate.Month.Equals(now.Month))
            .ToList();

        // TODO work
        if (recurrences.Count > 0)
        {
            var mainWindow = Application.Current.MainWindow;
            var actualWidth = mainWindow!.ActualWidth;

            var recurrentAddWindow = new RecurrentAddWindow(actualWidth);
            recurrentAddWindow.ShowDialog();
        }

        UpdateMonthLanguage();

        Years =
        [
            ..context
                .THistories
                .Where(s => s.Date.HasValue)
                .Select(s => s.Date!.Value.Year.ToString())
                .Distinct()
                .OrderByDescending(y => y)
        ];

        if (Years.Count.Equals(0))
        {
            Years.Add(DateTime.Now.Year.ToString());
        }

        SelectedYear = now.Year.ToString();
        SelectedMonth = Months[now.Month - 1];

        RefreshAccountTotal();

        InitializeComponent();
        UpdateLanguage();

        VHistories = new ObservableCollection<VHistory>();
        FilterDataGrid.ItemsSource = VHistories;

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

    private void Interface_OnThemeChanged(object sender, ConfigurationThemeChangedEventArgs e)
        => UpdatePieChartLegendTextPaint();

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
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
    {
        RefreshDataGrid();
        UpdateGraph();
    }

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
        UpdateGraph(name);
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
        UpdateGraph(accountName);

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

        history.Pointed = !history.Pointed;

        if (history.Pointed is true) history.DatePointed = DateTime.Now;
        else history.DatePointed = null;

        history.AddOrEdit();

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
        if (string.IsNullOrEmpty(accountName))
        {
            var radioButtons = ItemsControlVTotalAccount?.FindVisualChildren<RadioButton>().ToList() ?? [];
            if (radioButtons.Count.Equals(0)) return;

            accountName = radioButtons.FirstOrDefault(s => (bool)s.IsChecked!)?.Content as string;
        }

        if (string.IsNullOrEmpty(accountName)) return;

        using var context = new DataBaseContext();
        VHistories.Clear();

        var query = context.VHistories
            .Where(s => s.Account == accountName);

        if (!string.IsNullOrEmpty(SelectedMonth))
        {
            var monthInt = Months.IndexOf(SelectedMonth) + 1;
            query = query.Where(s => s.Date!.Value.Month.Equals(monthInt));
        }

        if (!string.IsNullOrEmpty(SelectedYear))
        {
            var yearInt = SelectedYear.ToInt();
            query = query.Where(s => s.Date!.Value.Year.Equals(yearInt));
        }

        var records = query
            .OrderBy(s => s.Pointed)
            .ThenByDescending(s => s.Date);

        VHistories.AddRange(records);
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

    private void UpdateGraph(string? accountName = null)
    {
        if (string.IsNullOrEmpty(accountName))
        {
            var radioButtons = ItemsControlVTotalAccount?.FindVisualChildren<RadioButton>().ToList() ?? [];
            if (radioButtons.Count.Equals(0)) return;

            accountName = radioButtons.FirstOrDefault(s => (bool)s.IsChecked!)?.Content as string;
        }

        using var context = new DataBaseContext();
        var categories = context.TCategoryTypes.ToList();

        var query = context.VDetailTotalCategories
            .Where(s => s.Account == accountName);

        if (!string.IsNullOrEmpty(SelectedMonth))
        {
            var monthInt = Months.IndexOf(SelectedMonth) + 1;
            query = query.Where(s => s.Month.Equals(monthInt));
        }

        if (!string.IsNullOrEmpty(SelectedYear))
        {
            var yearInt = SelectedYear.ToInt();
            query = query.Where(s => s.Year.Equals(yearInt));
        }

        var categoriesTotals = query
            .GroupBy(s => s.Category)
            .Select(g => new
            {
                Category = g.Key, Total = g.Sum(s => s.Value) ?? 0d,
                g.First().Symbol, g.First().HexadecimalColorCode
            })
            .OrderByDescending(s => Math.Abs(s.Total))
            .ToList();

        var grandTotal = Math.Round(categoriesTotals.Sum(ct => Math.Abs(ct.Total)), 2);

        CategoryTotals.Clear();

        var series = new List<PieSeries<double>>();
        foreach (var categoryTotalTemp in categoriesTotals)
        {
            var total = Math.Round(categoryTotalTemp.Total, 2);
            var absTotal = Math.Abs(total);
            var percentage = Math.Round(absTotal / grandTotal * 100, 2);

            var pieSeries = new PieSeries<double>
            {
                Values = new ObservableCollection<double> { absTotal },
                Name = $"{categoryTotalTemp.Category} ({percentage}%)",
                ToolTipLabelFormatter = _ => $"{total:F2} {categoryTotalTemp.Symbol}",
                Tag = categories.First(s => s.Name == categoryTotalTemp.Category)
            };

            var hexadecimalCode = categoryTotalTemp.HexadecimalColorCode;
            if (!string.IsNullOrEmpty(hexadecimalCode))
            {
                var skColor = hexadecimalCode.ToSkColor()!;
                if (skColor is not null) pieSeries.Fill = new SolidColorPaint((SKColor)skColor);
            }

            series.Add(pieSeries);

            var categoryTotal = new CategoryTotal
            {
                Name = categoryTotalTemp.Category,
                HexadecimalColor = hexadecimalCode,
                Percentage = percentage,
                Value = total,
                Symbol = categoryTotalTemp.Symbol
            };
            CategoryTotals.Add(categoryTotal);
        }

        PieChart.Series = series;
    }

    private void UpdateMonthLanguage()
    {
        var currentCulture = CultureInfo.CurrentCulture;
        LocalLanguage = currentCulture.ToLocal();

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