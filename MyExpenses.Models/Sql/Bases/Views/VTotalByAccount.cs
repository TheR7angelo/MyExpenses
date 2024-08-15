using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using PropertyChanged;

namespace MyExpenses.Models.Sql.Bases.Views;

[AddINotifyPropertyChangedInterface]
[Keyless]
public partial class VTotalByAccount : ISql
{
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public string? Name { get; set; }

    [Column("total")]
    public double? Total { get; set; }

    [Column("total_pointed")]
    public double? TotalPointed { get; set; }

    [Column("total_not_pointed")]
    public double? TotalNotPointed { get; set; }

    [Column("symbol")]
    public string? Symbol { get; set; }
}
