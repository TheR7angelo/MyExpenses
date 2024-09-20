using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Bases.Views;

[Keyless]
public partial class AnalysisVAccountCategoryMonthlySumPositiveNegative
{
    [Column("account_fk")]
    public int? AccountFk { get; set; }

    [Column("account")]
    public string? Account { get; set; }

    [Column("category_type")]
    public string? CategoryType { get; set; }

    [Column("color_code", TypeName = "TEXT(9)")]
    public string? ColorCode { get; set; }

    [Column("period")]
    public string? Period { get; set; }

    [Column("monthly_negative_sum")]
    public double? MonthlyNegativeSum { get; set; }

    [Column("monthly_positive_sum")]
    public double? MonthlyPositiveSum { get; set; }

    [Column("currency_fk")]
    public int? CurrencyFk { get; set; }

    [Column("currency")]
    public string? Currency { get; set; }
}
