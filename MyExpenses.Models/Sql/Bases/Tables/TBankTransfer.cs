using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MyExpenses.Models.Attributs;
using PropertyChanged;

namespace MyExpenses.Models.Sql.Bases.Tables;

[AddINotifyPropertyChangedInterface]
[Table("t_bank_transfer")]
public partial class TBankTransfer : ISql
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("value")]
    public double? Value { get; set; }

    [Required]
    [Column("from_account_fk")]
    public int? FromAccountFk { get; set; }

    [Required]
    [Column("to_account_fk")]
    public int? ToAccountFk { get; set; }

    [Required]
    [Column("main_reason")]
    [MaxLength(100)]
    public string? MainReason { get; set; }

    [Column("additional_reason")]
    [MaxLength(255)]
    public string? AdditionalReason { get; set; }

    [Required]
    [IgnoreReset]
    [Column("date", TypeName = "DATETIME")]
    public DateTime? Date { get; set; }

    [IgnoreReset]
    [Column("date_added", TypeName = "DATETIME")]
    public DateTime? DateAdded { get; set; }

    // ReSharper disable PropertyCanBeMadeInitOnly.Global
    [ForeignKey("FromAccountFk")]
    [InverseProperty("TBankTransferFromAccountFkNavigations")]
    public virtual TAccount? FromAccountFkNavigation { get; set; }

    [InverseProperty("BankTransferFkNavigation")]
    public virtual ICollection<THistory>? THistories { get; set; }

    [ForeignKey("ToAccountFk")]
    [InverseProperty("TBankTransferToAccountFkNavigations")]
    public virtual TAccount? ToAccountFkNavigation { get; set; }
    // ReSharper restore PropertyCanBeMadeInitOnly.Global
}
