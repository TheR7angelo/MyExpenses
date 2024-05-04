using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Views;

[Keyless]
public partial class VTotalByAccount
{
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public string? Name { get; set; }

    [Column("total")]
    public double? Total { get; set; }

    [Column("symbol")]
    public string? Symbol { get; set; }
}
