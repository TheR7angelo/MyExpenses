using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Bases.Views.Exports;

[Keyless]
public class ExportVRecursiveExpense
{
    [Column("id")]
    public int? Id { get; init; }

    [Column("account_name")]
    [MaxLength(55)]
    public string? AccountName { get; init; }

    [Column("description")]
    [MaxLength(255)]
    public string? Description { get; init; }

    [Column("note")]
    [MaxLength(255)]
    public string? Note { get; init; }

    [Column("category_type")]
    [MaxLength(55)]
    public string? CategoryType { get; init; }

    [Column("mode_payment")]
    [MaxLength(55)]
    public string? ModePayment { get; init; }

    [Column("value")]
    public double? Value { get; init; }

    [Column("place_name")]
    [MaxLength(155)]
    public string? PlaceName { get; init; }

    [Column("start_date", TypeName = "DATE")]
    public DateTime? StartDate { get; init; }

    [Column("recursive_total")]
    public int? RecursiveTotal { get; init; }

    [Column("recursive_count")]
    public int? RecursiveCount { get; init; }

    [Column("frequency")]
    [MaxLength(55)]
    public string? Frequency { get; init; }

    [Column("next_due_date", TypeName = "DATE")]
    public DateTime? NextDueDate { get; init; }

    [Column("is_active", TypeName = "BOOLEAN")]
    public bool? IsActive { get; init; }

    [Column("force_deactivate", TypeName = "BOOLEAN")]
    public bool? ForceDeactivate { get; init; }

    [Column("date_added", TypeName = "DATETIME")]
    public DateTime? DateAdded { get; init; }

    [Column("last_updated", TypeName = "DATETIME")]
    public DateTime? LastUpdated { get; init; }
}
