using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using MyExpenses.Models.Sql.Bases.Enums;

namespace MyExpenses.Models.Sql.Bases.Views;

[Keyless]
public partial class AnalysisVBudgetMonthly
{
    [Column("account_id")]
    public int? AccountId { get; init; }

    [Column("account_name")]
    public string? AccountName { get; init; }

    [Column("symbol_id")]
    public int? SymbolId { get; init; }

    [Column("symbol")]
    public string? Symbol { get; init; }

    [Column("period")]
    public string? Period { get; init; }

    [Column("period_value")]
    public double? PeriodValue { get; init; }

    [Column("previous_period")]
    public string? PreviousPeriod { get; init; }

    [Column("previous_period_value")]
    public double? PreviousPeriodValue { get; init; }

    [Column("status")]
    public string? Status { get; init; }

    [NotMapped]
    public EAnalysisVBudgetStatut? EAnalysisVBudgetStatut
        => string.IsNullOrWhiteSpace(Status)
            ? null
            : (EAnalysisVBudgetStatut)Enum.Parse(typeof(EAnalysisVBudgetStatut), Status, true);

    [Column("percentage")]
    public double? Percentage { get; init; }

    [Column("difference_value")]
    public byte[]? DifferenceValue { get; init; }
}
