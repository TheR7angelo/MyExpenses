using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyExpenses.Models.Sql.Tables;

[Table("t_recursive_expense")]
public partial class TRecursiveExpense
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("account_fk")]
    public int? AccountFk { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("category_type_fk")]
    public int? CategoryTypeFk { get; set; }

    [Column("mode_payment_fk")]
    public int? ModePaymentFk { get; set; }

    [Column("value")]
    public double? Value { get; set; }

    [Column("place_fk")]
    public int? PlaceFk { get; set; }

    [Column("start_date", TypeName = "DATE")]
    public DateTime StartDate { get; set; }

    [Column("recursive_total")]
    public int? RecursiveTotal { get; set; }

    [Column("recursive_count")]
    public int RecursiveCount { get; set; }

    [Column("frequency_fk")]
    public int FrequencyFk { get; set; }

    [Column("next_due_date", TypeName = "DATE")]
    public DateTime NextDueDate { get; set; }

    [Column("is_active", TypeName = "BOOLEAN")]
    public bool? IsActive { get; set; }

    [Column("date_added", TypeName = "DATE")]
    public DateTime? DateAdded { get; set; } = DateTime.Now;

    [ForeignKey("AccountFk")]
    [InverseProperty("TRecursiveExpenses")]
    public virtual TAccount? AccountFkNavigation { get; set; }

    [ForeignKey("CategoryTypeFk")]
    [InverseProperty("TRecursiveExpenses")]
    public virtual TCategoryType? CategoryTypeFkNavigation { get; set; }

    [ForeignKey("FrequencyFk")]
    [InverseProperty("TRecursiveExpenses")]
    public virtual TRecursiveFrequency FrequencyFkNavigation { get; set; } = null!;

    [ForeignKey("ModePaymentFk")]
    [InverseProperty("TRecursiveExpenses")]
    public virtual TModePayment? ModePaymentFkNavigation { get; set; }

    [ForeignKey("PlaceFk")]
    [InverseProperty("TRecursiveExpenses")]
    public virtual TPlace? PlaceFkNavigation { get; set; }

    [InverseProperty("RecursiveExpenseFkNavigation")]
    public virtual ICollection<THistory> THistories { get; set; } = new List<THistory>();
}
