using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Bases.Views.Analysis;

[Keyless]
public partial class AnalysisVAccountCategoryMonthlySum
{
    [Column("account_fk")]
    public int? AccountFk { get; init; }

    [Column("account")]
    [MaxLength(55)]
    public string? Account { get; init; }

    [Column("category_type")]
    [MaxLength(55)]
    public string? CategoryType { get; init; }

    [Column("color_code")]
    [MaxLength(9)]
    public string? ColorCode { get; init; }

    [Column("period")]
    [MaxLength(7)]
    public string? Period { get; init; }

    [Column("monthly_sum")]
    public double? MonthlySum { get; init; }

    [Column("currency_fk")]
    public int? CurrencyFk { get; init; }

    [Column("currency")]
    [MaxLength(55)]
    public string? Currency { get; init; }
}
