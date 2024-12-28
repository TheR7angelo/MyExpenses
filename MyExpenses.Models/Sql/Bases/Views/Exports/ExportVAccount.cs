using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Bases.Views.Exports;

[Keyless]
public class ExportVAccount
{
    [Column("id")]
    public int? Id { get; init; }

    [Column("name")]
    [MaxLength(55)]
    public string? Name { get; init; }

    [Column("account_type")]
    [MaxLength(100)]
    public string? AccountType { get; init; }

    [Column("currency")]
    [MaxLength(55)]
    public string? Currency { get; init; }

    [Column("active", TypeName = "BOOLEAN")]
    public bool? Active { get; init; }

    [Column("date_added", TypeName = "DATETIME")]
    public DateTime? DateAdded { get; init; }
}
