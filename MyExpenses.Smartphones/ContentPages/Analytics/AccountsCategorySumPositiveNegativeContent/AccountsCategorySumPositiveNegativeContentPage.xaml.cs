using MyExpenses.Models.Ui;
using MyExpenses.Sql.Context;

namespace MyExpenses.Smartphones.ContentPages.Analytics.AccountsCategorySumPositiveNegativeContent;

public partial class AccountsCategorySumPositiveNegativeContentPage
{
    public List<TabItemData> TabItems { get; }

    public AccountsCategorySumPositiveNegativeContentPage()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Necessary instantiation of DataBaseContext to interact with the database.
        // This creates a scoped database context for performing queries and modifications in the database.
        using var context = new DataBaseContext();
        TabItems = [..context.TAccounts.OrderBy(s => s.Name)
            .Select(s => new TabItemData { Header = s.Name!, Content = s })];

        InitializeComponent();
    }
}