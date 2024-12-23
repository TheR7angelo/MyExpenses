using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Bases.Views;

[Keyless]
public partial class VHistory : ISql
{
    [Column("id")]
    public int Id { get; set; }

    [Column("account")]
    [MaxLength(55)]
    public string? Account { get; set; }

    [Column("description")]
    [MaxLength(255)]
    public string? Description { get; set; }

    [Column("category")]
    [MaxLength(55)]
    public string? Category { get; set; }

    [Column("hexadecimal_color_code", TypeName = "TEXT(9)")]
    [MaxLength(9)]
    public string? HexadecimalColorCode { get; set; }

    [Column("mode_payment")]
    [MaxLength(55)]
    public string? ModePayment { get; set; }

    [Column("value")]
    public double? Value { get; set; }

    [Column("symbol")]
    [MaxLength(55)]
    public string? Symbol { get; set; }

    [Column("date", TypeName = "DATETIME")]
    public DateTime? Date { get; set; }

    [Column("place")]
    [MaxLength(155)]
    public string? Place { get; set; }

    [Column("is_pointed", TypeName = "BOOLEAN")]
    public bool? IsPointed { get; set; }

    [Column("main_reason")]
    [MaxLength(100)]
    public string? MainReason { get; set; }

    [Column("date_added", TypeName = "DATETIME")]
    public DateTime? DateAdded { get; set; }
}
