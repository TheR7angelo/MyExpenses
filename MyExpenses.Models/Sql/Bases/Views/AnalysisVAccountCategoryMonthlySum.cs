using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Bases.Views;

[Keyless]
public partial class AnalysisVAccountCategoryMonthlySum
{
    [Column("account_fk")]
    public int? AccountFk { get; init; }

    [Column("account")]
    public string? Account { get; init; }

    [Column("category_type")]
    public string? CategoryType { get; init; }

    [Column("color_code")]
    public string? ColorCode { get; init; }

    [Column("period")]
    public string? Period { get; init; }

    [Column("monthly_sum")]
    public double? MonthlySum { get; init; }

    [Column("currency_fk")]
    public int? CurrencyFk { get; init; }

    [Column("currency")]
    public string? Currency { get; init; }
}
