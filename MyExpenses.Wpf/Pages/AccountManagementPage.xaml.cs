using System.Collections.ObjectModel;
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
}