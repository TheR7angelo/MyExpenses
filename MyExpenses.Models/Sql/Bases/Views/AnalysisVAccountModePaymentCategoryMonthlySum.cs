using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Bases.Views;

[Keyless]
public partial class AnalysisVAccountModePaymentCategoryMonthlySum
{
    [Column("account_fk")]
    public int? AccountFk { get; set; }

    [Column("account")]
    public string? Account { get; set; }

    [Column("mode_payment")]
    public string? ModePayment { get; set; }

    [Column("period")]
    public string? Period { get; set; }

    [Column("category")]
    public string? Category { get; set; }

    [Column("hexadecimal_color_code", TypeName = "TEXT(9)")]
    public string? HexadecimalColorCode { get; set; }

    [Column("monthly_sum")]
    public double? MonthlySum { get; set; }

    [Column("currency_fk")]
    public int? CurrencyFk { get; set; }

    [Column("currency")]
    public string? Currency { get; set; }

    [Column("monthly_mode_payment")]
    public int? MonthlyModePayment { get; set; }
}
