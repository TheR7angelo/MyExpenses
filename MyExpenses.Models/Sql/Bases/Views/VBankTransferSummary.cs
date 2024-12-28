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
    public string? FromAccountName { get; init; }

    [Column("from_account_symbol")]
    [MaxLength(55)]
    public string? FromAccountSymbol { get; init; }

    [Column("to_account_name")]
    [MaxLength(55)]
    public string? ToAccountName { get; init; }

    [Column("to_account_symbol")]
    [MaxLength(55)]
    public string? ToAccountSymbol { get; init; }

    [Column("main_reason")]
    [MaxLength(100)]
    public string? MainReason { get; init; }

    [Column("additional_reason")]
    [MaxLength(255)]
    public string? AdditionalReason { get; init; }

    [Column("category_name")]
    [MaxLength(55)]
    public string? CategoryName { get; init; }

    [Column("category_color")]
    [MaxLength(9)]
    public string? CategoryColor { get; init; }

    [Column("mode_payment")]
    [MaxLength(55)]
    public string? ModePayment { get; init; }

    [Column("date", TypeName = "DATETIME")]
    public DateTime? Date { get; init; }

    [Column("from_account_balance_before")]
    public double? FromAccountBalanceBefore { get; init; }

    [Column("from_account_balance_after")]
    public double? FromAccountBalanceAfter { get; init; }

    [Column("value")]
    public double? Value { get; init; }

    [Column("to_account_balance_before")]
    public double? ToAccountBalanceBefore { get; init; }

    [Column("to_account_balance_after")]
    public double? ToAccountBalanceAfter { get; init; }

    [Column("date_added", TypeName = "DATETIME")]
    public DateTime? DateAdded { get; init; }
}
