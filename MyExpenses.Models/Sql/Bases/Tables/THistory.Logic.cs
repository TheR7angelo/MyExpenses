namespace MyExpenses.Models.Sql.Bases.Tables;

public partial class THistory : IDefaultBehavior
{
    public THistory()
        => SetDefaultValues();

    public void SetDefaultValues()
    {
        Date = DateTime.Now;
        IsPointed = false;
        DateAdded = DateTime.Now;
    }
}