using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Bases.Views;

[Keyless]
public partial class AnalysisVBudgetTotalAnnual
{
    [Column("account_id")]
    public int? AccountId { get; set; }

    [Column("account_name")]
    public string? AccountName { get; set; }

    [Column("current_year")]
    public int? CurrentYear { get; set; }

    [Column("current_year_value")]
    public double? CurrentYearValue { get; set; }

    [Column("previous_year")]
    public int? PreviousYear { get; set; }

    [Column("previous_year_value")]
    public double? PreviousYearValue { get; set; }

    [Column("status")]
    public string? Status { get; set; }

    [Column("percentage")]
    public double? Percentage { get; set; }

    [Column("difference_value")]
    public double? DifferenceValue { get; set; }
}
