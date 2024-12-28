using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Bases.Tables;

[Table("t_supported_languages")]
[Index("Code", IsUnique = true)]
public class TSupportedLanguage
{
    [Key]
    [Column("id")]
    public int Id { get; init; }

    [Column("code")]
    [MaxLength(10)]
    public string Code { get; init; } = null!;

    [Column("native_name")]
    [MaxLength(55)]
    public string NativeName { get; init; } = null!;

    [Column("english_name")]
    [MaxLength(55)]
    public string EnglishName { get; init; } = null!;

    [Column("default_language", TypeName = "BOOLEAN")]
    public bool? DefaultLanguage { get; init; } = false;

    [Column("date_added", TypeName = "DATETIME")]
    public DateTime? DateAdded { get; init; }
}
