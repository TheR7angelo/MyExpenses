using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Views;

[Keyless]
public partial class VDetailTotalCategory
{
    [Column("year", TypeName = "INT")]
    public int? Year { get; set; }

    [Column("week", TypeName = "INT")]
    public int? Week { get; set; }

    [Column("month", TypeName = "INT")]
    public int? Month { get; set; }

    [Column("day", TypeName = "INT")]
    public int? Day { get; set; }

    [Column("account")]
    public string? Account { get; set; }

    [Column("category")]
    public string? Category { get; set; }

    [Column("value")]
    public double? Value { get; set; }
}
