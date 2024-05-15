using System.Collections.ObjectModel;
using System.Windows;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Sql.Context;

namespace MyExpenses.Wpf.Pages;

public partial class CurrencyManagementPage
{
    public ObservableCollection<TCurrency> Currencies { get; }
    public required DashBoardPage DashBoardPage { get; set; }

    public CurrencyManagementPage()
    {
        using var context = new DataBaseContext();
        Currencies = [..context.TCurrencies.OrderBy(s => s.Symbol)];

        InitializeComponent();
    }

    private void ButtonAddCurrency_OnClick(object sender, RoutedEventArgs e)
    {
        //TODO work
    }

    private void ButtonEditCurrency_OnClick(object sender, RoutedEventArgs e)
    {
        //TODO work
    }
}