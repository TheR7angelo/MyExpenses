using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyExpenses.Models.Sql.Views;

[Keyless]
public partial class VAccountMonthlyCumulativeSum
{
    [Column("account_fk")]
    public int? AccountFk { get; set; }

    [Column("account")]
    public string? Account { get; set; }

    [Column("period")]
    public byte[]? Period { get; set; }

    [Column("cumulative_sum")]
    public byte[]? CumulativeSum { get; set; }
}
