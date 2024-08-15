using System.Collections.ObjectModel;
using MyExpenses.Models.Sql.Bases.Views;
using MyExpenses.Sql.Context;

namespace MyExpenses.Wpf.UserControls.Analytics.AccountTotalEllipseControl;

public partial class AccountTotalEllipseControl
{
    public ObservableCollection<VTotalByAccount> VTotalByAccounts { get; }
    public AccountTotalEllipseControl()
    {
        using var context = new DataBaseContext();
        VTotalByAccounts = [..context.VTotalByAccounts.OrderBy(s => s.Name)];

        InitializeComponent();
    }
}