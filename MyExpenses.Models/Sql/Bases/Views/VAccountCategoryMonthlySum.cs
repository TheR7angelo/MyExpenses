using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Bases.Views;

[Keyless]
public partial class VAccountCategoryMonthlySum
{
    [Column("account_fk")]
    public int? AccountFk { get; set; }

    [Column("account")]
    public string? Account { get; set; }

    [Column("category_type")]
    public string? CategoryType { get; set; }

    [Column("color_code")]
    public string? ColorCode { get; set; }

    [Column("period")]
    public string? Period { get; set; }

    [Column("monthly_sum")]
    public double? MonthlySum { get; set; }
}
