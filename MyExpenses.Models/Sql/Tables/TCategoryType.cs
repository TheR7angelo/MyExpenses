using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyExpenses.Models.Sql.Tables;

[Table("t_category_type")]
public partial class TCategoryType : ISql
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public string? Name { get; set; }

    [Column("color_fk")]
    public int? ColorFk { get; set; }

    [Column("date_added", TypeName = "DATETIME")]
    public DateTime? DateAdded { get; set; }

    [ForeignKey("ColorFk")]
    [InverseProperty("TCategoryTypes")]
    public virtual TColor? ColorFkNavigation { get; set; }

    [InverseProperty("CategoryTypeFkNavigation")]
    public virtual ICollection<THistory> THistories { get; set; } = new List<THistory>();
}
