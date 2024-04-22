using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using MyExpenses.Models.Sql.Views;
using MyExpenses.Sql.Context;

namespace MyExpenses.Wpf;

public partial class MainWindow : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public List<VHistory> VHistories { get; }
    public List<VTotalByAccount> VTotalByAccounts { get; }

    private double? _total { get; set; } = 0d;

    private double? Total
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

    public MainWindow()
    {
        using var context = new DataBaseContext();
        VTotalByAccounts = new List<VTotalByAccount>(context.VTotalByAccounts);
        VHistories = new List<VHistory>(context.VHistories.OrderByDescending(s => s.Date));

        InitializeComponent();
    }

    #region Action

    private void ButtonVTotalAccount_OnClick(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        var vTotalByAccount = (VTotalByAccount)button.DataContext;
        Total = vTotalByAccount.Total;
    }

    #endregion


}