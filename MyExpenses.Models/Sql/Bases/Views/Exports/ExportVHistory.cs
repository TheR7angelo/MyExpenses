using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Bases.Views.Exports;

[Keyless]
public partial class ExportVHistory
{
    [Column("id")]
    public int? Id { get; set; }

    [Column("account_name")]
    [MaxLength(55)]
    public string? AccountName { get; set; }

    [Column("description")]
    [MaxLength(255)]
    public string? Description { get; set; }

    [Column("category_type")]
    [MaxLength(55)]
    public string? CategoryType { get; set; }

    [Column("mode_payment")]
    [MaxLength(55)]
    public string? ModePayment { get; set; }

    [Column("value")]
    public double? Value { get; set; }

    [Column("date", TypeName = "DATETIME")]
    public DateTime? Date { get; set; }

    [Column("place")]
    [MaxLength(155)]
    public string? Place { get; set; }

    [Column("is_pointed", TypeName = "BOOLEAN")]
    public bool? IsPointed { get; set; }

    [Column("date_added", TypeName = "DATETIME")]
    public DateTime? DateAdded { get; set; }

    [Column("date_pointed", TypeName = "DATETIME")]
    public DateTime? DatePointed { get; set; }
}
