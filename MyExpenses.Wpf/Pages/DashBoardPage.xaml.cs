﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Models.Sql.Views;
using MyExpenses.Models.Wpf.Charts;
using MyExpenses.Sql.Context;
using MyExpenses.Utils.Collection;
using MyExpenses.Utils.Strings;
using MyExpenses.Wpf.Resources.Resx.Pages.DashBoardPage;
using MyExpenses.Wpf.Utils;
using SkiaSharp;
using SkiaSharp.Views.WPF;

namespace MyExpenses.Wpf.Pages;

public partial class DashBoardPage : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public ObservableCollection<VHistory> VHistories { get; } = [];
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

    public string ButtonAccountManagement { get; } = DashBoardPageResources.ButtonAccountManagement;
    public string ButtonAccountTypeManagement { get; } = DashBoardPageResources.ButtonAccountTypeManagement;
    public string ButtonCategoryTypeManagement { get; } = DashBoardPageResources.ButtonCategoryTypeManagement;
    public string ButtonColorManagement { get; } = DashBoardPageResources.ButtonColorManagement;
    public string ButtonCurrencyManagement { get; } = DashBoardPageResources.ButtonCurrencyManagement;
    public string ButtonLocationManagement { get; } = DashBoardPageResources.ButtonLocationManagement;
    public string ButtonModePaymentManagement { get; } = DashBoardPageResources.ButtonModePaymentManagement;
    public string ButtonMakeBankTransfer { get; } = DashBoardPageResources.ButtonMakeBankTransfer;
    public string ButtonRecordExpense { get; } = DashBoardPageResources.ButtonRecordExpense;

    #endregion

    #region DataGrid

    public string DataGridTextColumnAccount { get; } = DashBoardPageResources.DataGridTextColumnAccount;
    public string DataGridTextColumnDescription { get; } = DashBoardPageResources.DataGridTextColumnDescription;
    public string DataGridTextColumnCategory { get; } = DashBoardPageResources.DataGridTextColumnCategory;
    public string DataGridTextColumnModePayment { get; } = DashBoardPageResources.DataGridTextColumnModePayment;
    public string DataGridTextColumnValue { get; } = DashBoardPageResources.DataGridTextColumnValue;
    public string DataGridTextColumnDate { get; } = DashBoardPageResources.DataGridTextColumnDate;
    public string DataGridTextColumnPlace { get; } = DashBoardPageResources.DataGridTextColumnPlace;
    public string DataGridTextColumnPointed { get; } = DashBoardPageResources.DataGridTextColumnPointed;

    public string DataGridMenuItemHeaderEditRecord { get; } = DashBoardPageResources.DataGridMenuItemHeaderEditRecord;
    public string DataGridMenuItemHeaderDeleteRecord { get; } = DashBoardPageResources.DataGridMenuItemHeaderDeleteRecord;

    #endregion

    public string ComboBoxYearsHintAssist { get; } = DashBoardPageResources.ComboBoxYearsHintAssist;
    public string ComboBoxMonthHintAssist { get; } = DashBoardPageResources.ComboBoxMonthHintAssist;

    public ObservableCollection<CategoryTotal> CategoryTotals { get; } = [];
    public ObservableCollection<string> Years { get; }
    public ObservableCollection<string> Months { get; }

    //TODO work
    private static string NoFilter => "All";

    private string _selectedYear = NoFilter;

    public string SelectedYear
    {
        get => _selectedYear;
        set
        {
            _selectedYear = value;
            OnPropertyChanged();
        }
    }

    private string _selectedMonth = NoFilter;

    public string SelectedMonth
    {
        get => _selectedMonth;
        set
        {
            _selectedMonth = value;
            OnPropertyChanged();
        }
    }

    public DashBoardPage()
    {
        using var context = new DataBaseContext();
        Years =
        [
            NoFilter,
            ..context
                .THistories
                .Where(s => s.Date.HasValue)
                .Select(s => s.Date!.Value.Year.ToString())
                .Distinct()
                .OrderByDescending(y => y)
        ];

        Months =
        [
            NoFilter,
            ..CultureInfo.CurrentCulture.DateTimeFormat.MonthNames
                .Where(s => !string.IsNullOrEmpty(s))
                .Select(s => s.ToFirstCharUpper())
        ];

        var now = DateTime.Now;
        if(Years.Count.Equals(0)) {Years.Add(DateTime.Now.Year.ToString());}
        SelectedYear = now.Year.ToString();
        SelectedMonth = Months[now.Month];

        RefreshAccountTotal();

        InitializeComponent();

        // TODO add listener color change
        var brush = (SolidColorBrush)FindResource("MaterialDesignBody");
        var wpfColor = brush.Color;
        PieChart.LegendTextPaint = new SolidColorPaint(wpfColor.ToSKColor());
    }

    #region Action

    private void ButtonAccountManagement_OnClick(object sender, RoutedEventArgs e)
        => nameof(MainWindow.FrameBody).NavigateTo(typeof(AccountManagementPage));


    private void ButtonAccountTypeManagement_OnClick(object sender, RoutedEventArgs e)
        => nameof(MainWindow.FrameBody).NavigateTo(typeof(AccountTypeManagementPage));


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
    {
        var recordExpensePage = new RecordExpensePage { DashBoardPage = this };
        nameof(MainWindow.FrameBody).NavigateTo(recordExpensePage);
    }

    private void DataGridRow_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        DataGridRow = sender as DataGridRow;
    }

    private void ItemsControlVTotalAccount_OnLoaded(object sender, RoutedEventArgs e)
        => RefreshRadioButtonSelected();

    //TODO work
    private void MenuItemDeleteRecord_OnClick(object sender, RoutedEventArgs e)
    {
        if (DataGridRow!.DataContext is not VHistory vHistory) return;
        var history = vHistory.ToTHistory();

        history?.Delete(true);

        VHistories.Remove(vHistory);

        var name = vHistory.Account!;
        var dateTime = DateTime.Now;
        UpdateGraph(name, dateTime);
        RefreshDataGrid(name, dateTime);

        //TODO refresh total account display
    }

    private void MenuItemEditRecord_OnClick(object sender, RoutedEventArgs e)
    {
        if (DataGridRow!.DataContext is not VHistory vHistory) return;
        var history = vHistory.ToTHistory();
        if (history is null) return;

        var recordExpensePage = new RecordExpensePage { DashBoardPage = this };
        recordExpensePage.SetTHistory(history);

        nameof(MainWindow.FrameBody).NavigateTo(recordExpensePage);
    }

    private void MenuItemPointed_OnClick(object sender, RoutedEventArgs e)
    {
        if (DataGridRow!.DataContext is not VHistory vHistory) return;
        var history = vHistory.ToTHistory();

        history!.Pointed = !history.Pointed;
        history.AddOrEdit();
        RefreshDataGrid(vHistory.Account!, history.Date!.Value);
    }

    private void ToggleButtonVTotalAccount_OnChecked(object sender, RoutedEventArgs e)
    {
        var button = (RadioButton)sender;
        var vTotalByAccount = (VTotalByAccount)button.DataContext;
        Total = vTotalByAccount.Total;
        Symbol = vTotalByAccount.Symbol;

        var name = vTotalByAccount.Name;
        if (string.IsNullOrEmpty(name)) return;

        var dateTime = DateTime.Now;
        UpdateGraph(name, dateTime);
        RefreshDataGrid(name, dateTime);
    }

    #endregion

    #region Function

    internal void RefreshAccountTotal()
    {
        using var context = new DataBaseContext();
        VTotalByAccounts.Clear();
        VTotalByAccounts.AddRange([..context.VTotalByAccounts]);
    }

    private void RefreshDataGrid(string name, DateTime dateTime)
    {
        using var context = new DataBaseContext();
        VHistories.Clear();
        var records = context.VHistories
            .Where(s => s.Account == name)
            .Where(s => s.Date!.Value.Year == dateTime.Year && s.Date!.Value.Month == dateTime.Month)
            .OrderBy(s => s.Pointed).ThenByDescending(s => s.Date);
        VHistories.AddRange(records);
    }

    internal void RefreshRadioButtonSelected()
    {
        var radioButtons = ItemsControlVTotalAccount.FindVisualChildren<RadioButton>().ToList();
        foreach (var radioButton in radioButtons) radioButton.IsChecked = false;

        var firstRadioButton = radioButtons.FirstOrDefault();
        if (firstRadioButton is null) return;
        firstRadioButton.IsChecked = true;
    }

    internal void UpdateGraph(string accountName, DateTime dateTime)
    {
        using var context = new DataBaseContext();
        var categories = context.TCategoryTypes.ToList();
        var brutCategoriesTotals = context.VDetailTotalCategories
            .Where(s => s.Account == accountName);

        var categoriesTotals = brutCategoriesTotals
            .Where(s => s.Year == dateTime.Year && s.Month == dateTime.Month)
            .GroupBy(s => s.Category)
            .Select(g => new
            {
                Category = g.Key, Total = g.Sum(s => s.Value) ?? 0d,
                g.First().Symbol, g.First().HexadecimalColorCode
            })
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