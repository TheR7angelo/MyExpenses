using System.Collections.ObjectModel;
using MyExpenses.Models.Ui;
using MyExpenses.Sql.Context;

namespace MyExpenses.Wpf.UserControls.Analytics;

public partial class AccountsCategorySumControl
{
    public ObservableCollection<TabItemData> TabItems { get; } = [];

    public AccountsCategorySumControl()
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
            // The instantiation of the AccountCategorySumControl is necessary because
            // this control is responsible for managing and visualizing account category
            // data, tailored to specific accounts.
            var accountCategorySumControl = new AccountCategorySumControl(account.Id);
            var tabItemData = new TabItemData
            {
                Header = account.Name!,
                Content = accountCategorySumControl
            };

            TabItems.Add(tabItemData);
        }
    }
}