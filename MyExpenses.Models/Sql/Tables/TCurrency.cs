using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyExpenses.Models.Sql.Tables;

[Table("t_currency")]
public partial class TCurrency
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("currency")]
    public string? Currency { get; set; }

    [InverseProperty("CurrencyFkNavigation")]
    public virtual ICollection<TAccount> TAccounts { get; set; } = new List<TAccount>();
}
