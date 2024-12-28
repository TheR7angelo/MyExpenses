using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Bases.Views;

[Keyless]
public class VHistory : ISql
{
    [Column("id")]
    public int Id { get; set; }

    [Column("account")]
    [MaxLength(55)]
    public string? Account { get; init; }

    [Column("description")]
    [MaxLength(255)]
    public string? Description { get; init; }

    [Column("category")]
    [MaxLength(55)]
    public string? Category { get; init; }

    [Column("hexadecimal_color_code", TypeName = "TEXT(9)")]
    [MaxLength(9)]
    public string? HexadecimalColorCode { get; init; }

    [Column("mode_payment")]
    [MaxLength(55)]
    public string? ModePayment { get; init; }

    [Column("value")]
    public double? Value { get; init; }

    [Column("symbol")]
    [MaxLength(55)]
    public string? Symbol { get; init; }

    [Column("date", TypeName = "DATETIME")]
    public DateTime? Date { get; init; }

    [Column("place")]
    [MaxLength(155)]
    public string? Place { get; init; }

    [Column("is_pointed", TypeName = "BOOLEAN")]
    public bool? IsPointed { get; init; }

    [Column("main_reason")]
    [MaxLength(100)]
    public string? MainReason { get; init; }

    [Column("date_added", TypeName = "DATETIME")]
    public DateTime? DateAdded { get; init; }
}
