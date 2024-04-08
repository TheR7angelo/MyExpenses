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

    [InverseProperty("ModePaymentFkNavigation")]
    public virtual ICollection<THistory> THistories { get; set; } = new List<THistory>();
}
