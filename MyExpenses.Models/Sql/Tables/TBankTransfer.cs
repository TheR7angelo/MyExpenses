using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyExpenses.Models.Sql.Tables;

[Table("t_bank_transfer")]
public partial class TBankTransfer
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("value")]
    public double? Value { get; set; }

    [Column("from_account_fk")]
    public int? FromAccountFk { get; set; }

    [Column("from_history_fk")]
    public int? FromHistoryFk { get; set; }

    [Column("to_account")]
    public int? ToAccount { get; set; }

    [Column("to_history")]
    public int? ToHistory { get; set; }

    [Column("main_reason")]
    public string? MainReason { get; set; }

    [Column("additional_reason")]
    public int? AdditionalReason { get; set; }

    [Column("date", TypeName = "DATETIME")]
    public DateTime? Date { get; set; }

    [Column("date_added", TypeName = "DATETIME")]
    public DateTime? DateAdded { get; set; }

    [ForeignKey("FromAccountFk")]
    [InverseProperty("TBankTransferFromAccountFkNavigations")]
    public virtual TAccount? FromAccountFkNavigation { get; set; }

    [ForeignKey("FromHistoryFk")]
    [InverseProperty("TBankTransferFromHistoryFkNavigations")]
    public virtual THistory? FromHistoryFkNavigation { get; set; }

    [ForeignKey("ToAccount")]
    [InverseProperty("TBankTransferToAccountNavigations")]
    public virtual TAccount? ToAccountNavigation { get; set; }

    [ForeignKey("ToHistory")]
    [InverseProperty("TBankTransferToHistoryNavigations")]
    public virtual THistory? ToHistoryNavigation { get; set; }
}
