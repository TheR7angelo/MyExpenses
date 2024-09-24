using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using MyExpenses.Models.Sql.Bases.Enums;

namespace MyExpenses.Models.Sql.Bases.Views.Analysis;

[Keyless]
public partial class AnalysisVBudgetPeriodAnnualGlobal
{
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
