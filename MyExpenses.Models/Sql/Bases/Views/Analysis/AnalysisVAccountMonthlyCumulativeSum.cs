using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Bases.Views.Analysis;

[Keyless]
public partial class AnalysisVAccountMonthlyCumulativeSum
{
    [Column("account_fk")]
    public int? AccountFk { get; init; }

    [Column("account")]
    [MaxLength(55)]
    public string? Account { get; init; }

    [Column("period")]
    [MaxLength(7)]
    public string? Period { get; init; }

    [Column("cumulative_sum")]
    public double? CumulativeSum { get; init; }

    [Column("currency_fk")]
    public int? CurrencyFk { get; init; }

    [Column("currency")]
    [MaxLength(55)]
    public string? Currency { get; init; }
}
