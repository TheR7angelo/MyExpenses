using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Bases.Views;

[Keyless]
public partial class VTotalByAccount : ObservableValidator, ISql
{
    [Column("id")]
    public int Id { get; set; }

    [ObservableProperty]
    [Column("name")]
    [MaxLength(55)]
    public partial string Name { get; set; } = string.Empty;

    [Column("total")]
    public double Total { get; set; }

    [Column("total_pointed")]
    public double TotalPointed { get; init; }

    [Column("total_not_pointed")]
    public double TotalNotPointed { get; init; }

    [Column("symbol")]
    [MaxLength(55)]
    public string Symbol { get; set; }
}
