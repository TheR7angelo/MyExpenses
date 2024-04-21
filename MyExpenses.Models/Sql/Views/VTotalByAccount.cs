using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Views;

[Keyless]
public partial class VTotalByAccount
{
    [Column("name")]
    public string? Name { get; set; }

    [Column("total")]
    public byte[]? Total { get; set; }
}
