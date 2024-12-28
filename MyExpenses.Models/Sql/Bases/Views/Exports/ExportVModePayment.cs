using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Bases.Views.Exports;

[Keyless]
public class ExportVModePayment
{
    [Column("id")]
    public int? Id { get; init; }

    [Column("name")]
    [MaxLength(55)]
    public string? Name { get; init; }

    [Column("date_added", TypeName = "DATETIME")]
    public DateTime? DateAdded { get; init; }
}
