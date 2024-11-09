using MyExpenses.Models.Sql.Bases.Tables;

namespace MyExpenses.Models.Sql.Derivatives.Tables;

public class TPlaceDerive : TPlace
{
    public bool IsChecked { get; set; }
}