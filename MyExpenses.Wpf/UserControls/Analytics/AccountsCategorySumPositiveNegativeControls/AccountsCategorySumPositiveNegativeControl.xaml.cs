using System.Collections.ObjectModel;
using MyExpenses.Models.Ui;
using MyExpenses.Sql.Context;

namespace MyExpenses.Wpf.UserControls.Analytics.AccountsCategorySumPositiveNegativeControls;

public partial class AccountsCategorySumPositiveNegativeControl
{
    public ObservableCollection<TabItemData> TabItems { get; } = [];

    public AccountsCategorySumPositiveNegativeControl()
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
            // The instantiation of AccountCategorySumPositiveNegativeControl is essential because
            // this control handles the logic for visualizing account category data.
            var accountModePaymentMonthlySumControl = new AccountCategorySumPositiveNegativeControl(account.Id);
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            var tabItemData = new TabItemData
            {
                Header = account.Name!,
                Content = accountModePaymentMonthlySumControl
            };

            TabItems.Add(tabItemData);
        }
    }
}