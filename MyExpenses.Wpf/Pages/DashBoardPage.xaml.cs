﻿using System.Collections.ObjectModel;
using System.ComponentModel;
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
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using MyExpenses.Utils.Sql;
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
    public object ButtonCategoryTypeManagement { get; } = DashBoardPageResources.ButtonCategoryTypeManagement;
    public object ButtonColorManagement { get; } = DashBoardPageResources.ButtonColorManagement;
    public object ButtonCurrencyManagement { get; } = DashBoardPageResources.ButtonCurrencyManagement;
    public string ButtonLocationManagement { get; } = DashBoardPageResources.ButtonLocationManagement;

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

    #endregion

    public DashBoardPage()
    {
        RefreshAccountTotal();

        InitializeComponent();

        // TODO add listener color change
        var brush = (SolidColorBrush)FindResource("MaterialDesignBody");
        var wpfColor = brush.Color;
        PieChart.LegendTextPaint = new SolidColorPaint(wpfColor.ToSKColor());
    }

    #region Action

    private void ButtonAccountManagement_OnClick(object sender, RoutedEventArgs e)
    {
        var accountManagementPage = new AccountManagementPage { DashBoardPage = this };
        nameof(MainWindow.FrameBody).NavigateTo(accountManagementPage);
    }

    private void ButtonAccountTypeManagement_OnClick(object sender, RoutedEventArgs e)
    {
        var accountTypeManagementPage = new AccountTypeManagementPage { DashBoardPage = this };
        nameof(MainWindow.FrameBody).NavigateTo(accountTypeManagementPage);
    }

    private void ButtonCategoryTypeManagement_OnClick(object sender, RoutedEventArgs e)
    {
        var categoryTypeManagementPage = new CategoryTypeManagementPage { DashBoardPage = this };
        nameof(MainWindow.FrameBody).NavigateTo(categoryTypeManagementPage);
    }

    private void ButtonColorManagement_OnClick(object sender, RoutedEventArgs e)
        => nameof(MainWindow.FrameBody).NavigateTo(typeof(ColorManagementPage));

    private void ButtonCurrencyManagement_OnClick(object sender, RoutedEventArgs e)
    {
        var currencyManagementPage = new CurrencyManagementPage { DashBoardPage = this };
        nameof(MainWindow.FrameBody).NavigateTo(currencyManagementPage);
    }

    private void ButtonLocationManagement_OnClick(object sender, RoutedEventArgs e)
        => nameof(MainWindow.FrameBody).NavigateTo(typeof(LocationManagementPage));

    private void DataGridRow_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        DataGridRow = sender as DataGridRow;
    }

    private void ItemsControlVTotalAccount_OnLoaded(object sender, RoutedEventArgs e)
        => RefreshRadioButtonSelected();

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
        var radioButtons = ItemsControlVTotalAccount.FindVisualChildren<RadioButton>();
        var radioButton = radioButtons.FirstOrDefault();
        if (radioButton is null) return;
        radioButton.IsChecked = true;
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
            .Select(g => new { Category = g.Key, Total = g.Sum(s => s.Value) ?? 0d,
                g.First().Symbol, g.First().HexadecimalColorCode })
            .ToList();

        var grandTotal = Math.Round(categoriesTotals.Sum(ct => Math.Abs(ct.Total)), 2);

        var series = new List<PieSeries<double>>();
        foreach (var categoryTotal in categoriesTotals)
        {
            var total = Math.Round(categoryTotal.Total, 2);
            var absTotal = Math.Abs(total);
            var percentage = Math.Round(absTotal / grandTotal * 100, 2);

            var pieSeries = new PieSeries<double>
            {
                Values = new ObservableCollection<double> { absTotal },
                Name = $"{categoryTotal.Category} ({percentage}%)",
                ToolTipLabelFormatter = _ => $"{total:F2} {categoryTotal.Symbol}",
                Tag = categories.First(s => s.Name == categoryTotal.Category)
            };

            var hexadecimalCode = categoryTotal.HexadecimalColorCode;
            if (!string.IsNullOrEmpty(hexadecimalCode))
            {
                var skColor = hexadecimalCode.ToSkColor()!;
                if (skColor is not null) pieSeries.Fill = new SolidColorPaint((SKColor)skColor);
            }

            series.Add(pieSeries);
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
}