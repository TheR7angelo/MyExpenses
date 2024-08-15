using MyExpenses.Models.Sql.Bases.Views;

namespace MyExpenses.Models.Sql.Derivatives.Views;

public class VRecursiveExpenseDerive : VRecursiveExpense
{
    public bool RecursiveToAdd { get; set; } = true;
}