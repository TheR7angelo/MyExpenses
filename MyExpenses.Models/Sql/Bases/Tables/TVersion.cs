using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyExpenses.Models.Sql.Bases.Tables;

[Table("t_version")]
public class TVersion : ISql
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("version")]
    [MaxLength(25)]
    public string? VersionStr { get; init; }

    [NotMapped]
    public Version? Version
        => VersionStr is not null ? new Version(VersionStr) : null;
}