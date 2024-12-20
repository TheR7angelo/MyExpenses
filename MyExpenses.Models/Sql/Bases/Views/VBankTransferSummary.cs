﻿using System.ComponentModel.DataAnnotations.Schema;
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

    [Column("category_name")]
    public string? CategoryName { get; set; }

    [Column("category_color")]
    public string? CategoryColor { get; set; }

    [Column("mode_payment")]
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
