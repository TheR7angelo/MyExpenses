using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CommunityToolkit.Mvvm.ComponentModel;
using MyExpenses.Models.Attributs;

namespace MyExpenses.Models.Sql.Bases.Tables;

[ObservableObject]
[Table("t_account")]
public partial class TAccount : ISql
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("name")]
    [MaxLength(55)]
    public string? Name { get; set => SetProperty(ref field, value); }

    [Required]
    [Column("account_type_fk")]
    public int? AccountTypeFk { get; set => SetProperty(ref field, value); }

    [Required]
    [Column("currency_fk")]
    public int? CurrencyFk { get; set => SetProperty(ref field, value); }

    [IgnoreReset]
    [Required]
    [Column("active", TypeName = "BOOLEAN")]
    public bool? Active { get; set => SetProperty(ref field, value); }

    [IgnoreReset]
    [Column("date_added", TypeName = "DATETIME")]
    public DateTime? DateAdded { get; set => SetProperty(ref field, value); }

    // ReSharper disable PropertyCanBeMadeInitOnly.Global
    [ForeignKey("AccountTypeFk")]
    [InverseProperty("TAccounts")]
    public virtual TAccountType? AccountTypeFkNavigation { get; set => SetProperty(ref field, value); }

    [ForeignKey("CurrencyFk")]
    [InverseProperty("TAccounts")]
    public virtual TCurrency? CurrencyFkNavigation { get; set => SetProperty(ref field, value); }

    [InverseProperty("FromAccountFkNavigation")]
    public virtual ICollection<TBankTransfer> TBankTransferFromAccountFkNavigations { get; set; }

    [InverseProperty("ToAccountFkNavigation")]
    public virtual ICollection<TBankTransfer> TBankTransferToAccountFkNavigations { get; set; }

    [InverseProperty("AccountFkNavigation")]
    public virtual ICollection<THistory> THistories { get; set; }

    [InverseProperty("AccountFkNavigation")]
    public virtual ICollection<TRecursiveExpense> TRecursiveExpenses { get; set; }
    // ReSharper restore PropertyCanBeMadeInitOnly.Global
}
