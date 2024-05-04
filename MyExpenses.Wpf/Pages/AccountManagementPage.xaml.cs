using System.Collections.ObjectModel;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Sql.Context;

namespace MyExpenses.Wpf.Pages;

public partial class AccountManagementPage
{
    public ObservableCollection<TAccount> Accounts { get; }

    public AccountManagementPage()
    {
        using var context = new DataBaseContext();
        Accounts = [..context.TAccounts];

        InitializeComponent();
    }
}