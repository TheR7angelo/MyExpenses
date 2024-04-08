using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyExpenses.Models.Sql.Tables;

[Table("t_category_type")]
public partial class TCategoryType
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public string? Name { get; set; }

    [InverseProperty("CategoryTypeFkNavigation")]
    public virtual ICollection<THistory> THistories { get; set; } = new List<THistory>();
}
