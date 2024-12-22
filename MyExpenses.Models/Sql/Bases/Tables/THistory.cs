using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PropertyChanged;

namespace MyExpenses.Models.Sql.Bases.Tables;

[AddINotifyPropertyChangedInterface]
[Table("t_history")]
public partial class THistory : ISql
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("account_fk")]
    public int? AccountFk { get; set; }

    [Required]
    [Column("description")]
    [MaxLength(255)]
    public string? Description { get; set; }

    [Required]
    [Column("category_type_fk")]
    public int? CategoryTypeFk { get; set; }

    [Required]
    [Column("mode_payment_fk")]
    public int? ModePaymentFk { get; set; }

    [Required]
    [Column("value")]
    public double? Value { get; set; }

    [Required]
    [Column("date", TypeName = "DATETIME")]
    public DateTime? Date { get; set; } = DateTime.Now;

    [Required]
    [Column("place_fk")]
    public int? PlaceFk { get; set; }

    [Column("is_pointed", TypeName = "BOOLEAN")]
    public bool IsPointed { get; set; }

    [Column("bank_transfer_fk")]
    public int? BankTransferFk { get; set; }

    [Column("recursive_expense_fk")]
    public int? RecursiveExpenseFk { get; set; }

    [Column("date_added", TypeName = "DATETIME")]
    public DateTime? DateAdded { get; set; } = DateTime.Now;

    [Column("date_pointed", TypeName = "DATETIME")]
    public DateTime? DatePointed { get; set; }

    [ForeignKey("AccountFk")]
    [InverseProperty("THistories")]
    public virtual TAccount? AccountFkNavigation { get; set; }

    [ForeignKey("BankTransferFk")]
    [InverseProperty("THistories")]
    public virtual TBankTransfer? BankTransferFkNavigation { get; set; }

    [ForeignKey("CategoryTypeFk")]
    [InverseProperty("THistories")]
    public virtual TCategoryType? CategoryTypeFkNavigation { get; set; }

    [ForeignKey("ModePaymentFk")]
    [InverseProperty("THistories")]
    public virtual TModePayment? ModePaymentFkNavigation { get; set; }

    [ForeignKey("PlaceFk")]
    [InverseProperty("THistories")]
    public virtual TPlace? PlaceFkNavigation { get; set; }

    [ForeignKey("RecursiveExpenseFk")]
    [InverseProperty("THistories")]
    public virtual TRecursiveExpense? RecursiveExpenseFkNavigation { get; set; }
}
