using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Bases.Views;

[Keyless]
public class VDetailTotalCategory
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
    [MaxLength(55)]
    public string? Account { get; set; }

    [Column("category")]
    [MaxLength(55)]
    public string? Category { get; set; }

    [Column("value")]
    public double? Value { get; set; }

    [Column("symbol")]
    [MaxLength(55)]
    public string? Symbol { get; set; }

    [Column("hexadecimal_color_code", TypeName = "TEXT(9)")]
    [MaxLength(9)]
    public string? HexadecimalColorCode { get; set; }
}
