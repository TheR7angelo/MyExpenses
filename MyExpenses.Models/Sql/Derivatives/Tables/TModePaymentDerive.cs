using MyExpenses.Models.Sql.Bases.Tables;

namespace MyExpenses.Models.Sql.Derivatives.Tables;

public class TModePaymentDerive : TModePayment
{
    public bool IsChecked { get; set; }
}