using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyExpenses.Models.Sql.Tables;

[Table("t_mode_payment")]
public partial class TModePayment
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public string? Name { get; set; }

    [Column("can_be_deleted", TypeName = "BOOLEAN")]
    public bool? CanBeDeleted { get; set; } = true;

    [Column("date_added", TypeName = "DATETIME")]
    public DateTime? DateAdded { get; set; } = DateTime.Now;

    [InverseProperty("ModePaymentFkNavigation")]
    public virtual ICollection<THistory> THistories { get; set; } = new List<THistory>();
}
