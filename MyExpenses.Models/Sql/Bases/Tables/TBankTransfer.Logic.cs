namespace MyExpenses.Models.Sql.Bases.Tables;

public partial class TBankTransfer : IDefaultBehavior
{
    public TBankTransfer()
        => SetDefaultValues();

    public void SetDefaultValues()
    {
        Date = DateTime.Today;
        DateAdded = DateTime.Now;
    }
}