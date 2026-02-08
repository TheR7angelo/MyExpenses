using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Bases.Views;

[ObservableObject]
[Keyless]
public partial class VTotalByAccount : ISql
{
    [Column("id")]
    public int Id { get; set => SetProperty(ref field, value); }

    [Column("name")]
    [MaxLength(55)]
    public string? Name { get; set => SetProperty(ref field, value); }

    [Column("total")]
    public double? Total { get; set => SetProperty(ref field, value); }

    [Column("total_pointed")]
    public double? TotalPointed { get; init => SetProperty(ref field, value); }

    [Column("total_not_pointed")]
    public double? TotalNotPointed { get; init => SetProperty(ref field, value); }

    [Column("symbol")]
    [MaxLength(55)]
    public string? Symbol { get; set => SetProperty(ref field, value); }
}
