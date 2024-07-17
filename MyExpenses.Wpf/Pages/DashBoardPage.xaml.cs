using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using FilterDataGrid;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Models.Sql.Views;
using MyExpenses.Models.Wpf.Charts;
using MyExpenses.Sql.Context;
using MyExpenses.Utils.Collection;
using MyExpenses.Utils.Strings;
using MyExpenses.Wpf.Resources.Resx.Pages.DashBoardPage;
using MyExpenses.Wpf.Utils;
using MyExpenses.Wpf.Utils.FilterDataGrid;
using SkiaSharp;
using SkiaSharp.Views.WPF;

namespace MyExpenses.Wpf.Pages;

public partial class DashBoardPage : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public ObservableCollection<VHistory> VHistories { get; }
    public ObservableCollection<VTotalByAccount> VTotalByAccounts { get; } = [];

    private double? _total = 0d;

    public double? Total
    {
        get => _total;
        set
        {
            _total = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TotalStr));
        }
    }

    private string? _symbol;

    private string? Symbol
    {
        get => _symbol;
        set
        {
            _symbol = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TotalStr));
        }
    }

    public string TotalStr => (Total.HasValue ? Total.Value.ToString("F2") : 0d.ToString("F2")) + $" {Symbol}";

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

    // TODO work
    // public string DataGridTemplateColumnCategorySortMemberPath { get; } = nameof(VHistory.Category);

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
    public ObservableCollection<string> Months { get; }

    private string _selectedYear = string.Empty;

    public string SelectedYear
    {
        get => _selectedYear;
        set
        {
            _selectedYear = value;
            OnPropertyChanged();
        }
    }

    private string _selectedMonth = string.Empty;

    public string SelectedMonth
    {
        get => _selectedMonth;
        set
        {
            _selectedMonth = value;
            OnPropertyChanged();
        }
    }

    public Local LocalLanguage { get; }

    public DashBoardPage()
    {
        using var context = new DataBaseContext();
        Years =
        [
            ..context
                .THistories
                .Where(s => s.Date.HasValue)
                .Select(s => s.Date!.Value.Year.ToString())
                .Distinct()
                .OrderByDescending(y => y)
        ];

        Months =
        [
            ..CultureInfo.CurrentCulture.DateTimeFormat.MonthNames
                .Where(s => !string.IsNullOrEmpty(s))
                .Select(s => s.ToFirstCharUpper())
        ];

        var now = DateTime.Now;
        if (Years.Count.Equals(0))
        {
            Years.Add(DateTime.Now.Year.ToString());
        }

        SelectedYear = now.Year.ToString();
        SelectedMonth = Months[now.Month - 1];

        RefreshAccountTotal();

        var currentCulture = CultureInfo.CurrentCulture;
        LocalLanguage = currentCulture.ToLocal();

        Interface.ThemeChanged += Interface_OnThemeChanged;
        Interface.LanguageChanged += Interface_OnLanguageChanged;

        InitializeComponent();
        UpdateLanguage();

        VHistories = new ObservableCollection<VHistory>();
        FilterDataGrid.ItemsSource = VHistories;

        UpdatePieChartLegendTextPaint();
    }

    #region Action

    private void ButtonAccountManagement_OnClick(object sender, RoutedEventArgs e)
        => nameof(MainWindow.FrameBody).NavigateTo(typeof(AccountManagementPage));

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

    private void ButtonDateNow_OnClick(object sender, RoutedEventArgs e)
    {
        var now = DateTime.Now;
        var yearStr = now.Year.ToString();

        SelectedYear = Years.Contains(yearStr) ? yearStr : string.Empty;
        SelectedMonth = Months[now.Month - 1];
    }

    private void ButtonLocationManagement_OnClick(object sender, RoutedEventArgs e)
        => nameof(MainWindow.FrameBody).NavigateTo(typeof(LocationManagementPage));

    private void ButtonModePaymentManagement_OnClick(object sender, RoutedEventArgs e)
        => nameof(MainWindow.FrameBody).NavigateTo(typeof(ModePaymentManagementPage));

    private void ButtonMakeBankTransfer_OnClick(object sender, RoutedEventArgs e)
        => nameof(MainWindow.FrameBody).NavigateTo(typeof(BankTransferPage));

    private void ButtonRecordExpense_OnClick(object sender, RoutedEventArgs e)
        => nameof(MainWindow.FrameBody).NavigateTo(typeof(RecordExpensePage));

    private void DataGridRow_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        => DataGridRow = sender as DataGridRow;

    private void Interface_OnThemeChanged(object sender, ConfigurationThemeChangedEventArgs e)
        => UpdatePieChartLegendTextPaint();

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
        => UpdateLanguage();

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

        DataGridMenuItemHeaderEditRecord = DashBoardPageResources.DataGridMenuItemHeaderEditRecord;
        DataGridMenuItemHeaderDeleteRecord = DashBoardPageResources.DataGridMenuItemHeaderDeleteRecord;

        ComboBoxYearsHintAssist = DashBoardPageResources.ComboBoxYearsHintAssist;
        ComboBoxMonthHintAssist = DashBoardPageResources.ComboBoxMonthHintAssist;
    }

    private void ItemsControlVTotalAccount_OnLoaded(object sender, RoutedEventArgs e)
        => RefreshRadioButtonSelected();

    // TODO work
    private void MenuItemDeleteRecord_OnClick(object sender, RoutedEventArgs e)
    {
        if (DataGridRow!.DataContext is not VHistory vHistory) return;
        var history = vHistory.Id.ToISqlT<THistory>();

        history?.Delete(true);

        VHistories.Remove(vHistory);

        var accountName = vHistory.Account!;

        RefreshDataGrid(accountName);
        UpdateGraph(accountName);

        //TODO refresh total account display
    }

    private void MenuItemEditRecord_OnClick(object sender, RoutedEventArgs e)
    {
        if (DataGridRow!.DataContext is not VHistory vHistory) return;
        var history = vHistory.Id.ToISqlT<THistory>();
        if (history is null) return;

        var recordExpensePage = new RecordExpensePage();
        recordExpensePage.SetTHistory(history);

        nameof(MainWindow.FrameBody).NavigateTo(recordExpensePage);
    }

    private void MenuItemPointed_OnClick(object sender, RoutedEventArgs e)
    {
        if (DataGridRow!.DataContext is not VHistory vHistory) return;
        var history = vHistory.Id.ToISqlT<THistory>()!;

        history.Pointed = !history.Pointed;

        if (history.Pointed is true) history.DatePointed = DateTime.Now;
        else history.DatePointed = null;

        history.AddOrEdit();

        RefreshDataGrid();
    }

    private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        RefreshDataGrid();
        UpdateGraph();
    }

    private void ToggleButtonVTotalAccount_OnChecked(object sender, RoutedEventArgs e)
    {
        var button = (RadioButton)sender;
        var vTotalByAccount = (VTotalByAccount)button.DataContext;
        Total = vTotalByAccount.Total;
        Symbol = vTotalByAccount.Symbol;

        var name = vTotalByAccount.Name;
        if (string.IsNullOrEmpty(name)) return;

        RefreshDataGrid(name);
    }

    #endregion

    #region Function

    private void RefreshAccountTotal()
    {
        using var context = new DataBaseContext();
        VTotalByAccounts.Clear();
        VTotalByAccounts.AddRange([..context.VTotalByAccounts]);
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
        foreach (var radioButton in radioButtons) radioButton.IsChecked = false;

        var firstRadioButton = radioButtons.FirstOrDefault();
        if (firstRadioButton is null) return;
        firstRadioButton.IsChecked = true;

        RefreshDataGrid();
        UpdateGraph();
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

    private void UpdatePieChartLegendTextPaint()
    {
        var brush = (SolidColorBrush)FindResource("MaterialDesignBody");
        var wpfColor = brush.Color;
        PieChart.LegendTextPaint = new SolidColorPaint(wpfColor.ToSKColor());
    }

    #endregion

    private void PieChart_OnDataPointerDown(IChartView chart, IEnumerable<ChartPoint> points)
    {
        // TODO zoom on data clicked
        var categoryType = (points.FirstOrDefault()?.Context.Series as PieSeries<double>)?.Tag as TCategoryType;
        Console.WriteLine(categoryType?.Name);
    }

    //TODO work
    private void ButtonPieChart_OnClick(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        if (button.DataContext is not CategoryTotal categoryTotal) return;

        Console.WriteLine(categoryTotal.Name);
    }

    // //TODO work
    // private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    // {
    //     var button = (Button)sender;
    //     if (button.DataContext is not VHistory vHistory) return;
    //
    //     var json = vHistory.ToJsonString();
    //     Console.WriteLine(json);
    // }
}