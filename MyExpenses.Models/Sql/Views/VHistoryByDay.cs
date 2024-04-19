using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Views;

[Keyless]
public partial class VHistoryByDay
{
    [Column("account")]
    public string? Account { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("category")]
    public string? Category { get; set; }

    [Column("mode_payment")]
    public string? ModePayment { get; set; }

    [Column("value")]
    public double? Value { get; set; }

    [Column("date", TypeName = "DATETIME")]
    public DateTime? Date { get; set; }

    [Column("place")]
    public string? Place { get; set; }

    [Column("pointed", TypeName = "BOOLEAN")]
    public bool? Pointed { get; set; }
}
