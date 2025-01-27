using System.Collections.ObjectModel;
using MyExpenses.Models.Ui;
using MyExpenses.Sql.Context;

namespace MyExpenses.Wpf.UserControls.Analytics.AccountsModePaymentMonthlySumControls;

public partial class AccountsModePaymentMonthlySumControl
{
    public ObservableCollection<TabItemData> TabItems { get; } = [];

    public AccountsModePaymentMonthlySumControl()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Necessary instantiation of DataBaseContext to interact with the database.
        // This creates a scoped database context for performing queries and modifications in the database.
        using var context = new DataBaseContext();
        var accounts = context.TAccounts.OrderBy(s => s.Name);

        InitializeComponent();

        foreach (var account in accounts)
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            // The instantiation of the AccountModePaymentMonthlySumControl is required because
            // this control is specifically responsible for managing and displaying the logic
            // related to the visualization of account category data on a per-account basis.
            var accountModePaymentMonthlySumControl = new AccountModePaymentMonthlySumControl(account.Id);
            var tabItemData = new TabItemData
            {
                Header = account.Name!,
                Content = accountModePaymentMonthlySumControl
            };

            TabItems.Add(tabItemData);
        }
    }
}