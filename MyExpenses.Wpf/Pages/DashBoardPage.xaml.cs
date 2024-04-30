using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Models.Sql.Views;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using MyExpenses.Wpf.Utils;
using MyExpenses.Wpf.Resources.Resx.DashBoard;
using MyExpenses.Wpf.Windows;
using Serilog;
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

    public string TotalStr => Total.HasValue ? Total.Value.ToString("F2") : "0.00";

    #region Button WrapPanel

    public string ButtonAddAccount { get; } = DashBoardResources.ButtonAddAccount;
    public string ButtonLocationManagement { get; } = DashBoardResources.ButtonLocationManagement;

    #endregion

    #region DataGrid

    public string DataGridTextColumnAccount { get; } = DashBoardResources.DataGridTextColumnAccount;
    public string DataGridTextColumnDescription { get; } = DashBoardResources.DataGridTextColumnDescription;
    public string DataGridTextColumnCategory { get; } = DashBoardResources.DataGridTextColumnCategory;
    public string DataGridTextColumnModePayment { get; } = DashBoardResources.DataGridTextColumnModePayment;
    public string DataGridTextColumnValue { get; } = DashBoardResources.DataGridTextColumnValue;
    public string DataGridTextColumnDate { get; } = DashBoardResources.DataGridTextColumnDate;
    public string DataGridTextColumnPointed { get; } = DashBoardResources.DataGridTextColumnPointed;

    #endregion

    public DashBoardPage()
    {
        using var context = new DataBaseContext();

        RefreshAccountTotal();

        InitializeComponent();

        // TODO add listener color change
        var brush = (SolidColorBrush)FindResource("MaterialDesignBody");
        var wpfColor = brush.Color;
        PieChart.LegendTextPaint = new SolidColorPaint(wpfColor.ToSKColor());
    }

    #region Action

    private void ButtonAddAccount_OnClick(object sender, RoutedEventArgs e)
    {
        var addAccount = new AddAccountWindow();
        addAccount.ShowDialog();
        if (addAccount.DialogResult != true) return;

        var newAccount = addAccount.Account;

        if (addAccount.EnableStartingBalance)
        {
            var newHistory = addAccount.History;
            newAccount.THistories = new List<THistory> { newHistory };
        }

        Log.Information("Attempting to inject the new account \"{NewAccountName}\"", newAccount.Name);
        var (success, exception) = newAccount.AddOrEdit();
        if (success)
        {
            Log.Information("Account was successfully added");
            MessageBox.Show(DashBoardResources.MessageBoxAddAccountSuccess);

            RefreshAccountTotal();
            Application.Current.Dispatcher.InvokeAsync(RefreshRadioButtonSelected, DispatcherPriority.ContextIdle);
        }
        else
        {
            Log.Error(exception, "An error occurred please retry");
            MessageBox.Show(DashBoardResources.MessageBoxAddAccountError);
        }
    }

    private void ButtonLocationManagement_OnClick(object sender, RoutedEventArgs e)
    {
        Console.WriteLine("hey");
    }

    private void ItemsControlVTotalAccount_OnLoaded(object sender, RoutedEventArgs e)
        => RefreshRadioButtonSelected();


    private void ToggleButtonVTotalAccount_OnChecked(object sender, RoutedEventArgs e)
    {
        var button = (RadioButton)sender;
        var vTotalByAccount = (VTotalByAccount)button.DataContext;
        Total = vTotalByAccount.Total;

        var name = vTotalByAccount.Name;
        if (string.IsNullOrEmpty(name)) return;
        UpdateGraph(name);
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

    private void RefreshDataGrid(string name)
    {
        using var context = new DataBaseContext();
        VHistories.Clear();
        var records = context.VHistories
            .Where(s => s.Account == name)
            .OrderBy(s => s.Pointed).ThenByDescending(s => s.Date);
        VHistories.AddRange(records);
    }

    private void RefreshRadioButtonSelected()
    {
        var radioButtons = ItemsControlVTotalAccount.FindVisualChildren<RadioButton>();
        var radioButton = radioButtons.FirstOrDefault();
        if (radioButton is null) return;
        radioButton.IsChecked = true;
    }

    private void UpdateGraph(string accountName)
    {
        using var context = new DataBaseContext();
        var categories = context.TCategoryTypes.ToList();
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
                ToolTipLabelFormatter = _ => total.ToString(CultureInfo.CurrentCulture),
                Tag = categories.First(s => s.Name == categoryTotal.Category)
            };
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