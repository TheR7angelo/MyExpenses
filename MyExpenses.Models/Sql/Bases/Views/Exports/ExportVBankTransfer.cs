using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Bases.Views.Exports;

[Keyless]
public class ExportVBankTransfer
{
    [Column("id")]
    public int? Id { get; init; }

    [Column("value")]
    public double? Value { get; init; }

    [Column("from_account_name")]
    [MaxLength(55)]
    public string? FromAccountName { get; init; }

    [Column("to_account_name")]
    [MaxLength(55)]
    public string? ToAccountName { get; init; }

    [Column("main_reason")]
    [MaxLength(100)]
    public string? MainReason { get; init; }

    [Column("additional_reason")]
    [MaxLength(255)]
    public string? AdditionalReason { get; init; }

    [Column("date", TypeName = "DATETIME")]
    public DateTime? Date { get; init; }

    [Column("date_added", TypeName = "DATETIME")]
    public DateTime? DateAdded { get; init; }
}
