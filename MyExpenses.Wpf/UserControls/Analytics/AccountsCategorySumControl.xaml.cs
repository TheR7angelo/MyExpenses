using System.Windows.Controls;
using MyExpenses.Sql.Context;

namespace MyExpenses.Wpf.UserControls.Analytics;

public partial class AccountsCategorySumControl
{
    // public ObservableCollection<TAccount> Accounts { get; }

    public AccountsCategorySumControl()
    {
        using var context = new DataBaseContext();
        var accounts = context.TAccounts.OrderBy(s => s.Name);

        InitializeComponent();

        foreach (var account in accounts)
        {
            var accountCategorySumControl = new AccountCategorySumControl(account.Id);

            var tabItem = new TabItem
            {
                Header = account.Name,
                Content = accountCategorySumControl
            };
            TabControl.Items.Add(tabItem);
        }
    }
}