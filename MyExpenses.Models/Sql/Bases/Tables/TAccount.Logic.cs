namespace MyExpenses.Models.Sql.Bases.Tables;

public partial class TAccount : IDefaultBehavior
{
    public TAccount()
        => SetDefaultValues();

    public void SetDefaultValues()
    {
        Active = true;
        DateAdded = DateTime.Now;
    }
}