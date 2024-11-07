using MyExpenses.Models.Sql.Bases.Views;

namespace MyExpenses.Models.Sql.Derivatives.Views;

public class VCategoryDerive : VCategory
{
    public bool IsChecked { get; set; }
}