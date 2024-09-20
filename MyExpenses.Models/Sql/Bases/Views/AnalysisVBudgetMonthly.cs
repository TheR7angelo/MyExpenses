using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Bases.Views;

[Keyless]
public partial class AnalysisVBudgetMonthly
{
    [Column("account_id")]
    public int? AccountId { get; set; }

    [Column("account_name")]
    public string? AccountName { get; set; }

    [Column("period")]
    public string? Period { get; set; }

    [Column("current_period_value")]
    public double? CurrentPeriodValue { get; set; }

    [Column("previous_period")]
    public string? PreviousPeriod { get; set; }

    [Column("previous_period_value")]
    public double? PreviousPeriodValue { get; set; }

    [Column("status")]
    public string? Status { get; set; }

    [Column("percentage")]
    public double? Percentage { get; set; }

    [Column("difference_value")]
    public double? DifferenceValue { get; set; }
}
