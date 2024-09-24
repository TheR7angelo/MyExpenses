using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using MyExpenses.Models.Sql.Bases.Enums;

namespace MyExpenses.Models.Sql.Bases.Views.Analysis;

[Keyless]
public partial class AnalysisVBudgetPeriodAnnual
{
    [Column("account_fk")]
    public int? AccountFk { get; init; }

    [Column("account_name")]
    public string? AccountName { get; init; }

    [Column("symbol_fk")]
    public int? SymbolFk { get; init; }

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
    public double? DifferenceValue { get; init; }
}
