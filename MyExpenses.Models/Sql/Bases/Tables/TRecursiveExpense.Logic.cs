namespace MyExpenses.Models.Sql.Bases.Tables;

public partial class TRecursiveExpense : IDefaultBehavior
{
    public TRecursiveExpense()
        => SetDefaultValues();

    public void SetDefaultValues()
    {
        StartDate = DateOnly.FromDateTime(DateTime.Now);
        DateAdded = DateTime.Now;
    }
}