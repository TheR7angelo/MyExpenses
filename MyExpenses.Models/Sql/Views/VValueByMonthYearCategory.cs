using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Views;

[Keyless]
public partial class VValueByMonthYearCategory
{
    [Column("month_year")]
    public string? MonthYear { get; set; }

    [Column("name")]
    public string? Name { get; set; }

    [Column("total")]
    public double? Total { get; set; }
}
