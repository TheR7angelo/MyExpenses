using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Bases.Views.Analysis;

[Keyless]
public partial class AnalysisVAccountModePaymentCategoryMonthlySum
{
    [Column("account_fk")]
    public int? AccountFk { get; init; }

    [Column("account")]
    [MaxLength(55)]
    public string? Account { get; init; }

    [Column("mode_payment")]
    [MaxLength(55)]
    public string? ModePayment { get; init; }

    [Column("period")]
    [MaxLength(7)]
    public string? Period { get; init; }

    [Column("category")]
    [MaxLength(55)]
    public string? Category { get; init; }

    [Column("hexadecimal_color_code", TypeName = "TEXT(9)")]
    [MaxLength(9)]
    public string? HexadecimalColorCode { get; init; }

    [Column("monthly_sum")]
    public double? MonthlySum { get; init; }

    [Column("currency_fk")]
    public int? CurrencyFk { get; init; }

    [Column("currency")]
    [MaxLength(55)]
    public string? Currency { get; init; }

    [Column("monthly_mode_payment")]
    public int? MonthlyModePayment { get; init; }
}
