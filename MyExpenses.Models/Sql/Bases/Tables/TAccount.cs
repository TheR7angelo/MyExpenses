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

    [Required]
    [Column("active", TypeName = "BOOLEAN")]
    public bool? Active { get; set; } = true;

    [Column("date_added", TypeName = "DATETIME")]
    public DateTime? DateAdded { get; set; } = DateTime.Now;

    // ReSharper disable PropertyCanBeMadeInitOnly.Global
    [ForeignKey("AccountTypeFk")]
    [InverseProperty("TAccounts")]
    public virtual TAccountType? AccountTypeFkNavigation { get; set; }

    [ForeignKey("CurrencyFk")]
    [InverseProperty("TAccounts")]
    public virtual TCurrency? CurrencyFkNavigation { get; set; }

    // Each ICollection property is initialized to prevent null references
    // and to ensure the collections are ready for use, even if no data is loaded from the database.
    // ReSharper disable HeapView.ObjectAllocation.Evident
    [InverseProperty("FromAccountFkNavigation")]
    public virtual ICollection<TBankTransfer> TBankTransferFromAccountFkNavigations { get; set; } = new List<TBankTransfer>();

    [InverseProperty("ToAccountFkNavigation")]
    public virtual ICollection<TBankTransfer> TBankTransferToAccountFkNavigations { get; set; } = new List<TBankTransfer>();

    [InverseProperty("AccountFkNavigation")]
    public virtual ICollection<THistory> THistories { get; set; } = new List<THistory>();

    [InverseProperty("AccountFkNavigation")]
    public virtual ICollection<TRecursiveExpense> TRecursiveExpenses { get; set; } = new List<TRecursiveExpense>();
    // ReSharper restore PropertyCanBeMadeInitOnly.Global
    // ReSharper restore HeapView.ObjectAllocation.Evident
}
