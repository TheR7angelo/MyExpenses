using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Bases.Views.Exports;

[Keyless]
public partial class ExportVCategoryType
{
    [Column("id")]
    public int? Id { get; set; }

    [Column("name")]
    [MaxLength(55)]
    public string? Name { get; set; }

    [Column("color_name")]
    [MaxLength(55)]
    public string? ColorName { get; set; }

    [Column("date_added", TypeName = "DATETIME")]
    public DateTime? DateAdded { get; set; }
}
