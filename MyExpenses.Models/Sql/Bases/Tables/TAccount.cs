using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MyExpenses.Models.Attributs;
using PropertyChanged;

namespace MyExpenses.Models.Sql.Bases.Tables;

[AddINotifyPropertyChangedInterface]
[Table("t_account")]
public partial class TAccount : ISql
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("name")]
    [MaxLength(55)]
    public string? Name { get; set; }

    [Required]
    [Column("account_type_fk")]
    public int? AccountTypeFk { get; set; }

    [Required]
    [Column("currency_fk")]
    public int? CurrencyFk { get; set; }

    [IgnoreReset]
    [Required]
    [Column("active", TypeName = "BOOLEAN")]
    public bool? Active { get; set; }

    [IgnoreReset]
    [Column("date_added", TypeName = "DATETIME")]
    public DateTime? DateAdded { get; set; }

    // ReSharper disable PropertyCanBeMadeInitOnly.Global
    [ForeignKey("AccountTypeFk")]
    [InverseProperty("TAccounts")]
    public virtual TAccountType? AccountTypeFkNavigation { get; set; }

    [ForeignKey("CurrencyFk")]
    [InverseProperty("TAccounts")]
    public virtual TCurrency? CurrencyFkNavigation { get; set; }

    [InverseProperty("FromAccountFkNavigation")]
    public virtual ICollection<TBankTransfer> TBankTransferFromAccountFkNavigations { get; set; }

    [InverseProperty("ToAccountFkNavigation")]
    public virtual ICollection<TBankTransfer> TBankTransferToAccountFkNavigations { get; set; }

    [InverseProperty("AccountFkNavigation")]
    public virtual ICollection<THistory> THistories { get; set; } = new List<THistory>();

    [InverseProperty("AccountFkNavigation")]
    public virtual ICollection<TRecursiveExpense> TRecursiveExpenses { get; set; }
    // ReSharper restore PropertyCanBeMadeInitOnly.Global
}
