using System.Collections.ObjectModel;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Sql.Context;

namespace MyExpenses.Wpf.UserControls.Analytics;

public partial class AccountsCategorySumControl
{
    public ObservableCollection<TAccount> Accounts { get; }

    public AccountsCategorySumControl()
    {
        using var context = new DataBaseContext();
        Accounts = [..context.TAccounts.OrderBy(s => s.Name)];

        InitializeComponent();
    }
}