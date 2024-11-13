using MyExpenses.Models.Sql.Bases.Tables;

namespace MyExpenses.Models.Sql.Derivatives.Tables;

public class TAccountDerive : TAccount
{
    public bool IsChecked { get; set; }
}