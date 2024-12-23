using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Bases.Views.Exports;

[Keyless]
public partial class ExportVCurrency
{
    [Column("id")]
    public int? Id { get; set; }

    [Column("symbol")]
    [MaxLength(55)]
    public string? Symbol { get; set; }

    [Column("date_added", TypeName = "DATETIME")]
    public DateTime? DateAdded { get; set; }
}
