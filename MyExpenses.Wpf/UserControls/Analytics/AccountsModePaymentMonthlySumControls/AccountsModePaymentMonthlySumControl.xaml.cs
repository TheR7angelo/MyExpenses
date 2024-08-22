using System.Windows.Controls;
using MyExpenses.Sql.Context;

namespace MyExpenses.Wpf.UserControls.Analytics.AccountsModePaymentMonthlySumControls;

public partial class AccountsModePaymentMonthlySumControl
{
    public AccountsModePaymentMonthlySumControl()
    {
        using var context = new DataBaseContext();
        var accounts = context.TAccounts.OrderBy(s => s.Name);

        InitializeComponent();

        foreach (var account in accounts)
        {
            var accountModePaymentMonthlySumControl = new AccountModePaymentMonthlySumControl(account.Id);

            var tabItem = new TabItem
            {
                Header = account.Name,
                Content = accountModePaymentMonthlySumControl
            };
            TabControl.Items.Add(tabItem);
        }
    }
}