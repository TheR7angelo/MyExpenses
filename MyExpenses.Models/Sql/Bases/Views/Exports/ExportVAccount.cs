using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Bases.Views.Exports;

[Keyless]
public class ExportVAccount
{
    [Column("id")]
    public int? Id { get; set; }

    [Column("name")]
    [MaxLength(55)]
    public string? Name { get; set; }

    [Column("account_type")]
    [MaxLength(100)]
    public string? AccountType { get; set; }

    [Column("currency")]
    [MaxLength(55)]
    public string? Currency { get; set; }

    [Column("active", TypeName = "BOOLEAN")]
    public bool? Active { get; set; }

    [Column("date_added", TypeName = "DATETIME")]
    public DateTime? DateAdded { get; set; }
}
