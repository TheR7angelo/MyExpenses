using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Bases.Views.Exports;

[Keyless]
public partial class ExportVBankTransfer
{
    [Column("id")]
    public int? Id { get; set; }

    [Column("value")]
    public double? Value { get; set; }

    [Column("from_account_name")]
    public string? FromAccountName { get; set; }

    [Column("to_account_name")]
    public string? ToAccountName { get; set; }

    [Column("main_reason")]
    public string? MainReason { get; set; }

    [Column("additional_reason")]
    public string? AdditionalReason { get; set; }

    [Column("date", TypeName = "DATETIME")]
    public DateTime? Date { get; set; }

    [Column("date_added", TypeName = "DATETIME")]
    public DateTime? DateAdded { get; set; }
}
