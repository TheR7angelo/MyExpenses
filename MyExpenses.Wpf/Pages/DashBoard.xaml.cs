using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Models.Sql.Views;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using MyExpenses.Wpf.Resources.Resx.DashBoard;
using SkiaSharp.Views.WPF;

namespace MyExpenses.Wpf.Pages;

public partial class DashBoard : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public List<VHistory> VHistories { get; }
    public List<VTotalByAccount> VTotalByAccounts { get; }

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

    public string TotalStr => Total.HasValue ? Total.Value.ToString("F2") : "0.00";

    public string ButtonAddAccount { get; } = DashBoardResources.ButtonAddAccount;
    public string DataGridTextColumnAccount { get; } = DashBoardResources.DataGridTextColumnAccount;
    public string DataGridTextColumnDescription { get; } = DashBoardResources.DataGridTextColumnDescription;
    public string DataGridTextColumnCategory { get; } = DashBoardResources.DataGridTextColumnCategory;
    public string DataGridTextColumnModePayment { get; } = DashBoardResources.DataGridTextColumnModePayment;
    public string DataGridTextColumnValue { get; } = DashBoardResources.DataGridTextColumnValue;
    public string DataGridTextColumnDate { get; } = DashBoardResources.DataGridTextColumnDate;
    public string DataGridTextColumnPointed { get; } = DashBoardResources.DataGridTextColumnPointed;

    public DashBoard()
    {
        using var context = new DataBaseContext();
        VTotalByAccounts = new List<VTotalByAccount>(context.VTotalByAccounts);
        VHistories = new List<VHistory>(context.VHistories.OrderByDescending(s => s.Date));

        InitializeComponent();

        // TODO add listener color change
        var brush = (SolidColorBrush)FindResource("MaterialDesignBody");
        var wpfColor = brush.Color;
        PieChart.LegendTextPaint = new SolidColorPaint(wpfColor.ToSKColor());
    }

    #region Function

    private void UpdateGraph(string accountName)
    {
        using var context = new DataBaseContext();
        var brutCategoriesTotals = context.VDetailTotalCategories
            .Where(s => s.Account == accountName);

        var now = DateTime.Now;
        var categoriesTotals = brutCategoriesTotals
            .Where(s => s.Year == now.Year && s.Month == now.Month)
            .GroupBy(s => s.Category)
            .Select(g => new { Category = g.Key, Total = g.Sum(s => s.Value) ?? 0 })
            .ToList();

        var grandTotal = Math.Round(categoriesTotals.Sum(ct => Math.Abs(ct.Total)), 2);

        var series = new List<PieSeries<double>>();
        foreach (var categoryTotal in categoriesTotals)
        {
            var total = Math.Round(categoryTotal.Total, 2);
            var absTotal = Math.Abs(total);
            var percentage = Math.Round(absTotal / grandTotal * 100, 2);

            // TODO add local currency
            var pieSeries = new PieSeries<double>
            {
                Values = new ObservableCollection<double> { absTotal },
                Name = $"{categoryTotal.Category} ({percentage}%)",
                ToolTipLabelFormatter = _ => total.ToString(CultureInfo.CurrentCulture)
            };

            series.Add(pieSeries);
        }

        PieChart.Series = series;
    }

    #endregion

    #region Action

    private void ButtonAddAccount_OnClick(object sender, RoutedEventArgs e)
    {
        var addAccount = new AddAccount();
        addAccount.ShowDialog();
        if (addAccount.DialogResult != true) return;

        var newAccount = addAccount.Account;

        if (addAccount.EnableStartingBalance)
        {
            var newHistory = addAccount.History;
            newAccount.THistories = new List<THistory> { newHistory };
        }

        var success = newAccount.AddOrEdit();
        MessageBox.Show(success ? DashBoardResources.MessageBoxAddAccountSuccess : DashBoardResources.MessageBoxAddAccountError);
    }

    private void DashBoard_OnLoaded(object sender, RoutedEventArgs e)
    {
        var radioButtons = ItemsControlVTotalAccount.FindVisualChildren<RadioButton>();
        var radioButton = radioButtons.FirstOrDefault();
        if (radioButton is null) return;
        radioButton.IsChecked = true;
    }

    private void ToggleButtonVTotalAccount_OnChecked(object sender, RoutedEventArgs e)
    {
        var button = (RadioButton)sender;
        var vTotalByAccount = (VTotalByAccount)button.DataContext;
        Total = vTotalByAccount.Total;

        UpdateGraph(vTotalByAccount.Name!);
    }

    #endregion
}