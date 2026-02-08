using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CommunityToolkit.Mvvm.ComponentModel;
using MyExpenses.Models.Attributs;

namespace MyExpenses.Models.Sql.Bases.Tables;

[ObservableObject]
[Table("t_history")]
public partial class THistory : ISql
{
    [Key]
    [Column("id")]
    public int Id { get; set => SetProperty(ref field, value); }

    [Required]
    [Column("account_fk")]
    public int? AccountFk { get; set => SetProperty(ref field, value); }

    [Required]
    [Column("description")]
    [MaxLength(255)]
    public string? Description { get; set => SetProperty(ref field, value); }

    [Required]
    [Column("category_type_fk")]
    public int? CategoryTypeFk { get; set => SetProperty(ref field, value); }

    [Required]
    [Column("mode_payment_fk")]
    public int? ModePaymentFk { get; set => SetProperty(ref field, value); }

    [Required]
    [Column("value")]
    public double? Value { get; set => SetProperty(ref field, value); }

    [IgnoreReset]
    [Required]
    [Column("date", TypeName = "DATETIME")]
    public DateTime? Date { get; set => SetProperty(ref field, value); }

    [Required]
    [Column("place_fk")]
    public int? PlaceFk { get; set => SetProperty(ref field, value); }

    [IgnoreReset]
    [Column("is_pointed", TypeName = "BOOLEAN")]
    public bool IsPointed { get; set => SetProperty(ref field, value); }

    [Column("bank_transfer_fk")]
    public int? BankTransferFk { get; init => SetProperty(ref field, value); }

    [Column("recursive_expense_fk")]
    public int? RecursiveExpenseFk { get; init => SetProperty(ref field, value); }

    [IgnoreReset]
    [Column("date_added", TypeName = "DATETIME")]
    public DateTime? DateAdded { get; set => SetProperty(ref field, value); }

    [Column("date_pointed", TypeName = "DATETIME")]
    public DateTime? DatePointed { get; set => SetProperty(ref field, value); }

    // ReSharper disable PropertyCanBeMadeInitOnly.Global
    [ForeignKey("AccountFk")]
    [InverseProperty("THistories")]
    public virtual TAccount? AccountFkNavigation { get; set => SetProperty(ref field, value); }

    [ForeignKey("BankTransferFk")]
    [InverseProperty("THistories")]
    public virtual TBankTransfer? BankTransferFkNavigation { get; set => SetProperty(ref field, value); }

    [ForeignKey("CategoryTypeFk")]
    [InverseProperty("THistories")]
    public virtual TCategoryType? CategoryTypeFkNavigation { get; set => SetProperty(ref field, value); }

    [ForeignKey("ModePaymentFk")]
    [InverseProperty("THistories")]
    public virtual TModePayment? ModePaymentFkNavigation { get; set => SetProperty(ref field, value); }

    [ForeignKey("PlaceFk")]
    [InverseProperty("THistories")]
    public virtual TPlace? PlaceFkNavigation { get; set => SetProperty(ref field, value); }

    [ForeignKey("RecursiveExpenseFk")]
    [InverseProperty("THistories")]
    public virtual TRecursiveExpense? RecursiveExpenseFkNavigation { get; set => SetProperty(ref field, value); }
    // ReSharper restore PropertyCanBeMadeInitOnly.Global
}
