using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CommunityToolkit.Mvvm.ComponentModel;
using MyExpenses.Models.Attributs;

namespace MyExpenses.Models.Sql.Bases.Tables;

[ObservableObject]
[Table("t_bank_transfer")]
public partial class TBankTransfer : ISql
{
    [Key]
    [Column("id")]
    public int Id { get; set => SetProperty(ref field, value); }

    [Required]
    [Column("value")]
    public double? Value { get; set => SetProperty(ref field, value); }

    [Required]
    [Column("from_account_fk")]
    public int? FromAccountFk { get; set => SetProperty(ref field, value); }

    [Required]
    [Column("to_account_fk")]
    public int? ToAccountFk { get; set => SetProperty(ref field, value); }

    [Required]
    [Column("main_reason")]
    [MaxLength(100)]
    public string? MainReason { get; set => SetProperty(ref field, value); }

    [Column("additional_reason")]
    [MaxLength(255)]
    public string? AdditionalReason { get; set => SetProperty(ref field, value); }

    [Required]
    [IgnoreReset]
    [Column("date", TypeName = "DATETIME")]
    public DateTime? Date { get; set => SetProperty(ref field, value); }

    [IgnoreReset]
    [Column("date_added", TypeName = "DATETIME")]
    public DateTime? DateAdded { get; set => SetProperty(ref field, value); }

    // ReSharper disable PropertyCanBeMadeInitOnly.Global
    [ForeignKey("FromAccountFk")]
    [InverseProperty("TBankTransferFromAccountFkNavigations")]
    public virtual TAccount? FromAccountFkNavigation { get; set => SetProperty(ref field, value); }

    [InverseProperty("BankTransferFkNavigation")]
    public virtual ICollection<THistory>? THistories { get; set => SetProperty(ref field, value); }

    [ForeignKey("ToAccountFk")]
    [InverseProperty("TBankTransferToAccountFkNavigations")]
    public virtual TAccount? ToAccountFkNavigation { get; set => SetProperty(ref field, value); }
    // ReSharper restore PropertyCanBeMadeInitOnly.Global
}
