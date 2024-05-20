using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyExpenses.Models.Sql.Tables;

[Table("t_bank_transfer")]
public partial class TBankTransfer : ISql
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("value")]
    public double? Value { get; set; }

    [Column("from_account_fk")]
    public int? FromAccountFk { get; set; }

    [Column("to_account")]
    public int? ToAccount { get; set; }

    [Column("main_reason")]
    public string? MainReason { get; set; }

    [Column("additional_reason")]
    public int? AdditionalReason { get; set; }

    [Column("date", TypeName = "DATETIME")]
    public DateTime? Date { get; set; }

    [Column("date_added", TypeName = "DATETIME")]
    public DateTime? DateAdded { get; set; } = DateTime.Now;

    [ForeignKey("FromAccountFk")]
    [InverseProperty("TBankTransferFromAccountFkNavigations")]
    public virtual TAccount? FromAccountFkNavigation { get; set; }

    [InverseProperty("BankTransferFkNavigation")]
    public virtual ICollection<THistory> THistories { get; set; } = new List<THistory>();

    [ForeignKey("ToAccount")]
    [InverseProperty("TBankTransferToAccountNavigations")]
    public virtual TAccount? ToAccountNavigation { get; set; }
}
