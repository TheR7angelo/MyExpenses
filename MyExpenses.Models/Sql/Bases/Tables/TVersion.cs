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

    // The Version property is a derived, readonly property representing the parsed version from VersionStr.
    // It is marked as [NotMapped] to indicate that it is not stored in the database and exists purely for convenience.
    // ReSharper disable once HeapView.ObjectAllocation.Evident
    [NotMapped]
    public Version? Version
        => VersionStr is not null ? new Version(VersionStr) : null;
}