using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using MyExpenses.Models.Sql.Bases.Enums;

namespace MyExpenses.Models.Sql.Bases.Views;

[Keyless]
public partial class AnalysisVBudgetTotalAnnual
{
    [Column("account_id")]
    public int? AccountId { get; init; }

    [Column("account_name")]
    public string? AccountName { get; init; }

    [Column("symbol_id")]
    public int? SymbolId { get; init; }

    [Column("symbol")]
    public string? Symbol { get; init; }

    [Column("year")]
    public int? Year { get; init; }

    [Column("year_value")]
    public double? YearValue { get; init; }

    [Column("previous_year")]
    public int? PreviousYear { get; init; }

    [Column("previous_year_value")]
    public double? PreviousYearValue { get; init; }

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
