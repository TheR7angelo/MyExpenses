using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Views;

[Keyless]
public partial class VTotalByAccount
{
    [Column("name")]
    public string? Name { get; set; }

    [Column("total")]
    public double? Total { get; set; }

    [Column("currency")]
    public string? Currency { get; set; }
}
