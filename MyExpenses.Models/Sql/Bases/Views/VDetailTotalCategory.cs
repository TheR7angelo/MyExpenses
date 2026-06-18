using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Bases.Views;

[Keyless]
public class VDetailTotalCategory
{
    [Column("year", TypeName = "INT")]
    public int? Year { get; init; }

    [Column("week", TypeName = "INT")]
    public int? Week { get; init; }

    [Column("month", TypeName = "INT")]
    public int? Month { get; init; }

    [Column("day", TypeName = "INT")]
    public int? Day { get; init; }

    [Column("account_id")]
    public int AccountId { get; init; }

    [Column("account_name")]
    [MaxLength(55)]
    public string? AccountName { get; init; }

    [Column("category_name")]
    [MaxLength(55)]
    public string? CategoryName { get; init; }

    [Column("value")]
    public double? Value { get; init; }

    [Column("symbol")]
    [MaxLength(55)]
    public string? Symbol { get; init; }

    [Column("hexadecimal_color_code", TypeName = "TEXT(9)")]
    [MaxLength(9)]
    public string? HexadecimalColorCode { get; init; }
}
