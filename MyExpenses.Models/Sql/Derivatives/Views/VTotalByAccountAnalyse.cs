using MyExpenses.Models.Sql.Bases.Views;

namespace MyExpenses.Models.Sql.Derivatives.Views;

public class VTotalByAccountAnalyse : VTotalByAccount
{
    public override string? ToString() => Name;
}