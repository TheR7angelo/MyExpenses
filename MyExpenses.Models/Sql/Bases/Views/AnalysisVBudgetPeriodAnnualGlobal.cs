using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Bases.Views;

[Keyless]
public partial class AnalysisVBudgetPeriodAnnualGlobal
{
    [Column("current_period")]
    public string? CurrentMonth { get; set; }

    [Column("current_period_value")]
    public double? CurrentMonthValue { get; set; }

    [Column("previous_period")]
    public string? PreviousYearMonth { get; set; }

    [Column("previous_period_value")]
    public double? PreviousYearMonthValue { get; set; }

    [Column("status")]
    public string? Status { get; set; }

    [Column("percentage")]
    public double? Percentage { get; set; }

    [Column("difference_value")]
    public double? DifferenceValue { get; set; }
}
