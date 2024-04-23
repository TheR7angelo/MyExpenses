using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using LiveChartsCore.SkiaSharpView;
using MyExpenses.Models.Sql.Views;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;

namespace MyExpenses.Wpf.Pages;

public partial class DashBoard : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public List<VHistory> VHistories { get; }
    public List<VTotalByAccount> VTotalByAccounts { get; }

    private double? _total { get; set; } = 0d;

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

    public DashBoard()
    {
        using var context = new DataBaseContext();
        VTotalByAccounts = new List<VTotalByAccount>(context.VTotalByAccounts);
        VHistories = new List<VHistory>(context.VHistories.OrderByDescending(s => s.Date));

        InitializeComponent();
    }

    #region Function

    private void DashBoard_OnLoaded(object sender, RoutedEventArgs e)
    {
        var radioButtons = ItemsControlVTotalAccount.FindVisualChildren<RadioButton>();
        var radioButton = radioButtons.FirstOrDefault();
        if (radioButton is null) return;
        radioButton.IsChecked = true;
    }

    #endregion

    #region Action

    private void ToggleButtonVTotalAccount_OnChecked(object sender, RoutedEventArgs e)
    {
        var button = (RadioButton)sender;
        var vTotalByAccount = (VTotalByAccount)button.DataContext;
        Total = vTotalByAccount.Total;

        UpdateGraph(vTotalByAccount.Name!);
    }

    private void UpdateGraph(string accountName)
    {
        using var context = new DataBaseContext();
        var categoriesTotals = context.VDetailTotalCategories
            .Where(s => s.Account == accountName)
            .GroupBy(s => s.Category)
            .Select(g => new { Category = g.Key, Total = g.Sum(s => s.Value) ?? 0 })
            .ToList();

        var grandTotal = Math.Round(categoriesTotals.Sum(ct => Math.Abs(ct.Total)), 2);

        var series = new List<PieSeries<double>>();
        foreach (var categoryTotal in categoriesTotals)
        {
            var total = Math.Round(Math.Abs(categoryTotal.Total), 2);
            var percentage = Math.Round(total / grandTotal * 100, 2);

            var pieSeries = new PieSeries<double>
            {
                Values = new ObservableCollection<double> { total },
                Name = $"{categoryTotal.Category} ({percentage}%)"
            };

            series.Add(pieSeries);
        }

        PieChart.Series = series;
    }

    #endregion
}