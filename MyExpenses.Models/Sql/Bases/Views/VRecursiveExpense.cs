using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using MyExpenses.Models.Sql.Bases.Enums;

namespace MyExpenses.Models.Sql.Bases.Views;

[Keyless]
public partial class VRecursiveExpense : ISql
{
    [Column("id")]
    public int Id { get; set; }

    [Column("account_fk")]
    public int? AccountFk { get; set; }

    [Column("account")]
    public string? Account { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("note")]
    public string? Note { get; set; }

    [Column("category_type_fk")]
    public int? CategoryTypeFk { get; set; }

    [Column("category")]
    public string? Category { get; set; }

    [Column("mode_payment_fk")]
    public int? ModePaymentFk { get; set; }

    [Column("mode_payment")]
    public string? ModePayment { get; set; }

    [Column("value")]
    public double? Value { get; set; }

    [Column("symbol")]
    public string? Symbol { get; set; }

    [Column("hexadecimal_color_code")]
    public string? HexadecimalColorCode { get; set; }

    [Column("place_fk")]
    public int? PlaceFk { get; set; }

    [Column("place")]
    public string? Place { get; set; }

    [Column("start_date", TypeName = "DATE")]
    public DateOnly? StartDate { get; set; }

    [Column("recursive_total")]
    public int? RecursiveTotal { get; set; }

    [Column("recursive_count")]
    public int? RecursiveCount { get; set; }

    [Column("frequency_fk")]
    public int FrequencyFk { get; set; }

    [NotMapped]
    public ERecursiveFrequency ERecursiveFrequency
    {
        get => (ERecursiveFrequency)FrequencyFk;
        set => FrequencyFk = (int)value;
    }

    [Column("frequency")]
    public string? Frequency { get; set; }

    [Column("next_due_date", TypeName = "DATE")]
    public DateOnly NextDueDate { get; set; }

    [Column("is_active", TypeName = "BOOLEAN")]
    public bool IsActive { get; set; }

    [Column("date_added", TypeName = "DATETIME")]
    public DateTime? DateAdded { get; set; }

    [Column("last_updated", TypeName = "DATE")]
    public DateTime? LastUpdated { get; set; }
}
