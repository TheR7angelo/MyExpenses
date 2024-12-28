using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using MyExpenses.Models.Sql.Bases.Enums;

namespace MyExpenses.Models.Sql.Bases.Views;

[Keyless]
public class VRecursiveExpense : ISql
{
    [Column("id")]
    public int Id { get; set; }

    [Column("account_fk")]
    public int? AccountFk { get; init; }

    [Column("account")]
    [MaxLength(55)]
    public string? Account { get; init; }

    [Column("description")]
    [MaxLength(255)]
    public string? Description { get; init; }

    [Column("note")]
    [MaxLength(255)]
    public string? Note { get; init; }

    [Column("category_type_fk")]
    public int? CategoryTypeFk { get; init; }

    [Column("category")]
    [MaxLength(55)]
    public string? Category { get; init; }

    [Column("mode_payment_fk")]
    public int? ModePaymentFk { get; init; }

    [Column("mode_payment")]
    [MaxLength(55)]
    public string? ModePayment { get; init; }

    [Column("value")]
    public double? Value { get; init; }

    [Column("symbol")]
    [MaxLength(55)]
    public string? Symbol { get; init; }

    [Column("hexadecimal_color_code")]
    [MaxLength(9)]
    public string? HexadecimalColorCode { get; init; }

    [Column("place_fk")]
    public int? PlaceFk { get; init; }

    [Column("place")]
    [MaxLength(155)]
    public string? Place { get; init; }

    [Column("start_date", TypeName = "DATE")]
    public DateOnly StartDate { get; init; }

    [Column("recursive_total")]
    public int? RecursiveTotal { get; init; }

    [Column("recursive_count")]
    public int? RecursiveCount { get; init; }

    [Column("frequency_fk")]
    public int FrequencyFk { get; init; }

    [NotMapped]
    public ERecursiveFrequency ERecursiveFrequency
    {
        get => (ERecursiveFrequency)FrequencyFk;
        init => FrequencyFk = (int)value;
    }

    [Column("frequency")]
    [MaxLength(55)]
    public string? Frequency { get; init; }

    [Column("next_due_date", TypeName = "DATE")]
    public DateOnly NextDueDate { get; init; }

    [Column("is_active", TypeName = "BOOLEAN")]
    public bool IsActive { get; init; }

    [Column("force_deactivate", TypeName = "BOOLEAN")]
    public bool ForceDeactivate { get; init; }

    [Column("date_added", TypeName = "DATETIME")]
    public DateTime? DateAdded { get; init; }

    [Column("last_updated", TypeName = "DATE")]
    public DateTime? LastUpdated { get; init; }
}
