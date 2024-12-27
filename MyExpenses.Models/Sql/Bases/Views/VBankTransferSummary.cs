using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Bases.Views;

[Keyless]
public class VBankTransferSummary : ISql
{
    [Column("id")]
    public int Id { get; set; }

    [Column("from_account_name")]
    [MaxLength(55)]
    public string? FromAccountName { get; set; }

    [Column("from_account_symbol")]
    [MaxLength(55)]
    public string? FromAccountSymbol { get; set; }

    [Column("to_account_name")]
    [MaxLength(55)]
    public string? ToAccountName { get; set; }

    [Column("to_account_symbol")]
    [MaxLength(55)]
    public string? ToAccountSymbol { get; set; }

    [Column("main_reason")]
    [MaxLength(100)]
    public string? MainReason { get; set; }

    [Column("additional_reason")]
    [MaxLength(255)]
    public string? AdditionalReason { get; set; }

    [Column("category_name")]
    [MaxLength(55)]
    public string? CategoryName { get; set; }

    [Column("category_color")]
    [MaxLength(9)]
    public string? CategoryColor { get; set; }

    [Column("mode_payment")]
    [MaxLength(55)]
    public string? ModePayment { get; set; }

    [Column("date", TypeName = "DATETIME")]
    public DateTime? Date { get; set; }

    [Column("from_account_balance_before")]
    public double? FromAccountBalanceBefore { get; set; }

    [Column("from_account_balance_after")]
    public double? FromAccountBalanceAfter { get; set; }

    [Column("value")]
    public double? Value { get; set; }

    [Column("to_account_balance_before")]
    public double? ToAccountBalanceBefore { get; set; }

    [Column("to_account_balance_after")]
    public double? ToAccountBalanceAfter { get; set; }

    [Column("date_added", TypeName = "DATETIME")]
    public DateTime? DateAdded { get; set; }
}
