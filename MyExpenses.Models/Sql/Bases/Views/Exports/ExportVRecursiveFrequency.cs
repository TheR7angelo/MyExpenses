using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Bases.Views.Exports;

[Keyless]
public class ExportVRecursiveFrequency
{
    [Column("id")]
    public int? Id { get; set; }

    [Column("frequency")]
    [MaxLength(55)]
    public string? Frequency { get; set; }

    [Column("description")]
    [MaxLength(100)]
    public string? Description { get; set; }
}
