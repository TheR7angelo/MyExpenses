using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Bases.Views.Exports;

[Keyless]
public class ExportVHistory
{
    [Column("id")]
    public int? Id { get; init; }

    [Column("account_name")]
    [MaxLength(55)]
    public string? AccountName { get; init; }

    [Column("description")]
    [MaxLength(255)]
    public string? Description { get; init; }

    [Column("category_type")]
    [MaxLength(55)]
    public string? CategoryType { get; init; }

    [Column("mode_payment")]
    [MaxLength(55)]
    public string? ModePayment { get; init; }

    [Column("value")]
    public double? Value { get; init; }

    [Column("date", TypeName = "DATETIME")]
    public DateTime? Date { get; init; }

    [Column("place")]
    [MaxLength(155)]
    public string? Place { get; init; }

    [Column("is_pointed", TypeName = "BOOLEAN")]
    public bool? IsPointed { get; init; }

    [Column("date_added", TypeName = "DATETIME")]
    public DateTime? DateAdded { get; init; }

    [Column("date_pointed", TypeName = "DATETIME")]
    public DateTime? DatePointed { get; init; }
}
