using MyExpenses.Models.Ui;
using MyExpenses.Sql.Context;

namespace MyExpenses.Wpf.UserControls.Analytics.AccountsCategorySumPositiveNegativeControls;

public partial class AccountsCategorySumPositiveNegativeControl
{
    public List<TabItemData> TabItems { get; }

    public AccountsCategorySumPositiveNegativeControl()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Necessary instantiation of DataBaseContext to interact with the database.
        // This creates a scoped database context for performing queries and modifications in the database.
        using var context = new DataBaseContext();
        TabItems = [..context.TAccounts.OrderBy(s => s.Name)
            .Select(s => new TabItemData { Header = s.Name!, Content = new AccountCategorySumPositiveNegativeControl(s.Id) })];

        InitializeComponent();
    }
}