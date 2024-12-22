using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Bases.Tables;

[Table("t_supported_languages")]
[Index("Code", IsUnique = true)]
public partial class TSupportedLanguage
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("code")]
    [MaxLength(10)]
    public string Code { get; set; } = null!;

    [Column("native_name")]
    [MaxLength(55)]
    public string NativeName { get; set; } = null!;

    [Column("english_name")]
    [MaxLength(55)]
    public string EnglishName { get; set; } = null!;

    [Column("default_language", TypeName = "BOOLEAN")]
    public bool? DefaultLanguage { get; set; } = false;

    [Column("date_added", TypeName = "DATETIME")]
    public DateTime? DateAdded { get; set; } = DateTime.Now;
}
