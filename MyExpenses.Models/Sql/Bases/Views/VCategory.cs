using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Bases.Views;

[Keyless]
public partial class VCategory
{
    [Column("id")]
    public int? Id { get; set; }

    [Column("category_name")]
    public string? CategoryName { get; set; }

    [Column("color_fk")]
    public int? ColorFk { get; set; }

    [Column("date_category_added", TypeName = "DATETIME")]
    public DateTime? DateCategoryAdded { get; set; }

    [Column("color_name")]
    public string? ColorName { get; set; }

    [Column("hexadecimal_color_code", TypeName = "TEXT(9)")]
    public string? HexadecimalColorCode { get; set; }

    [Column("date_color_added", TypeName = "DATETIME")]
    public DateTime? DateColorAdded { get; set; }
}
