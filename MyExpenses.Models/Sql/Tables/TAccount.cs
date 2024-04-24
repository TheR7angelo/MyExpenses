using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyExpenses.Models.Sql.Tables;

[Table("t_account")]
public partial class TAccount
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public string? Name { get; set; }

    [Column("account_type_fk")]
    public int? AccountTypeFk { get; set; }

    [Column("active", TypeName = "BOOLEAN")]
    public bool? Active { get; set; }

    [Column("date_added", TypeName = "DATETIME")]
    public DateTime? DateAdded { get; set; }

    [ForeignKey("AccountTypeFk")]
    [InverseProperty("TAccounts")]
    public virtual TAccountType? AccountTypeFkNavigation { get; set; }

    [InverseProperty("CompteFkNavigation")]
    public virtual ICollection<THistory> THistories { get; set; } = new List<THistory>();
}
