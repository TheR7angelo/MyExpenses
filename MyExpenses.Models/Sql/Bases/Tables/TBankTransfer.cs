using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
    public string? MainReason { get; set; }

    [Column("additional_reason")]
    public string? AdditionalReason { get; set; }

    [Required]
    [Column("date", TypeName = "DATETIME")]
    public DateTime? Date { get; set; } = DateTime.Today;

    [Column("date_added", TypeName = "DATETIME")]
    public DateTime? DateAdded { get; set; } = DateTime.Now;

    [ForeignKey("FromAccountFk")]
    [InverseProperty("TBankTransferFromAccountFkNavigations")]
    public virtual TAccount? FromAccountFkNavigation { get; set; }

    [InverseProperty("BankTransferFkNavigation")]
    public virtual ICollection<THistory> THistories { get; set; } = new List<THistory>();

    [ForeignKey("ToAccountFk")]
    [InverseProperty("TBankTransferToAccountFkNavigations")]
    public virtual TAccount? ToAccountFkNavigation { get; set; }
}
