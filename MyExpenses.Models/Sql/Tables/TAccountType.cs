using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyExpenses.Models.Sql.Tables;

[Table("t_account_type")]
public partial class TAccountType
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public string? Name { get; set; }

    [InverseProperty("AccountTypeFkNavigation")]
    public virtual ICollection<TAccount> TAccounts { get; set; } = new List<TAccount>();
}
