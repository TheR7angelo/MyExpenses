using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyExpenses.Models.Sql.Tables;

public class TVersion
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("version")]
    public string? VersionStr { get; set; }

    [NotMapped]
    public Version? Version => VersionStr is not null ? new Version(VersionStr) : null;
}