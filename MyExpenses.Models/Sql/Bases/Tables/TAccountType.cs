using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PropertyChanged;

namespace MyExpenses.Models.Sql.Bases.Tables;

[AddINotifyPropertyChangedInterface]
[Table("t_account_type")]
public partial class TAccountType : ISql
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [MaxLength(100)]
    public string? Name { get; set; }

    [Column("date_added", TypeName = "DATETIME")]
    public DateTime? DateAdded { get; set; } = DateTime.Now;

    [InverseProperty("AccountTypeFkNavigation")]
    public virtual ICollection<TAccount> TAccounts { get; set; } = new List<TAccount>();
}
