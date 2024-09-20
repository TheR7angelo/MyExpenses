using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Bases.Views;

[Keyless]
public partial class AnalysisVBudgetPeriodAnnual
{
    [Column("account_id")]
    public int? AccountId { get; set; }

    [Column("account_name")]
    public string? AccountName { get; set; }

    [Column("current_month")]
    public string? CurrentMonth { get; set; }

    [Column("current_month_value")]
    public double? CurrentMonthValue { get; set; }

    [Column("previous_year_month_value")]
    public double? PreviousYearMonthValue { get; set; }

    [Column("previous_year_month")]
    public string? PreviousYearMonth { get; set; }

    [Column("cumulative_value")]
    public double? CumulativeValue { get; set; }

    [Column("moving_average")]
    public double? MovingAverage { get; set; }

    [Column("status")]
    public string? Status { get; set; }

    [Column("percentage")]
    public double? Percentage { get; set; }

    [Column("difference_value")]
    public double? DifferenceValue { get; set; }

    [Column("total_previous")]
    public double? TotalPrevious { get; set; }
}
