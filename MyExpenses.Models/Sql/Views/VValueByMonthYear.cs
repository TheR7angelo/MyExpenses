using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Views;

[Keyless]
public partial class VValueByMonthYear
{
    [Column("month_year")]
    public string? MonthYear { get; set; }

    [Column("total")]
    public double? Total { get; set; }
}
