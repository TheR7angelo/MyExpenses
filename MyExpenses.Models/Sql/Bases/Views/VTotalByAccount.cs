using System.ComponentModel.DataAnnotations;
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
    [MaxLength(55)]
    public string? Name { get; set; }

    [Column("total")]
    public double? Total { get; set; }

    [Column("total_pointed")]
    public double? TotalPointed { get; init; }

    [Column("total_not_pointed")]
    public double? TotalNotPointed { get; init; }

    [Column("symbol")]
    [MaxLength(55)]
    public string? Symbol { get; set; }
}
