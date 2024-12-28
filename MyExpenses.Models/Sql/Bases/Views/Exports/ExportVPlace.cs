using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Bases.Views.Exports;

[Keyless]
public class ExportVPlace
{
    [Column("id")]
    public int? Id { get; init; }

    [Column("name")]
    [MaxLength(155)]
    public string? Name { get; init; }

    [Column("number")]
    [MaxLength(20)]
    public string? Number { get; init; }

    [Column("street")]
    [MaxLength(155)]
    public string? Street { get; init; }

    [Column("postal")]
    [MaxLength(10)]
    public string? Postal { get; init; }

    [Column("city")]
    [MaxLength(100)]
    public string? City { get; init; }

    [Column("country")]
    [MaxLength(55)]
    public string? Country { get; init; }

    [Column("latitude")]
    public double? Latitude { get; init; }

    [Column("longitude")]
    public double? Longitude { get; init; }

    [Column("is_open", TypeName = "BOOLEAN")]
    public bool? IsOpen { get; init; }

    [Column("can_be_deleted", TypeName = "BOOLEAN")]
    public bool? CanBeDeleted { get; init; }

    [Column("date_added", TypeName = "DATETIME")]
    public DateTime? DateAdded { get; init; }
}
