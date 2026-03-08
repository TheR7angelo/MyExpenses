using System.Collections.ObjectModel;
using MyExpenses.Sql.Context;
using MyExpenses.Sql.Entities;

namespace MyExpenses.Wpf.UserControls.Analytics.AccountTotalEllipseControl;

public partial class AccountTotalEllipseControl
{
    public ObservableCollection<VTotalByAccount> VTotalByAccounts { get; }
    public AccountTotalEllipseControl()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Necessary instantiation of DataBaseContext to interact with the database.
        using var context = new DataBaseContextOld();
        VTotalByAccounts = [..context.VTotalByAccounts.OrderBy(s => s.Name)];

        InitializeComponent();
    }
}