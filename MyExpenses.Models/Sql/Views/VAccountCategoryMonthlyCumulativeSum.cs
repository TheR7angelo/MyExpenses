using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Views;

[Keyless]
public partial class VAccountCategoryMonthlyCumulativeSum
{
    [Column("account_fk")]
    public int? AccountFk { get; set; }

    [Column("account")]
    public string? Account { get; set; }

    [Column("category_type")]
    public string? CategoryType { get; set; }

    [Column("period")]
    public string? Period { get; set; }

    [Column("cumulative_sum")]
    public double? CumulativeSum { get; set; }
}
