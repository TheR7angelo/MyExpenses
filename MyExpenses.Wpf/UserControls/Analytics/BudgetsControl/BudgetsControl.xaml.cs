using System.Windows.Controls;
using MyExpenses.Sql.Context;

namespace MyExpenses.Wpf.UserControls.Analytics.BudgetsControl;

public partial class BudgetsControl
{
    public BudgetsControl()
    {
        using var context = new DataBaseContext();
        var accounts = context.TAccounts.OrderBy(s => s.Name);

        InitializeComponent();

        var budgetGlobal = new BudgetGlobalControl();
        var tabItem = new TabItem
        {
            Header = "Global",
            Content = budgetGlobal,
        };
        TabControl.Items.Add(tabItem);

        foreach (var account in accounts)
        {
            var budgetControl = new BudgetControl(account.Id);

            tabItem = new TabItem
            {
                Header = account.Name,
                Content = budgetControl
            };
            TabControl.Items.Add(tabItem);
        }

        InitializeComponent();
    }
}