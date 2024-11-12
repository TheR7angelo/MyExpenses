using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Bases.Views;

[Keyless]
public partial class VBankTransferSummary : ISql
{
    [Column("id")]
    public int Id { get; set; }

    [Column("from_account_name")]
    public string? FromAccountName { get; set; }

    [Column("from_account_symbol")]
    public string? FromAccountSymbol { get; set; }

    [Column("to_account_name")]
    public string? ToAccountName { get; set; }

    [Column("to_account_symbol")]
    public string? ToAccountSymbol { get; set; }

    [Column("main_reason")]
    public string? MainReason { get; set; }

    [Column("additional_reason")]
    public string? AdditionalReason { get; set; }

    [Column("date", TypeName = "DATETIME")]
    public DateTime? Date { get; set; }

    [Column("from_account_balance_before")]
    public byte[]? FromAccountBalanceBefore { get; set; }

    [Column("from_account_balance_after")]
    public byte[]? FromAccountBalanceAfter { get; set; }

    [Column("value")]
    public byte[]? Value { get; set; }

    [Column("to_account_balance_before")]
    public byte[]? ToAccountBalanceBefore { get; set; }

    [Column("to_account_balance_after")]
    public byte[]? ToAccountBalanceAfter { get; set; }

    [Column("date_added", TypeName = "DATETIME")]
    public DateTime? DateAdded { get; set; }
}
