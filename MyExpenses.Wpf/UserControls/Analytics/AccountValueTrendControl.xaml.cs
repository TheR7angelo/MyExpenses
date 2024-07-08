using MyExpenses.Sql.Context;

namespace MyExpenses.Wpf.UserControls.Analytics;

public partial class AccountValueTrendControl
{
    public AccountValueTrendControl()
    {
        using var context = new DataBaseContext();
        var groupsVAccountMonthlyCumulativeSums = context.VAccountMonthlyCumulativeSums
            .ToList()
            .GroupBy(s => s.Account)
            .ToList();

        InitializeComponent();
    }
}