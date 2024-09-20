using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Bases.Views;

[Keyless]
public partial class AnalysisVAccountMonthlyCumulativeSum
{
    [Column("account_fk")]
    public int? AccountFk { get; set; }

    [Column("account")]
    public string? Account { get; set; }

    [Column("period")]
    public string? Period { get; set; }

    [Column("cumulative_sum")]
    public double? CumulativeSum { get; set; }

    [Column("currency_fk")]
    public int? CurrencyFk { get; set; }

    [Column("currency")]
    public string? Currency { get; set; }
}
