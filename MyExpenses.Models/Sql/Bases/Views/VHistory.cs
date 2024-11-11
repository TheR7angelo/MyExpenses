using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Bases.Views;

[Keyless]
public partial class VHistory : ISql
{
    [Column("id")]
    public int Id { get; set; }

    [Column("account")]
    public string? Account { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("category")]
    public string? Category { get; set; }

    [Column("hexadecimal_color_code", TypeName = "TEXT(9)")]
    public string? HexadecimalColorCode { get; set; }

    [Column("mode_payment")]
    public string? ModePayment { get; set; }

    [Column("value")]
    public double? Value { get; set; }

    [Column("symbol")]
    public string? Symbol { get; set; }

    [Column("date", TypeName = "DATETIME")]
    public DateTime? Date { get; set; }

    [Column("place")]
    public string? Place { get; set; }

    [Column("is_pointed", TypeName = "BOOLEAN")]
    public bool? IsPointed { get; set; }

    [Column("main_reason")]
    public string? MainReason { get; set; }

    [Column("date_added", TypeName = "DATETIME")]
    public DateTime? DateAdded { get; set; }
}
