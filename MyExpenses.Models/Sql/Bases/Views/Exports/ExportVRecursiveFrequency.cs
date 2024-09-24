using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Bases.Views.Exports;

[Keyless]
public partial class ExportVRecursiveFrequency
{
    [Column("id")]
    public int? Id { get; set; }

    [Column("frequency")]
    public string? Frequency { get; set; }

    [Column("description")]
    public string? Description { get; set; }
}
