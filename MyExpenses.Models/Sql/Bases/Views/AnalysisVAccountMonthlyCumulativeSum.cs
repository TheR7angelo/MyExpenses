using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Bases.Views;

[Keyless]
public partial class AnalysisVAccountMonthlyCumulativeSum
{
    [Column("account_fk")]
    public int? AccountFk { get; init; }

    [Column("account")]
    public string? Account { get; init; }

    [Column("period")]
    public string? Period { get; init; }

    [Column("cumulative_sum")]
    public double? CumulativeSum { get; init; }

    [Column("currency_fk")]
    public int? CurrencyFk { get; init; }

    [Column("currency")]
    public string? Currency { get; init; }
}
