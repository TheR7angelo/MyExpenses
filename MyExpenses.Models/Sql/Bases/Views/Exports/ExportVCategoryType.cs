using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Bases.Views.Exports;

[Keyless]
public partial class ExportVCategoryType
{
    [Column("id")]
    public int? Id { get; set; }

    [Column("name")]
    public string? Name { get; set; }

    [Column("color_name")]
    public string? ColorName { get; set; }

    [Column("date_added", TypeName = "DATETIME")]
    public DateTime? DateAdded { get; set; }
}
