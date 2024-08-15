using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PropertyChanged;

namespace MyExpenses.Models.Sql.Bases.Tables;

[AddINotifyPropertyChangedInterface]
[Table("t_account")]
public partial class TAccount : ISql
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public string? Name { get; set; }

    [Column("account_type_fk")]
    public int? AccountTypeFk { get; set; }

    [Column("currency_fk")]
    public int? CurrencyFk { get; set; }

    [Column("active", TypeName = "BOOLEAN")]
    public bool? Active { get; set; } = true;

    [Column("date_added", TypeName = "DATETIME")]
    public DateTime? DateAdded { get; set; } = DateTime.Now;

    [ForeignKey("AccountTypeFk")]
    [InverseProperty("TAccounts")]
    public virtual TAccountType? AccountTypeFkNavigation { get; set; }

    [ForeignKey("CurrencyFk")]
    [InverseProperty("TAccounts")]
    public virtual TCurrency? CurrencyFkNavigation { get; set; }

    [InverseProperty("FromAccountFkNavigation")]
    public virtual ICollection<TBankTransfer> TBankTransferFromAccountFkNavigations { get; set; } = new List<TBankTransfer>();

    [InverseProperty("ToAccountFkNavigation")]
    public virtual ICollection<TBankTransfer> TBankTransferToAccountFkNavigations { get; set; } = new List<TBankTransfer>();

    [InverseProperty("AccountFkNavigation")]
    public virtual ICollection<THistory> THistories { get; set; } = new List<THistory>();

    [InverseProperty("AccountFkNavigation")]
    public virtual ICollection<TRecursiveExpense> TRecursiveExpenses { get; set; } = new List<TRecursiveExpense>();
}
