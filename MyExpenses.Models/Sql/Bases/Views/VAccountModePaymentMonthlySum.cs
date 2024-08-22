using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Bases.Views;

[Keyless]
public partial class VAccountModePaymentMonthlySum
{
    [Column("account_fk")]
    public int? AccountFk { get; set; }

    [Column("account")]
    public string? Account { get; set; }

    [Column("mode_payment")]
    public string? ModePayment { get; set; }

    [Column("period")]
    public string? Period { get; set; }

    [Column("monthly_sum")]
    public double? MonthlySum { get; set; }

    [Column("monthly_mode_payment")]
    public int? MonthlyModePayment { get; set; }
}
