using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using MyExpenses.Models.Sql.Views;
using MyExpenses.Sql.Context;

namespace MyExpenses.Wpf.Pages;

public partial class AccountManagementPage
{
    public ObservableCollection<VTotalByAccount> TotalByAccounts { get; }

    public AccountManagementPage()
    {
        using var context = new DataBaseContext();
        TotalByAccounts = [..context.VTotalByAccounts];

        InitializeComponent();
    }

    #region Action

    private void ButtonAddNewAccount_OnClick(object sender, RoutedEventArgs e)
    {
        Console.WriteLine("Need to create new account");
    }

    private void ButtonVAccount_OnClick(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        var vTotalByAccount = button.DataContext as VTotalByAccount;

        Console.WriteLine(vTotalByAccount?.Id);
    }

    #endregion
}