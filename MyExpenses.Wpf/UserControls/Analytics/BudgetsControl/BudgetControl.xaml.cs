using System.Windows.Controls;

namespace MyExpenses.Wpf.UserControls.Analytics.BudgetsControl;

public partial class BudgetControl
{
    public BudgetControl(int accountId)
    {
        InitializeComponent();

        //TODO add trad
        var budgetMonthlyControl = new BudgetMonthlyControl(accountId);
        var tabItemBudgetMonthlyControl = new TabItem
        {
            Header = "Budget Monthly",
            Content = budgetMonthlyControl
        };

        TabControl.Items.Add(tabItemBudgetMonthlyControl);
    }
}