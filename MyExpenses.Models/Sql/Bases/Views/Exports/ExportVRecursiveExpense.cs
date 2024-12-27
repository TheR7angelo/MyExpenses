using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Bases.Views.Exports;

[Keyless]
public class ExportVRecursiveExpense
{
    [Column("id")]
    public int? Id { get; set; }

    [Column("account_name")]
    [MaxLength(55)]
    public string? AccountName { get; set; }

    [Column("description")]
    [MaxLength(255)]
    public string? Description { get; set; }

    [Column("note")]
    [MaxLength(255)]
    public string? Note { get; set; }

    [Column("category_type")]
    [MaxLength(55)]
    public string? CategoryType { get; set; }

    [Column("mode_payment")]
    [MaxLength(55)]
    public string? ModePayment { get; set; }

    [Column("value")]
    public double? Value { get; set; }

    [Column("place_name")]
    [MaxLength(155)]
    public string? PlaceName { get; set; }

    [Column("start_date", TypeName = "DATE")]
    public DateTime? StartDate { get; set; }

    [Column("recursive_total")]
    public int? RecursiveTotal { get; set; }

    [Column("recursive_count")]
    public int? RecursiveCount { get; set; }

    [Column("frequency")]
    [MaxLength(55)]
    public string? Frequency { get; set; }

    [Column("next_due_date", TypeName = "DATE")]
    public DateTime? NextDueDate { get; set; }

    [Column("is_active", TypeName = "BOOLEAN")]
    public bool? IsActive { get; set; }

    [Column("force_deactivate", TypeName = "BOOLEAN")]
    public bool? ForceDeactivate { get; set; }

    [Column("date_added", TypeName = "DATETIME")]
    public DateTime? DateAdded { get; set; }

    [Column("last_updated", TypeName = "DATETIME")]
    public DateTime? LastUpdated { get; set; }
}
