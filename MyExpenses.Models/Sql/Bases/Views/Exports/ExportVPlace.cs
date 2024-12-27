using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Bases.Views.Exports;

[Keyless]
public class ExportVPlace
{
    [Column("id")]
    public int? Id { get; set; }

    [Column("name")]
    [MaxLength(155)]
    public string? Name { get; set; }

    [Column("number")]
    [MaxLength(20)]
    public string? Number { get; set; }

    [Column("street")]
    [MaxLength(155)]
    public string? Street { get; set; }

    [Column("postal")]
    [MaxLength(10)]
    public string? Postal { get; set; }

    [Column("city")]
    [MaxLength(100)]
    public string? City { get; set; }

    [Column("country")]
    [MaxLength(55)]
    public string? Country { get; set; }

    [Column("latitude")]
    public double? Latitude { get; set; }

    [Column("longitude")]
    public double? Longitude { get; set; }

    [Column("is_open", TypeName = "BOOLEAN")]
    public bool? IsOpen { get; set; }

    [Column("can_be_deleted", TypeName = "BOOLEAN")]
    public bool? CanBeDeleted { get; set; }

    [Column("date_added", TypeName = "DATETIME")]
    public DateTime? DateAdded { get; set; }
}
