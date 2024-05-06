using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyExpenses.Models.Sql.Tables;

[Table("t_color")]
public partial class TColor : ISql
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public string? Name { get; set; }

    [Column("hexadecimal_color_code", TypeName = "TEXT(9)")]
    [MaxLength(9)]
    public string? HexadecimalColorCode { get; set; }

    [Column("date_added", TypeName = "DATETIME")]
    public DateTime? DateAdded { get; set; }

    [InverseProperty("ColorFkNavigation")]
    public virtual ICollection<TCategoryType> TCategoryTypes { get; set; } = new List<TCategoryType>();
}
