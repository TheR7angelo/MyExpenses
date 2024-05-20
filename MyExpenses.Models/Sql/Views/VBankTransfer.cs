using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Views;

[Keyless]
public partial class VBankTransfer
{
    [Column("id")]
    public int? Id { get; set; }

    [Column("value")]
    public double? Value { get; set; }

    [Column("from_account")]
    public string? FromAccount { get; set; }

    [Column("to_account")]
    public string? ToAccount { get; set; }

    [Column("main_reason")]
    public string? MainReason { get; set; }

    [Column("additional_reason")]
    public int? AdditionalReason { get; set; }

    [Column("date", TypeName = "DATETIME")]
    public DateTime? Date { get; set; }

    [Column("date_added", TypeName = "DATETIME")]
    public DateTime? DateAdded { get; set; }
}
