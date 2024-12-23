using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Bases.Views.Analysis;

[Keyless]
public partial class AnalysisVAccountCategoryMonthlySumPositiveNegative
{
    [Column("account_fk")]
    public int? AccountFk { get; init; }

    [Column("account")]
    [MaxLength(55)]
    public string? Account { get; init; }

    [Column("category_type")]
    [MaxLength(55)]
    public string? CategoryType { get; init; }

    [Column("color_code", TypeName = "TEXT(9)")]
    [MaxLength(9)]
    public string? ColorCode { get; init; }

    [Column("period")]
    [MaxLength(7)]
    public string? Period { get; init; }

    [Column("monthly_negative_sum")]
    public double? MonthlyNegativeSum { get; init; }

    [Column("monthly_positive_sum")]
    public double? MonthlyPositiveSum { get; init; }

    [Column("currency_fk")]
    public int? CurrencyFk { get; init; }

    [Column("currency")]
    [MaxLength(55)]
    public string? Currency { get; init; }
}
