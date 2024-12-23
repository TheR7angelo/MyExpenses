using System.ComponentModel.DataAnnotations;
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
    [MaxLength(55)]
    public string? FromAccountName { get; set; }

    [Column("to_account_name")]
    [MaxLength(55)]
    public string? ToAccountName { get; set; }

    [Column("main_reason")]
    [MaxLength(100)]
    public string? MainReason { get; set; }

    [Column("additional_reason")]
    [MaxLength(255)]
    public string? AdditionalReason { get; set; }

    [Column("date", TypeName = "DATETIME")]
    public DateTime? Date { get; set; }

    [Column("date_added", TypeName = "DATETIME")]
    public DateTime? DateAdded { get; set; }
}
